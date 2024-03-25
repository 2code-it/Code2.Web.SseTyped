using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		private const string _dataField = "data: ";
		private const char _newLine = '\n';

		public async Task Send<T>(T message, IDictionary<string, string>? filter = null) where T : class
		{
			string typeName = typeof(T).Name;
			StringBuilder sb = new StringBuilder();

			ISseConnection[] connections = _sseConnectionManager.Get(typeName, filter);
			if (connections.Length == 0) return;

			sb.Append(_dataField).Append(_serializer.Serialize(message));
			sb.Append(_newLine).Append(_newLine);

			byte[] messageBytes = Encoding.UTF8.GetBytes(sb.ToString());
			var tasks = connections.AsParallel().Select(async x => await x.WriteAsync(messageBytes)).ToArray();
			await Task.WhenAll(tasks);
		}

	}
}
