using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic365.Shared.DTOs;

namespace Magic365.Core.Interfaces;
public interface IUsageTrackingService
{

    Task<string> SaveUserSessionAsync(SaveUserSessionDto session);

    Task TrackEventAsync(TrackUserEventDto eventDetails);

    Task TerminateSessionAsync(string sessionId);
}
