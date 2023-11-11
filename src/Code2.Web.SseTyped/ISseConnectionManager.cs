namespace Code2.Web.SseTyped
{
	public interface ISseConnectionManager
	{
		void Add(ISseConnection connection, string typeName);
		ISseConnection[] Get(string? typeName, string? clientId);
		void Remove(ISseConnection connection, string typeName);
	}
}