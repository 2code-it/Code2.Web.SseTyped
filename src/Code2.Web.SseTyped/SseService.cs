using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Code2.Web.SseTyped
{
	public class SseService : ISseService
	{
		public SseService(ISseConnectionManager sseConnectionManager) : this(sseConnectionManager, new JsonSerializer()) { }
		public SseService(ISseConnectionManager sseConnectionManager, ISerializer serializer)
		{
			_sseConnectionManager = sseConnectionManager;
			_serializer = serializer;
		}

		private readonly ISseConnectionManager _sseConnectionManager;
		private readonly ISerializer _serializer;

		private readonly static byte[] _messagePrefixBytes = Encoding.UTF8.GetBytes("data: ");
		private readonly static byte[] _messageSuffixBytes = Encoding.UTF8.GetBytes("\n\n");

		public async Task Send<T>(T message, string? clientId = null) where T : class
		{
			string typeName = typeof(T).Name;
			ISseConnection[] connections = _sseConnectionManager.Get(typeName, clientId);
			if (connections.Length == 0) return;

			byte[] messageBytes = ArrayConcat(_messagePrefixBytes, _serializer.SerializeToUtf8Bytes(message), _messageSuffixBytes);
			var tasks = connections.AsParallel().Select(async x => await x.WriteAsync(messageBytes)).ToArray();
			await Task.WhenAll(tasks);
		}

		private byte[] ArrayConcat(byte[] data1, byte[] data2, byte[] data3)
		{
			byte[] result = new byte[data1.Length + data2.Length + data3.Length];

			Array.Copy(data1, result, data1.Length);
			Array.Copy(data2, 0, result, data1.Length, data2.Length);
			Array.Copy(data3, 0, result, data1.Length + data2.Length, data3.Length);
			return result;
		}
	}
}
