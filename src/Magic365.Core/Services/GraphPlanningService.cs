﻿using Magic365.Core.Exceptions;
using Magic365.Core.Interfaces;
using Magic365.Core.Models;
using Magic365.Core.Options;
using Magic365.Shared;
using Magic365.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Models.ODataErrors;
using Microsoft.Kiota.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Magic365.Core.Services;

public class GraphPlanningService : IPlanningService
{

    //private readonly ILanguageUnderstandingService _language;
    private readonly GraphServiceClient _graph;
    private readonly HttpClient _httpClient;
    private readonly IPlansStorageService _plansStorage;
    private readonly IGraphAccessTokenProvider _tokenProvider;
    private readonly IdentityOptions _identity;
    private readonly ILogger<GraphPlanningService> _logger;
    public GraphPlanningService(GraphServiceClient graph,
                                HttpClient httpClient,
                                ILogger<GraphPlanningService> logger,
                                IPlansStorageService plansStorage,
                                IGraphAccessTokenProvider tokenProvider,
                                IdentityOptions identity)
    {
        _graph = graph;
        _httpClient = httpClient;
        _logger = logger;
        _plansStorage = plansStorage;
        _tokenProvider = tokenProvider;
        _identity = identity;
    }
    #region Analyze Note 
    /// <summary>
    /// Understand a note using AI and build a productivity plan out of it for user to review and fill the remaining data
    /// </summary>
    /// <param name="note">Note query request</param>
    /// <returns></returns>
    [Obsolete("The method is obsolete. Use AnalyzeNoteAsync instead within the interface INoteAnalyzeService.")]
    public Task<PlanDetails> AnalyzeNoteAsync(SubmitNoteRequest note)
    {
        //var items = new List<PlanItem>();

        //string[] sentences = PrepareSentences(note);

        //foreach (var sentence in sentences)
        //{
        //	//var analyzeResponse = await _language.AnalyzeTextAsync(sentence);
        //	var analyzeResult = analyzeResponse.Result;
        //	if (analyzeResult?.Prediction?.TopIntent?.Equals("None") ?? false)
        //		continue;

        //	PlanEntityType entityType = ExtractEntityType(analyzeResult);

        //	switch (entityType)
        //	{
        //		case PlanEntityType.Event:
        //			{
        //				PlanItem planItem = ExtractEventItem(note, analyzeResult);
        //				items.Add(planItem);
        //				break;
        //			}
        //		case PlanEntityType.Meeting:
        //			{
        //				PlanItem planItem = await ExtractMeetingItem(note, analyzeResult);
        //				items.Add(planItem);
        //				break;
        //			}
        //		case PlanEntityType.ToDoItem:
        //			{
        //				PlanItem planItem = ExtractToDoItem(sentence, analyzeResult);
        //				items.Add(planItem);
        //				break;
        //			}
        //		default:
        //			continue;
        //	}
        //}

        return Task.FromResult(new PlanDetails
        {
            Items = Enumerable.Empty<PlanItem>()
        });
    }
    #region Build To-Do Item 
    /// <summary>
    /// Extract a to-do item from the sentence analzying result that has been identified as a to-do item
    /// </summary>
    /// <param name="sentence"></param>
    /// <param name="analyzeResult"></param>
    /// <returns></returns>
    private PlanItem ExtractToDoItem(string sentence, PredicationResult? analyzeResult)
    {
        // Get the entity
        string title = sentence;
        var eventDescription = analyzeResult?.Prediction?.Entities?.FirstOrDefault(e => e.Category.Equals("Event"));

        if (eventDescription != null)
            title = CapitilizeFirstLetter(eventDescription.Text);

        var planItem = new PlanItem()
        {
            Title = title,
            Type = PlanEntityType.ToDoItem,
        };
        return planItem;
    }
    #endregion

