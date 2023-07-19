using Magic365.Core.Interfaces;
using Magic365.Core.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace Magic365.Core.Services;

public class AccessTokenProvider : IGraphAccessTokenProvider
{

    private readonly MicrosoftAuthenticationOptions _options;
    private readonly ILogger<AccessTokenProvider> _logger;
    public AccessTokenProvider(MicrosoftAuthenticationOptions options,
                               ILogger<AccessTokenProvider> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync(string scope, string idToken, string tenantId) => await GetTokenAsync(new string[] { scope }, idToken, tenantId);

    public async Task<string> GetTokenAsync(string[] scopes, string idToken, string tenantId)
    {
        var confidentialClientApplication = ConfidentialClientApplicationBuilder
            .Create(_options.ClientId)
            .WithTenantId(tenantId)
            .WithClientSecret(_options.ClientSecret)
            .Build();

        try
        {
            var result = await confidentialClientApplication.AcquireTokenOnBehalfOf(scopes, new UserAssertion(idToken)).ExecuteAsync();
            return result.AccessToken;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unable to retrieve access token {ex.Message}", ex);
            throw new Exception("Unable to retrieve access token", ex);
        }
    }
}
