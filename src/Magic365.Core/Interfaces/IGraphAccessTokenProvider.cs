using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Web;

namespace Magic365.Core.Interfaces;
public interface IGraphAccessTokenProvider
{

    Task<string> GetTokenAsync(string[] scopes, string idToken, string tenantId);
    Task<string> GetTokenAsync(string scope, string idToken, string tenantId);

}
