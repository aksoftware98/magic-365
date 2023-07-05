using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;

namespace Magic365.Core.Models;
public class UserSession : ITableEntity
{

    public UserSession()
    {
        PartitionKey = "UserSession";
        RowKey = Guid.NewGuid().ToString();
        UserId = string.Empty;
        SessionId = string.Empty;
        StartedAt = DateTime.UtcNow;
    }

    public string UserId
    {

        get;
        set;
    }

    public string SessionId
    {

        get;
        set;
    }

    public DateTime StartedAt
    {
        get;
        set;
    }

    public DateTime? EndDateAt
    {
        get;
        set;
    }

    public string? DisplayName
    {
        get;
        set;
    }

    public string? Email
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
