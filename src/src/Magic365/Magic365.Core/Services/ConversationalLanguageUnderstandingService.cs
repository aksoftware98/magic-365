using Magic365.Core.Interfaces;
using Magic365.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Core.Services
{
	public class ConversationalLanguageUnderstandingService : ILanguageUnderstnadingService
	{

		private readonly HttpClient _httpClient;
		private readonly string _apiKey;
		public ConversationalLanguageUnderstandingService(HttpClient httpClient, 
														  string apiKey)
		{
			_httpClient = httpClient;
			_apiKey = apiKey;
		}

		public async Task<AITextAnalyzeResponse> AnalyzeTextAsync(string query)
		{
			_httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
			var response = await _httpClient.PostAsJsonAsync("https://cls-Magic365ai.cognitiveservices.azure.com/language/:analyze-conversations?api-version=2022-05-01", new
			{
				analysisInput = new
				{
					conversationItem = new
					{
						text = query,
						id = "1",
						participantId = "1",
					}
				},
				parameters = new
				{
					projectName = "Magic365",
					deploymentName = "FivthModel",
					stringIndexType = "TextElement_V8",
				},
				kind = "Conversation",
			});

			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var result = await response.Content.ReadFromJsonAsync<AITextAnalyzeResponse>();
				return result  ?? new();
			}
			else
			{
				throw new Exception("Error");
			}
		}
	}
}
