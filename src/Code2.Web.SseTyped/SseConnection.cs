using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public class SseConnection : ISseConnection
	{
		public SseConnection(HttpContext httpContext) : this(httpContext, new StringDictionary())
		{
		}

		public SseConnection(HttpContext httpContext, StringDictionary properties)
		{
			_httpContext = httpContext;
			_httpContext.RequestAborted.Register(() => _tcsCompleted.SetResult(0));
			Properties = properties;
		}

		private readonly HttpContext _httpContext;
		private readonly TaskCompletionSource<int> _tcsCompleted = new TaskCompletionSource<int>();

		public CancellationToken RequestAborted => _httpContext.RequestAborted;

		public StringDictionary Properties { get; private set; }
		public Task CompletedAsync => _tcsCompleted.Task;

		public async Task WriteAsync(byte[] data)
		{
			if (RequestAborted.IsCancellationRequested) return;
			try
			{
				await _httpContext.Response.Body.WriteAsync(data, RequestAborted);
				await _httpContext.Response.Body.FlushAsync(RequestAborted);
			}
			catch (OperationCanceledException)
			{
				return;
			}
		}

		public void Close()
		{
			_httpContext.Abort();
		}
	}
}
