using NSubstitute;
using System.Collections.Specialized;

namespace Code2.Web.SseTyped.Tests
{
	[TestClass]
	public class SseConnectionManagerTests
	{
		[TestMethod]
		public void Add_When_ConnectionAdded_Expect_GetConnectionResult()
		{
			string typeName = "Type1";
			ISseConnection connection = Substitute.For<ISseConnection>();
			SseConnectionManager manager = new SseConnectionManager();

			manager.Add(connection, typeName);

			Assert.AreEqual(1, manager.Get(typeName).Length);
		}

		[TestMethod]
		public void Get_When_FilterSpecified_Expect_ConnectionsAccordingToFilter()
		{
			string typeName = "Type1";
			StringDictionary properties = new StringDictionary() { { "key1", "value1" }, { "key2", "value2" } };
			ISseConnection connection = Substitute.For<ISseConnection>();
			connection.Properties.Returns(properties);
			SseConnectionManager manager = new SseConnectionManager();

			manager.Add(connection, typeName);

			var connections1 = manager.Get(typeName, (p) => p["key1"] == "value1");
			var connections2 = manager.Get(typeName, (p) => p["key2"] == "value1");
			var connections3 = manager.Get(typeName, (p) => p["key3"] == "value3");

			Assert.AreEqual(1, connections1.Length);
			Assert.AreEqual(0, connections2.Length);
			Assert.AreEqual(0, connections3.Length);
		}

		[TestMethod]
		public void Get_When_TypeNotSpecified_Expect_AllConnections()
		{
			int count = 10;
			SseConnectionManager manager = new SseConnectionManager();
			for (int i = 0; i < count; i++)
			{
				manager.Add(Substitute.For<ISseConnection>(), $"Type{i}");
			}

			var connections1 = manager.Get(null);

			Assert.AreEqual(count, connections1.Length);
		}
	}
}