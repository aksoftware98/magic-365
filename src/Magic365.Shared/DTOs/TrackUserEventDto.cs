using System.Text.Json.Serialization;

namespace Magic365.Shared.DTOs;

/// <summary>
/// Track the usage of the user activity inside the app
/// </summary>
public class TrackUserEventDto
{

    public TrackUserEventDto()
    {
        SessionId = string.Empty;
        EventName = string.Empty;
        UserId = string.Empty;
    }

    [JsonPropertyName("sessionId")]
    public string SessionId
    {
        get; set;
    }

    [JsonPropertyName("eventName")]
    public string EventName
    {
           get; set;
    }

    [JsonPropertyName("userId")]
    public string UserId
    {
        get; set;
    }

    [JsonPropertyName("eventData")]
    public object? EventData
    {
        get;set;
    }
}