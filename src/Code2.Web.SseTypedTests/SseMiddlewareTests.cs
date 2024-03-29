using Microsoft.VisualStudio.TestTools.UnitTesting;
using Code2.Web.SseTyped;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Code2.Web.SseTyped.Internals;
using Microsoft.AspNetCore.Http;
using NSubstitute.Core;

namespace Code2.Web.SseTyped.Tests
{
	[TestClass]
	public class SseMiddlewareTests
	{
		[TestMethod]
		public void InvokeAsync_When_AcceptHeaderIsNotEventStream_Expect_NextInvoke()
		{
			var sseHttpUtility = Substitute.For<ISseHttpUtility>();
			sseHttpUtility.IsAcceptHeaderEventStream(Arg.Any<string>()).Returns(false);
			var next = Substitute.For<RequestDelegate>();
			var options = Substitute.For<SseMiddlewareOptions>();
			var httpContext = Substitute.For<HttpContext>();
			var connectionManager = Substitute.For<ISseConnectionManager>();
			SseMiddleware middleWare = new SseMiddleware(next, options, sseHttpUtility);

			middleWare.InvokeAsync(httpContext, connectionManager).Wait();

			next.Received(1).Invoke(httpContext);
		}

		[TestMethod]
		public void InvokeAsync_When_IsValidSseRequest_Expect_SseResponseSet()
		{
			var sseHttpUtility = Substitute.For<ISseHttpUtility>();
			sseHttpUtility.IsAcceptHeaderEventStream(Arg.Any<string>()).Returns(true);
			sseHttpUtility.GetTypeNameFromRequestPath(Arg.Any<string>()).Returns("Type1");
			string? result = null;
			sseHttpUtility.ValidateRequest(Arg.Any<HttpRequest>(), Arg.Any<SseMiddlewareOptions>(), Arg.Any<string>()).Returns(result);
			var next = Substitute.For<RequestDelegate>();
			var options = Substitute.For<SseMiddlewareOptions>();
			var httpContext = Substitute.For<HttpContext>();
			httpContext.Request.Returns(Substitute.For<HttpRequest>());
			var connectionManager = Substitute.For<ISseConnectionManager>();
			SseMiddleware middleWare = new SseMiddleware(next, options, sseHttpUtility);

			middleWare.InvokeAsync(httpContext, connectionManager).Wait();

			sseHttpUtility.Received(1).SetSseResponseAsync(httpContext);
		}

		[TestMethod]
		public void InvokeAsync_When_IsInvalidSseRequest_Expect_BadRequestResponseSet()
		{
			var sseHttpUtility = Substitute.For<ISseHttpUtility>();
			sseHttpUtility.IsAcceptHeaderEventStream(Arg.Any<string>()).Returns(true);
			sseHttpUtility.GetTypeNameFromRequestPath(Arg.Any<string>()).Returns("Type1");
			string? result = "invalid";
			sseHttpUtility.ValidateRequest(Arg.Any<HttpRequest>(), Arg.Any<SseMiddlewareOptions>(), Arg.Any<string>()).Returns(result);
			var next = Substitute.For<RequestDelegate>();
			var options = Substitute.For<SseMiddlewareOptions>();
			var httpContext = Substitute.For<HttpContext>();
			httpContext.Request.Returns(Substitute.For<HttpRequest>());
			httpContext.Response.Returns(Substitute.For<HttpResponse>());
			var connectionManager = Substitute.For<ISseConnectionManager>();
			SseMiddleware middleWare = new SseMiddleware(next, options, sseHttpUtility);

			middleWare.InvokeAsync(httpContext, connectionManager).Wait();

			sseHttpUtility.Received(1).RespondBadRequestAsync(Arg.Any<HttpResponse>(), Arg.Any<string>());
			connectionManager.Received(1).Add(Arg.Any<ISseConnection>(), Arg.Any<string>());
		}
	}
}