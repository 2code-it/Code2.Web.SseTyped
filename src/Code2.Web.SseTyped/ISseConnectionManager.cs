using System.Collections.Generic;

namespace Code2.Web.SseTyped
{
	public interface ISseConnectionManager
	{
		void Add(ISseConnection connection, string typeName);
		ISseConnection[] Get(string? typeName, IDictionary<string, string>? filter);
		void Remove(ISseConnection connection, string typeName);
	}
}