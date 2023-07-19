using Magic365.Core.Interfaces;
using Magic365.Core.Options;
using Magic365.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Core;

public static class DependencyInjectionExtensions
{

    public static IServiceCollection AddLanguageUnderstandingService(this IServiceCollection services, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentNullException(nameof(apiKey));

        return services.AddScoped<ILanguageUnderstandingService>(sp => new ConversationalLanguageUnderstandingService(sp.GetRequiredService<HttpClient>(), apiKey));
    }

    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration config)
    {
        return services.AddAnalyzerFunctionOptions(config)
            .AddPlanningService()
            .AddMicrosoftAuthenticationOptions(config)
            .AddGraphServiceClient()
            .AddScoped<INoteAnalyzingService, NoteAnalyzingService>()
            .AddScoped<IGraphDataService, MicrosoftGraphDataService>()
            .AddScoped<IGraphAccessTokenProvider, AccessTokenProvider>()
            .AddScoped<IPlansStorageService>(sp =>
            {
                var authOptions = sp.GetRequiredService<MicrosoftAuthenticationOptions>();
                var identityOptions = sp.GetRequiredService<IdentityOptions>();
                return new TableStoragePlansService(config["AzureStorageConnectionString"], authOptions, identityOptions);
                })
            .AddScoped<IUsageTrackingService>(sp => new TableStorageUsageTrackingService(config["AzureStorageConnectionString"]))
            .AddIdentityOptions();
    }

    public static IServiceCollection AddAnalyzerFunctionOptions(this IServiceCollection services, IConfiguration config)
    {
        return services.AddSingleton(sp =>
        {
            var options = new Options.AIAnalyzerOptions();
            options.AnalyzeUrl = config["AnalyzerFunctionUrl"];
            options.ApiKey = config["AnalyzerFunctionApiKey"];
            return options;
        });
    }

    public static IServiceCollection AddPlanningService(this IServiceCollection services)
        => services.AddScoped<IPlanningService, GraphPlanningService>();

    /// <summary>
    /// Register the settings associated with the Microsoft Authentication options to be used to fetch the token using the on-behalf-of flow
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    public static IServiceCollection AddMicrosoftAuthenticationOptions(this IServiceCollection services, IConfiguration config)
    {
        return services.AddSingleton(sp =>
               {
                   return new MicrosoftAuthenticationOptions
                   {
                       ClientId = config["AzureAd:ClientId"],
                       ClientSecret = config["AzureAd:ClientSecret"]
                   };
               });
    }

    /// <summary>
    /// Add the identity options related to the logged in user. 
    /// <see cref="IdentityOptions"/> contains a properties of the logged in user such as the user id, tenant id, email, name, etc.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddIdentityOptions(this IServiceCollection services)
    {
        return services.AddScoped(sp =>
        {
            var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (!httpContext.User.Identity.IsAuthenticated)
                return new();

            var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var user = httpContext.User;
            var userId = user.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
            var tenantId = user.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid")?.Value;
            var email = user.FindFirst("preferred_username")?.Value;
            var name = user.FindFirst("name")?.Value;

            return new IdentityOptions
            {
                DisplayName = name,
                Email = email,
                TenantId = tenantId,
                UserId = userId,
                IdToken = token
            };
        });
    }

    public static IServiceCollection AddGraphServiceClient(this IServiceCollection services)
    {
        return services.AddScoped(sp =>
        {
            var tokenProvider = sp.GetRequiredService<IGraphAccessTokenProvider>();
            var identity = sp.GetRequiredService<IdentityOptions>();
            return new GraphServiceClient(new BaseBearerTokenAuthenticationProvider(new TokenProvider(async () =>
            {
                return await tokenProvider.GetTokenAsync(new string[] { "https://graph.microsoft.com/.default" }, identity.IdToken, identity.TenantId);
            })));
        });
    }
}