    #region Build Event Object 
    /// <summary>
    /// Extract a event item from the sentence analzying result that has been identified as a event item
    /// </summary>
    /// <param name="sentence"></param>
    /// <param name="analyzeResult"></param>
    /// <returns></returns>
    private PlanItem ExtractEventItem(SubmitNoteRequest note, PredicationResult? analyzeResult)
    {
        // Get the entity
        var eventDescription = analyzeResult?.Prediction?.Entities?.FirstOrDefault(e => e.Category.Equals("Event"));
        DateTime startTime = ExtractStartTimeFromEntities(note, analyzeResult);
        DateTime endTime = startTime.AddHours(1);
        var planItem = new PlanItem()
        {
            Title = CapitilizeFirstLetter(eventDescription.Text),
            Type = PlanEntityType.Event,
            StartTime = startTime,
            EndTime = endTime,
        };
        return planItem;
    }
    #endregion

    #region Build Meeting Item
    /// <summary>
    /// Building a meeting plan item out of the sentence analzying result that has been identified as a meeting
    /// </summary>
    /// <param name="note"></param>
    /// <param name="analyzeResult"></param>
    /// <returns></returns>
    private async Task<PlanItem> ExtractMeetingItem(SubmitNoteRequest note, PredicationResult? analyzeResult)
    {
        var eventDescription = analyzeResult?.Prediction?.Entities?.FirstOrDefault(e => e.Category.Equals("Event"));
        DateTime startTime = ExtractStartTimeFromEntities(note, analyzeResult);
        DateTime endTime = startTime.AddHours(1);

        var meetingPeople = new List<MeetingPerson>();
        var people = analyzeResult?.Prediction?.Entities?.Where(e => e.Category.Equals("Person"));
        var userEntities = await FetchContactsFromGraphAsync(people);

        foreach (var person in people)
        {
            var contact = BuildTheContactObject(meetingPeople, userEntities, person);
            meetingPeople.Add(contact);
        }

        var planItem = new PlanItem()
        {
            Title = ConstructMeetingTitle(eventDescription, people),
            Type = PlanEntityType.Meeting,
            StartTime = startTime,
            EndTime = endTime,
            People = meetingPeople
        };
        return planItem;
    }
    #endregion

    #region Helper Methods 
    private string ConstructMeetingTitle(Models.Entity? eventDescription, IEnumerable<Models.Entity> people)
    {
        return !string.IsNullOrWhiteSpace(eventDescription?.Text) ? CapitilizeFirstLetter(eventDescription?.Text) : $"Meeting with {string.Join(",", people.Select(p => p.Text))}";
    }

    /// <summary>
    /// Build the <see cref="MeetingPerson" /> object populated with the data from the entities and the result of the contacts retrieved from Graph or build a simple object only from the name available in the sentence
    /// </summary>
    /// <param name="meetingPeople"></param>
    /// <param name="userEntities"></param>
    /// <param name="person"></param>
    /// <returns></returns>
    private static MeetingPerson BuildTheContactObject(List<MeetingPerson> meetingPeople, ContactCollectionResponse? userEntities, Models.Entity? person)
    {
        var contact = userEntities?.Value?.FirstOrDefault(u => u.DisplayName.Contains(person.Text));
        if (contact != null)
        {
            return new MeetingPerson()
            {
                Id = contact.Id,
                Email = contact.EmailAddresses?.FirstOrDefault()?.Address,
                Name = contact.DisplayName,
                AddContact = false,
                AddEmailToContact = false,
            };
        }
        else
            return new MeetingPerson()
            {
                Name = person.Text,
                AddContact = true,
                AddEmailToContact = true,
            };
    }

    private DateTime ExtractStartTimeFromEntities(SubmitNoteRequest note, PredicationResult? analyzeResult)
    {
        var eventTime = analyzeResult?.Prediction?.Entities?.FirstOrDefault(e => e.Category.Equals("Time"));
        var timeDataType = eventTime?.Resolutions?.FirstOrDefault();
        DateTime startTime = DateTime.Now.AddDays(1);
        if (timeDataType != null)
        {
            if (timeDataType.DateTimeSubKind.Equals("Date"))
            {
                startTime = DateTime.ParseExact($"{note.Date:yyyy-MM-dd} {timeDataType.Value}", "yyyy-MM-dd HH:mm:ss", null);
            }
            else if (timeDataType.DateTimeSubKind.Equals("DateTime"))
            {
                startTime = DateTime.ParseExact(timeDataType.Value, "yyyy-MM-dd HH:mm:ss", null);
            }
        }

        return startTime;
    }


