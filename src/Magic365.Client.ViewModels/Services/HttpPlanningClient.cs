using Magic365.Client.ViewModels.Exceptions;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Shared.DTOs;
using Magic365.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Magic365.Client.ViewModels.Services
{
	// TODO: Refactor the code of Http request and use a global user object instead of passing the token excplicityly as below
	public class HttpPlanningClient : IPlanningClient
	{

		/* COMMENT THE FOLLWOING LINE AND UNCOMMENT THE AFTER TO USE THE LOCAL API IN DEBUG MODE INSTEAD OF THE ONLINE VERSION */
		//private const string BaseUrl = "https://Magic365.azurewebsites.net";

		/* UNCOMMENT THIS FOR LOCAL TESTING */
		private const string BaseUrl = "https://localhost:7210";

		public async Task<PlanDetails> AnalyzeNoteAsync(string token, string note)
		{
			if (string.IsNullOrWhiteSpace(note))
				throw new ArgumentNullException(nameof(note));

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			var response = await client.PostAsJsonAsync($"{BaseUrl}/analyze-note", new
			{
				query = note
			});

			if (response.IsSuccessStatusCode)
			{
				var result = await response.Content.ReadFromJsonAsync<PlanDetails>();
				return result ?? new(); 
			}
			else
			{
				var content = await response.Content.ReadAsStringAsync();
				Debug.WriteLine(content);
				var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
				throw new ApiException(error);
			}
		}

        public async Task<IEnumerable<MeetingPerson>> SearchContactAsync(string token, string query)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{BaseUrl}/contacts/search?query={query}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<IEnumerable<MeetingPerson>>();
                return result ?? Enumerable.Empty<MeetingPerson>();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Debug.WriteLine(content);
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                throw new ApiException(error);
            }
        }

        public async Task SubmitPlanAsync(string? token, PlanDetails request)
		{

			using var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

			var response = await client.PostAsJsonAsync($"{BaseUrl}/submit-plan", request);

			if (!response.IsSuccessStatusCode)
			{ 
				var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
				throw new ApiException(error);
			}
		}
	}
}
