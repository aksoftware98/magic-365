﻿using Magic365.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Client.ViewModels.Interfaces;

public interface IPlanningClient
{
    Task<PlanDetails> AnalyzeNoteAsync(string? token, string? note);

    Task SubmitPlanAsync(string? token, PlanDetails request);

    Task<IEnumerable<MeetingPerson>> SearchContactAsync(string? token, string query);
    Task<IEnumerable<ToDoItemDto>> ListUndoneToDoTasksAsync(string? token);
    Task<IEnumerable<CalendarEventDto>> ListUpcomingCalendarEventsAsync(string? token);

    Task<IEnumerable<PlanHistoryDto>> ListHistoryAsync(string? token, string userId);

}
