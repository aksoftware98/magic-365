using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Magic365.Shared.DTOs;

/// <summary>
/// Used for usage tracking of the users inside the system.
/// </summary>
public class SaveUserSessionDto
{

    public SaveUserSessionDto()
    {
        UserId = string.Empty;
        Email = string.Empty;
        DisplayName = string.Empty;
    }

    [JsonPropertyName("userId")]
    public string UserId
    {
        get; set;
    }

    [JsonPropertyName("email")]
    public string Email
    {
        get; set;
    }

    [JsonPropertyName("displayName")]
    public string DisplayName
    {
        get; set;
    }
}
