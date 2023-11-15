namespace Code2.Web.SseTyped
{
	public interface ISerializer
	{
		string Serialize<T>(T obj);
	}
}
