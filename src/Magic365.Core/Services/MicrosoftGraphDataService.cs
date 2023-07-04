using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic365.Core.Interfaces;
using Magic365.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Models.ODataErrors;

namespace Magic365.Core.Services;
public class MicrosoftGraphDataService : IGraphDataService
{

    private readonly GraphServiceClient _graphClient;
    private readonly ILogger<MicrosoftGraphDataService> _logger;
    public MicrosoftGraphDataService(GraphServiceClient graphClient,
                                     ILogger<MicrosoftGraphDataService> logger)
    {
        _graphClient = graphClient;
        _logger = logger;
    }

    public async Task<IEnumerable<ToDoItemDto>> GetLastTasksAsync()
    {
        try
        {
            var tasksListResults = await _graphClient.Me
                                        .Todo
                                        .Lists
                                        .GetAsync(config =>
                                        {
                                            config.QueryParameters.Filter = $"displayName eq 'Tasks'";
                                        });
            var tasksList = tasksListResults?.Value?.FirstOrDefault();

            var tasks = await _graphClient.Me
                     .Todo
                     .Lists[tasksList.Id]
                     .Tasks
                     .GetAsync(options =>
                     {
                         options.QueryParameters.Top = 5;
                         options.QueryParameters.Filter = "status ne 'completed'";
                     });

            return tasks?.Value?.Select(t => new ToDoItemDto
            {
                Id = t.Id,
                IsDone = t.Status == Microsoft.Graph.Models.TaskStatus.Completed,
                Title = t.Title
            });
        }
        catch (ODataError ex)
        {
            _logger.LogError($"Response code: {ex.ResponseStatusCode}, Error message: {ex.Error?.Message}, while retrieving the top 5 tasks from Tasks list");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve the tasks list from Graph: {ex.Message}");
            throw;
        }


    }
    public async Task<IEnumerable<CalendarEventDto>> GetUpcomingEventsAsync()
    {
        try
        {
            var events = await _graphClient.Me.Calendar
                               .Events
                               .GetAsync(options =>
                               {
                                   options.QueryParameters.Top = 5;
                                   options.QueryParameters.Orderby = new[] { "start/dateTime" };
                               });

            return events?.Value?.Select(e => new CalendarEventDto
            {
                Id = e.Id,
                Subject = e.Subject,
                StartDate = DateTime.Parse(e.Start.DateTime),
                EndDate = DateTime.Parse(e.End.DateTime)
            });
        }
        catch (ODataError ex)
        {
            _logger.LogError($"Response code: {ex.ResponseStatusCode}, Error message: {ex.Error?.Message}, while retrieving the top 5 tasks from Tasks list");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to retrieve the tasks list from Graph: {ex.Message}");
            throw;
        }
    }
}
