using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Shared.DTOs;
public class PlanHistoryDto
{

    public PlanHistoryDto()
    {
        Note = string.Empty;
    }

    public string Note
    {
        get; set;
    }

    public DateTime Date
    {
        get;set;
    }

    public int ToDoItemsCount
    {
        get; set;
    }

    public int MeetingsCount
    {
        get;set;
    }

    public int EventsCount
    {
        get; set;
    }

}
