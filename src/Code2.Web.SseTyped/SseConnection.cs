using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Threading;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public class SseConnection : ISseConnection
	{
		public SseConnection(HttpContext httpContext, string? clientId)
		{
			_httpContext = httpContext;
			ClientId = clientId;

			_httpContext.RequestAborted.Register(() => _tcsCompleted.SetResult(0));
		}

		private readonly HttpContext _httpContext;
		private TaskCompletionSource<int> _tcsCompleted = new TaskCompletionSource<int>();

		public CancellationToken RequestAborted => _httpContext.RequestAborted;
		public string? ClientId { get; private set; }
		public Task CompletedAsync => _tcsCompleted.Task;

		public async Task WriteAsync(byte[] data)
		{
			if (RequestAborted.IsCancellationRequested) return;
			await _httpContext.Response.Body.WriteAsync(data, RequestAborted);
			await _httpContext.Response.Body.FlushAsync(RequestAborted);
		}

		public void Close()
		{
			_httpContext.Abort();
		}
	}
}
