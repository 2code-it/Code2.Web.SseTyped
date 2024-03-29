using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Code2.Web.SseTyped
{
	public class SseConnectionManager : ISseConnectionManager
	{
		private readonly IDictionary<string, List<ISseConnection>> _connectionListsByTypeName = new Dictionary<string, List<ISseConnection>>();
		private static readonly object _lock = new object();

		public void Add(ISseConnection connection, string typeName)
		{
			lock (_lock)
			{
				if (!_connectionListsByTypeName.ContainsKey(typeName)) _connectionListsByTypeName.Add(typeName, new List<ISseConnection>());
				_connectionListsByTypeName[typeName].Add(connection);
				connection.RequestAborted.Register(() => Remove(connection, typeName));
			}
		}

		public ISseConnection[] Get(string? typeName, Func<StringDictionary, bool>? filter = null)
		{
			lock (_lock)
			{
				IEnumerable<ISseConnection>? connections = true switch
				{
					true when string.IsNullOrEmpty(typeName) => _connectionListsByTypeName.Values.SelectMany(x => x),
					true when _connectionListsByTypeName.ContainsKey(typeName) => _connectionListsByTypeName[typeName],
					_ => null
				};

				if (connections is null) return Array.Empty<ISseConnection>();

				if (filter is not null)
				{
					connections = connections.Where(x => filter(x.Properties));
				}
				return connections.ToArray();
			}
		}

		public void Remove(ISseConnection connection, string typeName)
		{
			lock (_lock)
			{
				if (!_connectionListsByTypeName.ContainsKey(typeName)) throw new ArgumentException($"Collection not found for type {typeName}", nameof(typeName));
				_connectionListsByTypeName[typeName].Remove(connection);
			}
		}
	}
}
