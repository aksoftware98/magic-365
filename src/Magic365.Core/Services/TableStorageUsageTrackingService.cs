using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Data.Tables;
using Magic365.Core.Interfaces;
using Magic365.Core.Models;
using Magic365.Shared.DTOs;
using Microsoft.Graph.Models.CallRecords;

namespace Magic365.Core.Services;
public class TableStorageUsageTrackingService : IUsageTrackingService
{
    private readonly TableClient _sessionsTable;
    private readonly TableClient _eventsTable; 

    public TableStorageUsageTrackingService(string connectionString)
    {
        _sessionsTable = new TableClient(connectionString, "UserSessions");
        _eventsTable = new TableClient(connectionString, "UserEvents");
    }

    public async Task<string> SaveUserSessionAsync(SaveUserSessionDto session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        var sessionEntity = new UserSession
        {
            DisplayName = session.DisplayName,
            Email = session.Email,
            SessionId = Guid.NewGuid().ToString(),
            UserId = session.UserId,
        };
        await _sessionsTable.CreateIfNotExistsAsync();
        await _sessionsTable.AddEntityAsync(sessionEntity);

        return sessionEntity.SessionId;

    }
    public async Task TerminateSessionAsync(string sessionId) => throw new NotImplementedException();
    public async Task TrackEventAsync(TrackUserEventDto eventDetails)
    {
        if (eventDetails == null)
            throw new ArgumentNullException(nameof(eventDetails));

        var eventEntity = new UsageEvent
        {
            SessionId = eventDetails.SessionId,
            UserId = eventDetails.UserId,
            EventDataAsJson = eventDetails == null ? null : JsonSerializer.Serialize(eventDetails.EventData),
            Event = eventDetails.EventName,
        };
        await _eventsTable.CreateIfNotExistsAsync();
        await _eventsTable.AddEntityAsync(eventEntity);
    }
}
