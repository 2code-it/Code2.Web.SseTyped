namespace Code2.Web.SseTyped
{
	public interface ISerializer
	{
		byte[] SerializeToUtf8Bytes<T>(T obj);
	}
}
