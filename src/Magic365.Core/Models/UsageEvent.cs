using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace Magic365.Core.Models;
public class UsageEvent : ITableEntity
{

    public UsageEvent()
    {
        SessionId = string.Empty;
        EventDate = DateTime.UtcNow;
        PartitionKey = DateTime.UtcNow.Year.ToString();
        RowKey = Guid.NewGuid().ToString();
    }

    public string? UserId
    {
        get; set;
    }


    public string SessionId
    {
        get; set;
    }

    public string Event
    {
        get; set;
    } = string.Empty;

    public string? EventDataAsJson
    {
        get; set;
    }

    public DateTime EventDate
    {
        get; set;
    }

    public string PartitionKey
    {
        get;
        set;
    }
    public string RowKey
    {
        get;
        set;
    }
    public DateTimeOffset? Timestamp
    {
        get;
        set;
    }
    public ETag ETag
    {
        get;
        set;
    }
}
