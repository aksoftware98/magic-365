using Magic365.Core.Interfaces;
using Magic365.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Magic365.Core
{
	public static class DependencyInjectionExtensions
	{

		public static IServiceCollection AddLanguageUnderstandingService(this IServiceCollection services, string apiKey)
		{
			if (string.IsNullOrWhiteSpace(apiKey))
				throw new ArgumentNullException(nameof(apiKey));
			
			return services.AddScoped<ILanguageUnderstnadingService>(sp => new ConversationalLanguageUnderstandingService(sp.GetRequiredService<HttpClient>(), apiKey));
		}

		public static IServiceCollection AddPlanningService(this IServiceCollection services)
			=> services.AddScoped<IPlanningService, GraphPlanningService>(); 
	}
}
