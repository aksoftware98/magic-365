using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace Magic365.Core.Models;
public class PlanMetadata : ITableEntity
{

    public PlanMetadata()
    {
        Note = string.Empty;
        PlanAsJson = string.Empty;
        UserId = string.Empty;
        ToDoItemsCount = 0;
        PlanDate = DateTime.UtcNow;
        PartitionKey = PlanDate.Year.ToString();
        RowKey = Guid.NewGuid().ToString();
    }

    public string Note
    {
        get;
        set;
    }

    public string UserId
    {
        get; set;
    }

    public string PlanAsJson
    {
        get;
        set;
    }

    public int ToDoItemsCount
    {
        get;
        set;
    }

    public int MeetingsCount
    {
        get;
        set;
    }

    public int EventsCount
    {
        get;
        set;
    }

    public DateTime PlanDate
    {

        get;
        set;
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
