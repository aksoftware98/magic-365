using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic365.Client.ViewModels.Models;
using Magic365.Shared.DTOs;

namespace Magic365.Client.ViewModels.Interfaces;
public interface IUsagesClient
{
    Task<string> StartUserSessionAsync(string? token, User user);
    Task TrackEventAsync(string? token, TrackUserEventDto request);

}
