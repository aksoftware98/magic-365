using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic365.Shared.DTOs;

namespace Magic365.Client.ViewModels.Models;
public class AutoSuggestQueryRequest
{

    public bool IsSearchNeeded
    {
        get; set;
    }

    public string? Query
    {
        get; set;
    }

    public MeetingPerson? SelectedPerson
    {
        get; set;
    }

}
