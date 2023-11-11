using System.Text.Json;

namespace Code2.Web.SseTyped
{
	public class JsonSerializer : ISerializer
	{
		private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

		public byte[] SerializeToUtf8Bytes<T>(T obj)
		{
			return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj, jsonOptions);
		}
	}
}
