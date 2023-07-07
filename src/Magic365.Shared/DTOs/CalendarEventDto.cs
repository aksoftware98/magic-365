using System.Text.Json.Serialization;

namespace Magic365.Shared.DTOs;

public class CalendarEventDto
{

    public CalendarEventDto()
    {
        Id = string.Empty;
        Subject = string.Empty;
    }

    [JsonPropertyName("id")]
    public string Id
    {
        get;
        set;
    }

    [JsonPropertyName("subject")]
    public string Subject
    {

        get;
        set;
    }

    [JsonPropertyName("start")]
    public DateTime StartDate
    {
        get; set;
    }

    [JsonPropertyName("end")]
    public DateTime EndDate
    {

        get; set;
    }

    [JsonPropertyName("participants")]
    public IEnumerable<MeetingPerson>? Participants
    {
        get; set;
    }
}
