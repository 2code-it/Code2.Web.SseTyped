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
using Microsoft.Extensions.Primitives;

namespace Code2.Web.SseTypedTests
{
	[TestClass]
	public class SseHttpUtilityTests
	{
		[TestMethod]
		public void GetTypeNameFromRequestPath_WhenInvalidPath_Expect_Null()
		{
			SseHttpUtility sseHttpUtility = new SseHttpUtility();

			string? typeName = sseHttpUtility.GetTypeNameFromRequestPath("empty");

			Assert.IsNull(typeName);
		}

		[TestMethod]
		public void GetTypeNameFromRequestPath_WhenValidPath_Expect_TypeName()
		{
			string typeName = "Type1";
			SseHttpUtility sseHttpUtility = new SseHttpUtility();

			string? typeNameResult = sseHttpUtility.GetTypeNameFromRequestPath($"/sse/{typeName}");

			Assert.AreEqual(typeName, typeNameResult);
		}

		[TestMethod]
		public void ValidateRequest_When_TypeNameIsNull_Expect_ResultNotNull()
		{
			HttpRequest httpRequest = Substitute.For<HttpRequest>();
			httpRequest.Path.Returns(new PathString(""));
			SseMiddlewareOptions options = new SseMiddlewareOptions();
			string? typeName = null;
			SseHttpUtility sseHttpUtility = new SseHttpUtility();

			string? validationResult = sseHttpUtility.ValidateRequest(httpRequest, options, typeName);

			Assert.IsNotNull(validationResult);
		}

		[TestMethod]
		public void ValidateRequest_When_PathDoesNotStartWithRootPathOption_Expect_ResultNotNull()
		{
			HttpRequest httpRequest = Substitute.For<HttpRequest>();
			httpRequest.Path.Returns(new PathString("/sse"));
			SseMiddlewareOptions options = new SseMiddlewareOptions();
			options.RootPath = "/serve";
			string? typeName = "Type1";
			SseHttpUtility sseHttpUtility = new SseHttpUtility();

			string? validationResult = sseHttpUtility.ValidateRequest(httpRequest, options, typeName);

			Assert.IsNotNull(validationResult);
		}

		[TestMethod]
		public void ValidateRequest_When_TypeNameNotInAllowedTypeNamesOption_Expect_ResultNotNull()
		{
			HttpRequest httpRequest = Substitute.For<HttpRequest>();
			httpRequest.Path.Returns(new PathString("/sse"));
			SseMiddlewareOptions options = new SseMiddlewareOptions();
			options.RootPath = "/sse";
			options.AllowedTypeNames = new [] { "Type2", "Type3" };
			string? typeName = "Type1";
			SseHttpUtility sseHttpUtility = new SseHttpUtility();

			string? validationResult = sseHttpUtility.ValidateRequest(httpRequest, options, typeName);

			Assert.IsNotNull(validationResult);
		}

		[TestMethod]
		public void ValidateRequest_When_VariablesAreAccordingToOptions_Expect_ResultNull()
		{
			HttpRequest httpRequest = Substitute.For<HttpRequest>();
			httpRequest.Path.Returns(new PathString("/sse"));
			SseMiddlewareOptions options = new SseMiddlewareOptions();
			options.RootPath = "/sse";
			options.AllowedTypeNames = new[] { "Type1", "Type2" };
			string? typeName = "Type1";
			SseHttpUtility sseHttpUtility = new SseHttpUtility();

			string? validationResult1 = sseHttpUtility.ValidateRequest(httpRequest, options, typeName);
			options.AllowedTypeNames = null;
			string? validationResult2 = sseHttpUtility.ValidateRequest(httpRequest, options, typeName);

			Assert.IsNull(validationResult1);
			Assert.IsNull(validationResult2);
		}

		[TestMethod]
		public void CreateConnection_When_HasQueryString_Expect_QueryItemAsProperties()
		{
			HttpContext httpContext = Substitute.For<HttpContext>();
			httpContext.Request.Returns(Substitute.For<HttpRequest>());
			Dictionary<string, StringValues> querySource = new Dictionary<string, StringValues>() { { "key1", "value1" }, {"key2", "value2" } };
			QueryCollection queryCollection = new QueryCollection(querySource);
			httpContext.Request.Query.Returns(queryCollection);
			SseHttpUtility sseHttpUtility = new SseHttpUtility();

			ISseConnection connection = sseHttpUtility.CreateConnection(httpContext);

			Assert.AreEqual("value1", connection.Properties["key1"]);
			Assert.AreEqual("value2", connection.Properties["key2"]);
		}
	}
}
