using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Magic365.Shared.DTOs;
public class ToDoItemDto
{

    public ToDoItemDto()
    {
        Id = string.Empty;
        Title = string.Empty;
    }

    [JsonPropertyName("id")]
    public string Id
    {
        get; set;
    }

    [JsonPropertyName("title")]
    public string Title
    {
        get; set;
    }

    [JsonPropertyName("isDone")]
    public bool IsDone
    {
        get; set;
    }



}
