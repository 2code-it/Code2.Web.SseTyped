using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Code2.Web.SseTyped
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddSseTyped(this IServiceCollection services)
		{
			services.AddSingleton<ISseConnectionManager, SseConnectionManager>();
			services.AddSingleton<ISseService, SseService>();
			return services;
		}

		public static IApplicationBuilder UseSseTyped(this IApplicationBuilder app)
		{
			app.UseSseTyped(SseMiddleware.GetDefaultOptions());
			return app;
		}

		public static IApplicationBuilder UseSseTyped(this IApplicationBuilder app, Action<SseMiddlewareOptions> action)
		{
			SseMiddlewareOptions options = SseMiddleware.GetDefaultOptions();
			action(options);
			app.UseSseTyped(options);
			return app;
		}

		public static IApplicationBuilder UseSseTyped(this IApplicationBuilder app, string[]? allowedTypeNames = null, string? rootPath = null)
		{
			SseMiddlewareOptions options = SseMiddleware.GetDefaultOptions();
			if (allowedTypeNames is not null) options.AllowedTypeNames = allowedTypeNames;
			if (rootPath is not null) options.RootPath = rootPath;
			app.UseSseTyped(options);
			return app;
		}

		public static IApplicationBuilder UseSseTyped(this IApplicationBuilder app, SseMiddlewareOptions options)
		{
			app.UseMiddleware<SseMiddleware>(options);
			return app;
		}
	}
}
