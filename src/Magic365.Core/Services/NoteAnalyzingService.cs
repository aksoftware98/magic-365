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
                dateTime = DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm tt")
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
								EndTime = DateTime.ParseExact(planItem.EndDateTime, "yyyy-MM-dd hh:mm tt", null),
								StartTime = DateTime.ParseExact(planItem.StartDateTime, "yyyy-MM-dd hh:mm tt", null),
								Type = PlanEntityType.Event
							});
							break;
						case "Meeting":
							items.Add(new PlanItem
							{
								Title = planItem.Action,
								EndTime = DateTime.ParseExact(planItem.EndDateTime, "yyyy-MM-dd hh:mm tt", null),
								StartTime = DateTime.ParseExact(planItem.StartDateTime, "yyyy-MM-dd hh:mm tt", null),
								Type = PlanEntityType.Meeting,
								People = planItem?.People?.Select(p => new MeetingPerson
								{
									Name = p
								})
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
	}
}
