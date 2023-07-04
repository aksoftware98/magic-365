using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic365.Shared.DTOs;

namespace Magic365.Core.Interfaces;
public interface IGraphDataService
{

    Task<IEnumerable<ToDoItemDto>> GetLastTasksAsync(); 

    Task<IEnumerable<CalendarEventDto>> GetUpcomingEventsAsync();

}
