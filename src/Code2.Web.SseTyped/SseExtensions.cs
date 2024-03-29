using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Code2.Web.SseTyped
{
	public static class SseExtensions
	{
		public static void AddSseTyped(this IServiceCollection services)
		{
			services.AddSingleton<ISseConnectionManager, SseConnectionManager>();
			services.AddSingleton<ISseService, SseService>();
		}

		public static void UseSseTyped(this IApplicationBuilder app)
		{
			app.UseSseTyped(SseMiddleware.GetDefaultOptions());
		}

		public static void UseSseTyped(this IApplicationBuilder app, Action<SseMiddlewareOptions> action)
		{
			SseMiddlewareOptions options = SseMiddleware.GetDefaultOptions();
			action(options);
			app.UseSseTyped(options);
		}

		public static void UseSseTyped(this IApplicationBuilder app, string[]? allowedTypeNames = null, string? rootPath = null)
		{
			SseMiddlewareOptions options = SseMiddleware.GetDefaultOptions();
			if (!(allowedTypeNames is null)) options.AllowedTypeNames = allowedTypeNames;
			if (!(rootPath is null)) options.RootPath = rootPath;
			app.UseSseTyped(options);
		}

		public static void UseSseTyped(this IApplicationBuilder app, SseMiddlewareOptions options)
		{
			app.UseMiddleware<SseMiddleware>(options);
		}
	}
}
