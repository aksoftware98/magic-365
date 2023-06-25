using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Magic365.AI.Functions
{
    public class AnalyzeNote
    {

        private readonly IConfiguration _configuration;

		public AnalyzeNote(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[FunctionName("AnalyzeNote")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            // API Key  

            var body = await new StreamReader(req.Body).ReadToEndAsync();
            if (string.IsNullOrWhiteSpace(body))
                return new BadRequestObjectResult(new
                {
                    message = "Request body is required"
                });

            var noteRequest = JsonSerializer.Deserialize<NoteRequest>(body);
            if (string.IsNullOrWhiteSpace(noteRequest.Message))
            {
				return new BadRequestObjectResult(new
				{
					message = "Message content is required, You can set the value by setting the property 'message' in the body"
				});
			}    

            if (noteRequest.Message.Length > 2000)
            {
				return new BadRequestObjectResult(new
				{
					message = "Message content cannot be more than 2000 characters"
				});
			}

            var apiKey = _configuration["ApiKey"];
			string prompt = @"Analyze the following plan as separate sentences and respond to me a JSON array containing list of objects, each object has the following properties: type, action, startDate, startTime, endDate, endTime, people
                            ---
                            user Message here
                            ---

                            Each sentence must be a type of three: ToDoItem, Event, Meeting. the sentence must be only one type and cannot have two types at the same time. 
                            the ToDoItem sentence is the sentence that represents actions and doesn't contain a time or people.
                            the Event is the sentence that represents action but with a time and doesn't contain people. 
                            the Meeting is like Event but it has people's names involved. 
                            action property is the actual action the user wants in the sentence.
                            define the startDate and endDate for the Event and the meeting and print it in the following format ""yyyy-MM-dd"" and if the date is not specific set it to null. endDate if it's not defined also set it to null. 
                            for the startTime and endTime for the Event and the Meeting should be in the following format ""hh:mm:ss tt"" and if the endTime is not specific calculate at 30 mins from the startTime. 
                            The Meeting sentence contains should contain at least one person name, so show this name as JSON string array in the object. 
                            The final output should be like the following example
                            ---
                            [
                              {
                                ""type"": ""ToDoItem"",
                                ""action"": ""Go to the super market"",
                                ""startDate"": ""2023-06-18""
                              },
                              {
                                ""type"": ""Event"",
                                ""startDate"": ""2023-06-18"",
                                ""action"": ""Have a coffee"",
                                ""startTime"": ""04:00 PM"",
                                ""endDate"": ""2023-06-18"",
                                ""endTime"": ""04:30 PM""
                              },
                              {
                                ""type"": ""Meeting"",
                                ""startDate"": ""2023-06-18"",
                                ""action"": ""Study AI"",
                                ""startTime"": ""06:00 PM"",
                                ""endDate"": ""2023-06-18"",
                                ""endTime"": ""08:00 PM"",
                                ""people"": [""Ahmad""]
                              }
                            ]
                            ---";

			using var client = new HttpClient(); 
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", new
            {
                model = "gpt-3.5-turbo",
                messages = new[] {
                    new
                    {
                        role = "system", 
                        content = prompt
                    },
                    new
                    {
                        role = "user",
                        content = noteRequest.Message
					}
                }
            });
            
            var result = await response.Content.ReadFromJsonAsync<NoteResponse>();
            var plan = JsonSerializer.Deserialize<PlanItem[]>(result?.Choices[0].Message.Content);

            return new OkObjectResult(plan);
        }
    }

    public class NoteRequest
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class NoteResponse
    {
        [JsonPropertyName("choices")]
        public Choice[] Choices { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public ResponseMessage Message { get; set; }    
    }

    public class ResponseMessage
    {
        [JsonPropertyName("role")]
		public string Role { get; set; }

        [JsonPropertyName("content")]
		public string Content { get; set; }
    }

    public class PlanItem
    {
        [JsonPropertyName("action")]
        public string Action { get; set; }
		[JsonPropertyName("startTime")]
		public string StartTime { get; set; }
		[JsonPropertyName("endTime")]
		public string EndTime { get; set; }
		[JsonPropertyName("startDate")]
		public string StartDate { get; set; }
		[JsonPropertyName("endDate")]
		public string EndDate { get; set; }
		[JsonPropertyName("people")]
		public string[] People { get; set; }
    }
}
