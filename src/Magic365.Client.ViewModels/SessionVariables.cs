using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Magic365.Client.ViewModels.Models;

namespace Magic365.Client.ViewModels;
public static class SessionVariables
{

    public static User User
    {
        get; set;
    }


    public static string SessionId
    {
        get;
        set;
    }

}