    private static PlanEntityType ExtractEntityType(PredicationResult? analyzeResult)
    {
        return analyzeResult?.Prediction?.TopIntent switch
        {
            "AddReminder" => PlanEntityType.Event,
            "CreateMeeting" => PlanEntityType.Meeting,
            "AddToDo" => PlanEntityType.ToDoItem,
            _ => PlanEntityType.None,
        };
    }

    private static string[] PrepareSentences(SubmitNoteRequest note)
    {
        note.Query = note.Query
                                        .Replace(",", "\r")
                                        .Replace(".", "\r")
                                        .Replace(";", "\r");
        var sentences = note.Query
                            .Split(new[] { "\r\n", "\r" }, StringSplitOptions.None)
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .ToArray();
        return sentences;
    }

    private string CapitilizeFirstLetter(string text)
    {
        if (text.Length == 0)
            return text;
        else if (text.Length == 1)
            return text.ToUpper();
        else
            return text.Substring(0, 1).ToUpper() + text.Substring(1);
    }
    #endregion
    #endregion

    #region Submit Plan to Graph 
    public async Task SubmitPlanAsync(PlanDetails plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        if (string.IsNullOrWhiteSpace(plan.Note))
            throw new DominException("Note cannot be empty");
        if (string.IsNullOrWhiteSpace(plan.UserId))
            throw new DominException("User cannot be empty");

        // TODO: Validate the plan details object
        if (plan.Items.Count() > 12)
            throw new DominException("The free version supports only up to 12 tasks a day");
        // TODO: Remove the limitation of the date from tomorrow to understand it from the AI or allow the user to choose a date before start populating the note
        
        // Get the tasks list of the to-do list
        var tasksList = await GetDefaultTasksListFromGraphAsync();
        var userTimeZone = await GetUserTimeZoneFromGraphAsync();

        // Build the batch requests
        var batchReqeustContent = new BatchRequestContentCollection(_graph);

        await BuildToDoItemsGraphRequestsAsync(plan, tasksList, userTimeZone, batchReqeustContent);
        await BuildMeetingsGraphRequestsAsync(plan, userTimeZone, batchReqeustContent);
        await BuildEventsGraphRequestsAsync(plan, userTimeZone, batchReqeustContent);

        await _graph.Batch.PostAsync(batchReqeustContent);

        await _plansStorage.SavePlanAsync(new PlanMetadata
        {
            Note = plan.Note,
            UserId = plan.UserId,
            PlanAsJson = JsonSerializer.Serialize(plan),
            EventsCount = plan.Items.Count(i => i.Type == PlanEntityType.Event),
            ToDoItemsCount = plan.Items.Count(i => i.Type == PlanEntityType.ToDoItem),
            MeetingsCount = plan.Items.Count(i => i.Type == PlanEntityType.Meeting),
        });
    }

    #region Graph Calls 
    private async Task BuildEventsGraphRequestsAsync(PlanDetails plan, string userTimeZone, BatchRequestContentCollection batchReqeustContent)
    {
        // Check if there is meetings in the call 
        var events = plan.Items.Where(i => i.Type == PlanEntityType.Event);
        if (events.Any())
        {
            foreach (var item in events)
            {
                RequestInformation calendarEventRequest = BuildPostEventGraphRequest(userTimeZone, item);

                await batchReqeustContent.AddBatchRequestStepAsync(calendarEventRequest);
            }
        }
    }

    private async Task BuildMeetingsGraphRequestsAsync(PlanDetails plan, string userTimeZone, BatchRequestContentCollection batchReqeustContent)
    {
        // Build the events item batch requests 
        var meetings = plan.Items.Where(i => i.Type == PlanEntityType.Meeting);
        if (meetings.Any())
        {
            foreach (var item in meetings)
            {
                var calendarMeetingRequest = BuildPostMeetingGraphRequest(userTimeZone, item);

                foreach (var contact in item.People)
                {
                    if (contact.AddContact && string.IsNullOrWhiteSpace(contact.Id)) // Add the contact to Microsoft Graph if the contact is not added already 
                    {
                        RequestInformation contactRequest = BuildPostContactGraphRequest(contact);

                        await batchReqeustContent.AddBatchRequestStepAsync(contactRequest);
                    }
                }

                await batchReqeustContent.AddBatchRequestStepAsync(calendarMeetingRequest);
            }
        }
    }

