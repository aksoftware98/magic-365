using Magic365.Core.Interfaces;
using Magic365.Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            .AddScoped<INoteAnalyzingService, NoteAnalyzingService>()
        .AddScoped<IGraphDataService, MicrosoftGraphDataService>()
        .AddScoped<IPlansStorageService>(sp => new TableStoragePlansService(config["AzureStorageConnectionString"]))
        .AddScoped<IUsageTrackingService>(sp => new TableStorageUsageTrackingService(config["AzureStorageConnectionString"]));
    }

    public static IServiceCollection AddAnalyzerFunctionOptions(this IServiceCollection services, IConfiguration config)
    {
        return services.AddSingleton(sp =>
        {
            var options = new Options.AIAnalyzerOptions();
            options.AnalyzeUrl = config["AnalyzerFunction:Url"];
            options.ApiKey = config["AnalyzerFunction:ApiKey"];
            return options;
        });
    }

    public static IServiceCollection AddPlanningService(this IServiceCollection services)
        => services.AddScoped<IPlanningService, GraphPlanningService>();
}
