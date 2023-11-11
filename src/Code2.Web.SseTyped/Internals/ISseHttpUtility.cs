using Microsoft.AspNetCore.Http;

namespace Code2.Web.SseTyped.Internals
{
	internal interface ISseHttpUtility
	{
		ISseConnection CreateConnection(HttpContext context, string? clientIdKey);
		string? GetClientIdFromRequest(HttpRequest request, string? clientIdKey);
		string? GetTypeNameFromRequestPath(string requestPath);
		string? ValidateRequest(HttpRequest request, SseMiddlewareOptions options, string? typeName);
	}
}