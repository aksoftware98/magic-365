using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Core.Options;
public class MicrosoftAuthenticationOptions
{

    public string ClientId
    {
        get; set;
    } = null!;

    public string ClientSecret
    {
    
           get; set;
    } = null!;



}
