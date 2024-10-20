using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public class SseConnection : ISseConnection
	{
		public SseConnection(HttpContext httpContext) : this(httpContext, GetPropertiesFromQuery(httpContext.Request.Query))
		{
		}

		public SseConnection(HttpContext httpContext, StringDictionary properties)
		{
			_httpContext = httpContext;
			_requestCancellationToken = _httpContext.RequestAborted;
			Properties = properties;
		}

		private readonly HttpContext _httpContext;
		private readonly CancellationToken _requestCancellationToken;
		private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

		public CancellationToken RequestAborted => _requestCancellationToken;
		public StringDictionary Properties { get; private set; }

		public async Task WriteAsync(byte[] data)
		{
			await _semaphore.WaitAsync();
			try
			{
				if (!RequestAborted.IsCancellationRequested) await _httpContext.Response.Body.WriteAsync(data, 0, data.Length, RequestAborted);
				if (!RequestAborted.IsCancellationRequested) await _httpContext.Response.Body.FlushAsync(RequestAborted);
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				_semaphore.Release();
			}
		}

		public void Close()
		{
			_httpContext.Abort();
		}

		private static StringDictionary GetPropertiesFromQuery(IQueryCollection query)
		{
			StringDictionary dictionary = new StringDictionary();
			foreach (var item in query)
			{
				dictionary.Add(item.Key, item.Value.ToString());
			}
			return dictionary;
		}
	}
}
