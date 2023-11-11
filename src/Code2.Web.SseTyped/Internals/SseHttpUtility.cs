using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

namespace Code2.Web.SseTyped.Internals
{
	internal class SseHttpUtility : ISseHttpUtility
	{
		public ISseConnection CreateConnection(HttpContext context, string? clientIdKey)
			=> new SseConnection(context, GetClientIdFromRequest(context.Request, clientIdKey));
		
		public string? GetTypeNameFromRequestPath(string requestPath)
		{
			int index = requestPath.LastIndexOf('/');
			return index == -1 ? null : requestPath.Substring(index+1);
		}

		public string? GetClientIdFromRequest(HttpRequest request, string? clientIdKey)
		{
			if (clientIdKey is null) return null;
			return request.Query[clientIdKey].FirstOrDefault() ?? request.Cookies[clientIdKey];
		}

		public string? ValidateRequest(HttpRequest request, SseMiddlewareOptions options, string? typeName) 
		{
			if (typeName is null) return "Typename is not defined";
			if (!request.Path.StartsWithSegments(options.RootPath)) return $"Invalid path '{request.Path}'";
			if (options.AllowedTypeNames?.Any(x => x.Equals(typeName, StringComparison.InvariantCultureIgnoreCase)) ?? false)
				return $"Typename '{typeName}' not allowed";

			return null;
		}
	}
}
