using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Code2.Web.SseTyped.Internals;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public class SseMiddleware
	{
		public SseMiddleware(RequestDelegate next, IOptions<SseMiddlewareOptions> options) 
			: this(next, options.Value) { }

		public SseMiddleware(RequestDelegate next, SseMiddlewareOptions options)
			: this (next, options, new SseHttpUtility()) {}

		internal SseMiddleware(RequestDelegate next, SseMiddlewareOptions options, ISseHttpUtility sseHttpUtility)
		{
			_next = next;
			_options = options;
			_sseHttpUtility = sseHttpUtility;
		}
		

		private readonly RequestDelegate _next;
		private SseMiddlewareOptions _options;
		private readonly ISseHttpUtility _sseHttpUtility;

		private const string _sseHttpRequestMethod = "GET";
		private const string _sseHttpAcceptHeader = "text/event-stream";
		private const string _defaultRootPath = "/sse";
		private const string _defaultClientIdKey = "clientid";

		public async Task InvokeAsync(HttpContext context, ISseConnectionManager connectionManager)
		{
			if (context.Request.Headers["accept"] != _sseHttpAcceptHeader || context.Request.Method != _sseHttpRequestMethod)
			{
				await _next(context);
				return;
			}

			string? typeName = _sseHttpUtility.GetTypeNameFromRequestPath(context.Request.Path);
			string? validationResult = _sseHttpUtility.ValidateRequest(context.Request, _options, typeName);
			if (!(validationResult is null))
			{
				await RespondBadRequest(context.Response, validationResult);
				return;
			}
			
			SetSseResponse(context);

			var connection = _sseHttpUtility.CreateConnection(context, _options.ClientIdKey);
			connectionManager.Add(connection, typeName!);
			await connection.CompletedAsync;
		}

		public static SseMiddlewareOptions GetDefaultOptions()
		{
			return new SseMiddlewareOptions
			{
				ClientIdKey = _defaultClientIdKey,
				RootPath = _defaultRootPath,
			};
		}

		private async Task RespondBadRequest(HttpResponse response, string validationResult)
		{
			response.StatusCode = 400;
			await response.WriteAsync(validationResult);
			await response.Body.FlushAsync();
		}

		private async void SetSseResponse(HttpContext context)
		{
			//context.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();
			context.Response.StatusCode = 200;
			context.Response.Headers["content-type"] = "text/event-stream";
			context.Response.Headers["cache-control"] = "no-cache";
			context.Response.Headers["connection"] = "keep-alive";
			await context.Response.Body.FlushAsync();
		}
	}
}