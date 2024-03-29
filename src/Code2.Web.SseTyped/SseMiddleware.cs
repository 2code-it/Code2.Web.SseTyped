using Code2.Web.SseTyped.Internals;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public class SseMiddleware
	{
		public SseMiddleware(RequestDelegate next, IOptions<SseMiddlewareOptions> options)
			: this(next, options.Value) { }

		public SseMiddleware(RequestDelegate next, SseMiddlewareOptions options)
			: this(next, options, new SseHttpUtility()) { }

		internal SseMiddleware(RequestDelegate next, SseMiddlewareOptions options, ISseHttpUtility sseHttpUtility)
		{
			_next = next;
			_options = options;
			_sseHttpUtility = sseHttpUtility;
		}

		private readonly RequestDelegate _next;
		private readonly SseMiddlewareOptions _options;
		private readonly ISseHttpUtility _sseHttpUtility;

		private const string _defaultRootPath = "/sse";

		public async Task InvokeAsync(HttpContext context, ISseConnectionManager connectionManager)
		{
			if (!_sseHttpUtility.IsAcceptHeaderEventStream(context.Request.Headers["accept"]))
			{
				await _next(context);
				return;
			}

			string? typeName = _sseHttpUtility.GetTypeNameFromRequestPath(context.Request.Path);
			string? validationResult = _sseHttpUtility.ValidateRequest(context.Request, _options, typeName);
			if (validationResult is not null)
			{
				await _sseHttpUtility.RespondBadRequestAsync(context.Response, validationResult);
				return;
			}

			await _sseHttpUtility.SetSseResponseAsync(context);

			var connection = _sseHttpUtility.CreateConnection(context);
			connectionManager.Add(connection, typeName!);
			await connection.CompletedAsync;
		}

		public static SseMiddlewareOptions GetDefaultOptions()
		{
			return new SseMiddlewareOptions
			{
				RootPath = _defaultRootPath,
			};
		}
	}
}