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

namespace Magic365.Client.ViewModels.Services;

// TODO: Refactor the code of Http request and use a global user object instead of passing the token explicitly as below
public class HttpPlanningClient : HttpClientServiceBase, IPlanningClient
{
    public async Task<PlanDetails> AnalyzeNoteAsync(string? token, string? note)
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
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(error);
        }
    }

    public async Task<IEnumerable<PlanHistoryDto>> ListHistoryAsync(string token, string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentNullException(nameof(userId));

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"{BaseUrl}/plans/history?userId={userId}");

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<PlanHistoryDto>>();
            return result ?? Enumerable.Empty<PlanHistoryDto>();
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(error);
        }
    }

    public async Task<IEnumerable<ToDoItemDto>> ListUndoneToDoTasksAsync(string? token)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"{BaseUrl}/tasks/list");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<ToDoItemDto>>();
            return result ?? Enumerable.Empty<ToDoItemDto>();
        }
        else
        {
            var content = await response.Content.ReadAsStringAsync();
            Debug.WriteLine(content);
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(error);
        }
    }

    public async Task<IEnumerable<CalendarEventDto>> ListUpcomingCalendarEventsAsync(string? token)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.GetAsync($"{BaseUrl}/events/list");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<IEnumerable<CalendarEventDto>>();
            return result ?? Enumerable.Empty<CalendarEventDto>();
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
