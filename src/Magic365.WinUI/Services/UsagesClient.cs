using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Magic365.Client.ViewModels;
using Magic365.Client.ViewModels.Exceptions;
using Magic365.Client.ViewModels.Interfaces;
using Magic365.Client.ViewModels.Models;
using Magic365.Client.ViewModels.Services;
using Magic365.Shared.DTOs;
using Magic365.Shared.Responses;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;

namespace Magic365.WinUI.Services;

public class HttpUsageTrackingClient : HttpClientServiceBase, IUsagesClient
{

    public async Task<string> StartUserSessionAsync(string? token, User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync($"{BaseUrl}/usages/start-session", new SaveUserSessionDto
        {
            DisplayName = user.FullName,
            Email = user.Email,
            UserId = user.Email,
        });

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<string>();
            return result;
        }
        else
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(error);
        }
    }

    public async Task TrackEventAsync(string? token, TrackUserEventDto request)
    {
        var eventData = request.EventData?.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(request.EventData)?.ToString()) ?? new Dictionary<string, string?>();
        eventData.Add("UserId", SessionVariables.SessionId);
        Analytics.TrackEvent(request.EventName, eventData);
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync($"{BaseUrl}/usages/track-event", request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
            throw new ApiException(error);
        }
    }
}