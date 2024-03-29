using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped.Internals
{
	internal class SseHttpUtility : ISseHttpUtility
	{

		private const string _acceptHeaderEventStream = "text/event-stream";

		public bool IsAcceptHeaderEventStream(string acceptHeader)
			=> _acceptHeaderEventStream == acceptHeader;

		public ISseConnection CreateConnection(HttpContext context)
		{
			StringDictionary dictionary = new StringDictionary();
			foreach (var item in context.Request.Query)
			{
				dictionary.Add(item.Key, item.Value);
			}
			return new SseConnection(context, dictionary);
		}

		public string? GetTypeNameFromRequestPath(string requestPath)
		{
			int index = requestPath.LastIndexOf('/');
			return index == -1 ? null : requestPath.Substring(index + 1);
		}

		public string? ValidateRequest(HttpRequest request, SseMiddlewareOptions options, string? typeName)
		{
			if (typeName is null) return "Typename is not defined";
			if (!request.Path.StartsWithSegments(options.RootPath)) return $"Invalid path '{request.Path}'";
			if (options.AllowedTypeNames is not null && !options.AllowedTypeNames.Any(x => x.Equals(typeName, StringComparison.InvariantCultureIgnoreCase)))
				return $"Typename '{typeName}' not allowed";

			return null;
		}

		public async Task RespondBadRequestAsync(HttpResponse response, string validationResult)
		{
			response.StatusCode = 400;
			await response.WriteAsync(validationResult);
			await response.Body.FlushAsync();
		}

		public async Task SetSseResponseAsync(HttpContext context)
		{
			context.Response.StatusCode = 200;
			context.Response.Headers["content-type"] = "text/event-stream";
			context.Response.Headers["cache-control"] = "no-cache";
			context.Response.Headers["connection"] = "keep-alive";
			await context.Response.Body.FlushAsync();
		}
	}
}
