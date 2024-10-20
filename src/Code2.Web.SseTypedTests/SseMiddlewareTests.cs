using Code2.Web.SseTyped.Internals;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Threading;

namespace Code2.Web.SseTyped.Tests
{
	[TestClass]
	public class SseMiddlewareTests
	{
		private HttpContext _httpContext = default!;
		private ISseHttpUtility _sseHttpUtility = default!;
		private RequestDelegate _next = default!;
		private SseMiddlewareOptions _options = default!;
		private ISseConnectionManager _connectionManager = default!;

		[TestInitialize]
		public void ResetDependencies()
		{
			_sseHttpUtility = Substitute.For<ISseHttpUtility>();
			_next = Substitute.For<RequestDelegate>();
			_options = Substitute.For<SseMiddlewareOptions>();
			_httpContext = Substitute.For<HttpContext>();
			_httpContext.Request.Returns(Substitute.For<HttpRequest>());
			_httpContext.Response.Returns(Substitute.For<HttpResponse>());
			_connectionManager = Substitute.For<ISseConnectionManager>();
		}


		[TestMethod]
		public void InvokeAsync_When_AcceptHeaderIsNotEventStream_Expect_NextInvoke()
		{
			_sseHttpUtility.IsAcceptHeaderEventStream(Arg.Any<string>()).Returns(false);
			SseMiddleware middleWare = new SseMiddleware(_next, _options, _sseHttpUtility);

			middleWare.InvokeAsync(_httpContext, _connectionManager).Wait();

			_next.Received(1).Invoke(_httpContext);
		}

		[TestMethod]
		public void InvokeAsync_When_IsValidSseRequest_Expect_SseResponseSet()
		{
			_sseHttpUtility.IsAcceptHeaderEventStream(Arg.Any<string>()).Returns(true);
			_sseHttpUtility.GetTypeNameFromRequestPath(Arg.Any<string>()).Returns("Type1");
			string? result = null;
			_sseHttpUtility.ValidateRequest(Arg.Any<HttpRequest>(), Arg.Any<SseMiddlewareOptions>(), Arg.Any<string>()).Returns(result);
			var connection = Substitute.For<ISseConnection>();
			var cts = new CancellationTokenSource(10);
			connection.RequestAborted.Returns(cts.Token);
			_sseHttpUtility.CreateConnection(Arg.Any<HttpContext>()).Returns(connection);

			SseMiddleware middleWare = new SseMiddleware(_next, _options, _sseHttpUtility);

			middleWare.InvokeAsync(_httpContext, _connectionManager).Wait();

			_sseHttpUtility.Received(1).SetSseResponseAsync(_httpContext);
		}

		[TestMethod]
		public void InvokeAsync_When_IsInvalidSseRequest_Expect_BadRequestResponseSet()
		{
			_sseHttpUtility.IsAcceptHeaderEventStream(Arg.Any<string>()).Returns(true);
			_sseHttpUtility.GetTypeNameFromRequestPath(Arg.Any<string>()).Returns("Type1");
			string? result = "invalid";
			_sseHttpUtility.ValidateRequest(Arg.Any<HttpRequest>(), Arg.Any<SseMiddlewareOptions>(), Arg.Any<string>()).Returns(result);

			SseMiddleware middleWare = new SseMiddleware(_next, _options, _sseHttpUtility);

			middleWare.InvokeAsync(_httpContext, _connectionManager).Wait();

			_sseHttpUtility.Received(1).RespondBadRequestAsync(Arg.Any<HttpResponse>(), Arg.Any<string>());
			_connectionManager.Received(0).Add(Arg.Any<ISseConnection>(), Arg.Any<string>());
		}
	}
}