    private RequestInformation BuildPostContactGraphRequest(MeetingPerson contact)
    {
        var newContact = new Contact
        {
            DisplayName = contact.Name,
            GivenName = contact.Name,

        };

        if (!string.IsNullOrWhiteSpace(contact.Email))
        {
            newContact.EmailAddresses = new List<EmailAddress>()
                                {
                                    new EmailAddress
                                    {
                                        Address = contact.Email
                                    }
                                };
        }

        var contactRequest = _graph
                                .Me
                                .Contacts
                                .ToPostRequestInformation(newContact);
        return contactRequest;
    }

    private async Task BuildToDoItemsGraphRequestsAsync(PlanDetails plan, TodoTaskList tasksList, string userTimeZone, BatchRequestContentCollection batchReqeustContent)
    {
        // Build the to-do items batch requests 
        var todoItems = plan.Items.Where(i => i.Type == PlanEntityType.ToDoItem);

        if (todoItems.Any())
        {
            foreach (var item in todoItems)
            {
                var todoItemRequest = BuildPostToDoItemGraphRequest(tasksList, userTimeZone, item);
                await batchReqeustContent.AddBatchRequestStepAsync(todoItemRequest);
            }
        }
    }

    private RequestInformation BuildPostEventGraphRequest(string userTimeZone, PlanItem? item)
    {
        var calendarEvent = new Event
        {
            Subject = item.Title,
            Start = new DateTimeTimeZone
            {
                DateTime = new DateTime(item.StartTime.Value.Year, item.StartTime.Value.Month, item.StartTime.Value.Day, item.StartTime.Value.Hour, item.StartTime.Value.Minute, 0).ToString("yyyy-MM-ddTHH:mm:ss.ffff"),
                TimeZone = userTimeZone
            },
            End = new DateTimeTimeZone
            {
                DateTime = new DateTime(item.EndTime.Value.Year, item.EndTime.Value.Month, item.EndTime.Value.Day, item.EndTime.Value.Hour, item.EndTime.Value.Minute, 0).ToString("yyyy-MM-ddTHH:mm:ss.ffff"),
                TimeZone = userTimeZone
            }
        };

        var calendarEventRequest = _graph
                                    .Me
                                    .Events
                                    .ToPostRequestInformation(calendarEvent);
        return calendarEventRequest;
    }

    private RequestInformation BuildPostMeetingGraphRequest(string userTimeZone, PlanItem? item)
    {
        var calendarMeeting = new Event
        {
            Subject = item.Title,
            Start = new DateTimeTimeZone
            {
                DateTime = new DateTime(item.StartTime.Value.Year, item.StartTime.Value.Month, item.StartTime.Value.Day, item.StartTime.Value.Hour, item.StartTime.Value.Minute, 0).ToString("yyyy-MM-ddTHH:mm:ss.ffff"),
                TimeZone = userTimeZone
            },
            End = new DateTimeTimeZone
            {
                DateTime = new DateTime(item.EndTime.Value.Year, item.EndTime.Value.Month, item.EndTime.Value.Day, item.EndTime.Value.Hour, item.EndTime.Value.Minute, 0).ToString("yyyy-MM-ddTHH:mm:ss.ffff"),
                TimeZone = userTimeZone
            },
            IsOnlineMeeting = true,
            Attendees = item.People.Select(p => new Attendee
            {
                EmailAddress = new EmailAddress
                {
                    Address = p.Email,
                    Name = p.Name
                }
            }).ToList()
        };

        var calendarEventRequest = _graph
                                    .Me
                                    .Events
                                    .ToPostRequestInformation(calendarMeeting);
        return calendarEventRequest;
    }

