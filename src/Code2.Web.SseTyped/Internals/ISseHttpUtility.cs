using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped.Internals
{
	internal interface ISseHttpUtility
	{
		ISseConnection CreateConnection(HttpContext context);
		string? GetTypeNameFromRequestPath(string requestPath);
		string? ValidateRequest(HttpRequest request, SseMiddlewareOptions options, string? typeName);
		Task RespondBadRequestAsync(HttpResponse response, string validationResult);
		Task SetSseResponseAsync(HttpContext context);
		bool IsAcceptHeaderEventStream(string acceptHeader);
	}
}