using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Code2.Web.SseTyped
{
	public interface ISseConnectionManager
	{
		void Add(ISseConnection connection, string typeName);
		ISseConnection[] Get(string? typeName, Func<StringDictionary, bool>? filter = null);
		void Remove(ISseConnection connection, string typeName);
	}
}