    private RequestInformation BuildPostToDoItemGraphRequest(TodoTaskList? tasksList, string userTimeZone, PlanItem? item)
    {
        var todoItem = new TodoTask
        {
            Title = item.Title,
        };
        if (item.StartTime != null)
            todoItem.DueDateTime = new()
            {
                TimeZone = userTimeZone,
                DateTime = new DateTime(item.StartTime.Value.Year, item.StartTime.Value.Month, item.StartTime.Value.Day, item.StartTime.Value.Hour, item.StartTime.Value.Minute, 0).ToString("yyyy-MM-ddTHH:mm:ss.ffff"),
            };
        var todoItemRequest = _graph
                                .Me
                                .Todo
                                .Lists[tasksList?.Id]
                                .Tasks
                                .ToPostRequestInformation(todoItem);
        return todoItemRequest;
    }

    private async Task<TodoTaskList> GetDefaultTasksListFromGraphAsync()
    {
        try
        {
            var tasksListResults = await _graph.Me
                                        .Todo
                                        .Lists
                                        .GetAsync(config =>
                                        {
                                            config.QueryParameters.Filter = $"displayName eq 'Tasks'";
                                        });
            var tasksList = tasksListResults?.Value?.FirstOrDefault();
            return tasksList;
        }
        catch (ODataError ex)
        {
            _logger.LogError($"Response code: {ex.ResponseStatusCode}, Error message: {ex.Error?.Message}, while retrieving the tasks list from Graph");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve the tasks list from Graph: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieve the user timezone from the mailbox settings 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<string> GetUserTimeZoneFromGraphAsync()
    {
        var token = await _tokenProvider.GetTokenAsync(new[] { "MailboxSettings.Read" }, _identity.IdToken, _identity.TenantId);
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _httpClient.GetAsync("https://graph.microsoft.com/v1.0/me/mailboxsettings/timeZone");
        
        var content = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogWarning($"Time zone was not found, retrieving the default UTC");
                return "UTC";
            }
            var result = await response.Content.ReadFromJsonAsync<TimeZoneResponse>();
            return result?.Value ?? "UTC";
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Failed to retrieve the user timezone from Graph: {errorContent}");
            // Manage a better error handling 
            throw new Exception("Error getting user time zone");
        }

    }
    #endregion

    /// <summary>
    /// Fetch contacts from Microsoft Graphs that the user has mentioned in the sentence. 
    /// The contacts will help populate the plan with the emails of the contacts that the person has mentioned
    /// </summary>
    /// <param name="people">List of people mentioned in the sentence</param>
    /// <returns></returns>
    private async Task<ContactCollectionResponse?> FetchContactsFromGraphAsync(IEnumerable<Models.Entity>? people)
    {
        var contactsFilter = string.Join(" or ", people.Select(p => $"contains(displayName, '{p.Text}')"));
        try
        {
            var userEntities = (await _graph
                                    .Me
                                    .Contacts
                                    .GetAsync(config =>
                                    {
                                        config.QueryParameters.Filter = contactsFilter;
                                        config.QueryParameters.Select = new[] { "id", "displayName", "emailAddresses" };
                                    }));
            return userEntities;
        }
        catch (ODataError ex)
        {
            _logger.LogError($"Response status code: {ex.ResponseStatusCode}, Error message: {ex.Error?.Message}, while fetching the contacts from Graph");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while fetching the contacts from Graph: {ex.Message}");
            return null;
        }

    }


    #endregion

    public async Task<IEnumerable<MeetingPerson>> FetchContactsAsync(string query)
    {
        var contacts = await _graph
                                .Me
                                .Contacts
                                .GetAsync(config =>
                                {
                                    config.QueryParameters.Filter = $"contains(displayName, '{query}')";
                                    config.QueryParameters.Select = new[] { "id", "displayName", "emailAddresses" };
                                });

        return contacts?.Value?.Select(c => new MeetingPerson
        {

            Name = c.DisplayName,
            Email = c.EmailAddresses?.FirstOrDefault()?.Address
        }) ?? Enumerable.Empty<MeetingPerson>();
    }
}

internal class TimeZoneResponse
{
    [JsonPropertyName("value")]
    public string? Value
    {
        get; set;
    }
}
