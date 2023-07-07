using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Shared.DTOs;
public class DashboardDto
{

    public DashboardDto()
    {
        ToDoItems = Enumerable.Empty<ToDoItemDto>();
        CalendarEvents = Enumerable.Empty<CalendarEventDto>();
    }

    public IEnumerable<ToDoItemDto> ToDoItems
    {
        get; set;
    }

    public IEnumerable<CalendarEventDto> CalendarEvents
    {
    
           get; set;
       }

}
