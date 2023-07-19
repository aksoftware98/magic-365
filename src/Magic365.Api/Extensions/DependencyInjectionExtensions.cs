using Azure.Core;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Net.Http.Headers;

namespace Magic365.Api.Extensions
{
	public static class DependencyInjectionExtensions
	{

		public static IServiceCollection AddAuthorizedHttpClient(this IServiceCollection services)
		{
			return services.AddScoped(sp =>
			{
				var context = sp.GetRequiredService<IHttpContextAccessor>();
				var token = context.ExtractToken();

				var client = new HttpClient();
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				return client;
			});
		}

	}
}
