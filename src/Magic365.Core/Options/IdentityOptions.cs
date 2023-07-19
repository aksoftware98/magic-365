using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Core.Options;
public class IdentityOptions
{

    public string? UserId
    {
        get; set;
    }

    public string? TenantId
    {
    
           get; set;
       }

    public string? Email
    {
        get; set;
    }

    public string? DisplayName
    {
        get; set;
    }

    public string? IdToken
    {
        get; set;
    }

}
