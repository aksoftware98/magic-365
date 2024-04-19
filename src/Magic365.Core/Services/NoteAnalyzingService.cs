using Magic365.Core.Exceptions;
using Magic365.Core.Interfaces;
using Magic365.Core.Models;
using Magic365.Core.Options;
using Magic365.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Core.Services
{
	/// <summary>
	/// Analyze the submitted note by the user, build the plan based on the note analyzing result, and return it to the user. 
	/// </summary>
	public class NoteAnalyzingService : INoteAnalyzingService
	{

		private readonly HttpClient _httpClient;
		private readonly AIAnalyzerOptions _options;
		private readonly GraphServiceClient _graphClient;
		private readonly ILogger<NoteAnalyzingService> _logger;
		public NoteAnalyzingService(HttpClient httpClient,
									AIAnalyzerOptions options,
									GraphServiceClient graphClient,
									ILogger<NoteAnalyzingService> logger)
		{
			_httpClient = httpClient;
			_options = options;
			_graphClient = graphClient;
			_logger = logger;
		}

		public async Task<PlanDetails> AnalyzePlanAsync(string query)
		{
			if (string.IsNullOrWhiteSpace(query))
				throw new DominException("Submitted note is empty");

			var response = await _httpClient.PostAsJsonAsync($"{_options.AnalyzeUrl}?code={_options.ApiKey}", new
			{
				message = query,
                dateTime = DateTime.UtcNow
			});

            if (response.IsSuccessStatusCode)
            {
                var analyzerResult = await response.Content.ReadFromJsonAsync<AnalyzerResultItem[]>();
				if (analyzerResult == null)
				{
					_logger.LogError($"Failed to analyze the submitted note. Status code: {response.StatusCode}", await response.Content.ReadAsStringAsync());
					throw new DominException("Failed to analyze the submitted note");
				}

				var plan = new PlanDetails();
				var items = new List<PlanItem>();
				foreach (var planItem in analyzerResult)
				{
                    if ((planItem.StartDate == "null" || planItem.StartDate == null) && planItem.Type != "ToDoItem")
                        planItem.StartDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");  
                    if ((planItem.EndDate == "null" || planItem.EndDate == null) && planItem.Type != "ToDoItem")
                        planItem.EndDate = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
					if (planItem.StartTime == null && planItem.EndTime == null)
						planItem.Type = "ToDoItem";
					switch (planItem.Type)
					{
						case "ToDoItem":
							items.Add(new PlanItem
							{
								Title = planItem.Action,
								Type = PlanEntityType.ToDoItem,
							});
							break;
						case "Event":
							items.Add(new PlanItem
							{
								Title = planItem.Action,
								EndTime = DateTime.ParseExact(planItem.EndDateTime, "yyyy-MM-dd hh:mm:ss tt", null),
								StartTime = DateTime.ParseExact(planItem.StartDateTime, "yyyy-MM-dd hh:mm:ss tt", null),
								Type = PlanEntityType.Event
							});
							break;
                        case "Meeting":
                            var peopleNames = planItem?.People;
                            var meetingPeople = await FetchMeetingContactsAsync(planItem, peopleNames);
                            items.Add(new PlanItem
                            {
                                Title = planItem.Action,
                                EndTime = DateTime.ParseExact(planItem.EndDateTime, "yyyy-MM-dd hh:mm:ss tt", null),
                                StartTime = DateTime.ParseExact(planItem.StartDateTime, "yyyy-MM-dd hh:mm:ss tt", null),
                                Type = PlanEntityType.Meeting,
                                People = meetingPeople
                            });
                            break;
                        default:
							break;
					}
				}
				plan.Items = items;
				return plan;
            }
			else
			{
				_logger.LogError($"Failed to analyze the submitted note. Status code: {response.StatusCode}", await response.Content.ReadAsStringAsync());
				throw new DominException("Failed to analyze the submitted note");
			}
        }

        /// <summary>
        /// Fetching the meeting contacts from the user's contact list based on the names mentioned in the note.
        /// // TODO: The logic should be improved to handle the case when the user has multiple contacts with the same name. 
        /// #15 https://github.com/aksoftware98/magic-365/issues/15
        /// // TODO: Add support to fetch the meeting's contact email and display name from the tenant users list not only Graph contacts 
        /// #16 https://github.com/aksoftware98/magic-365/issues/16
        /// </summary>
        /// <param name="planItem"></param>
        /// <param name="peopleNames"></param>
        /// <returns></returns>
        private async Task<List<MeetingPerson>> FetchMeetingContactsAsync(AnalyzerResultItem? planItem, string[]? peopleNames)
        {
            var meetingPeople = planItem?.People?.Select(p => new MeetingPerson
            {
                Name = p
            }).ToList() ?? new List<MeetingPerson>();
            if (meetingPeople.Any())
            {
                var peopleFitler = string.Join(" or ", peopleNames.Select(s => $"startswith(displayName,'{s}')"));
                try
                {
                    var fetchContacts = await _graphClient
                                                .Me
                                                .Contacts
                                                .GetAsync(options =>
                                                {
                                                    options.QueryParameters.Filter = $"{peopleFitler}";
                                                });

                    foreach (var person in meetingPeople)
                    {
                        var existingContact = fetchContacts?.Value?.FirstOrDefault(c => c.DisplayName?.StartsWith(person.Name) ?? false);
                        if (existingContact != null)
                        {
                            person.Email = existingContact.EmailAddresses?.FirstOrDefault()?.Address;
                            person.Id = existingContact.Id;
                            person.Name = existingContact.DisplayName;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to fetch the meeting contacts. Error: {ex.Message}");
                }
                

            }

            return meetingPeople;
        }
    }
}
