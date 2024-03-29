using System.Collections.Specialized;
using System.Threading;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public interface ISseConnection
	{
		StringDictionary Properties { get; }
		Task CompletedAsync { get; }
		CancellationToken RequestAborted { get; }

		void Close();
		Task WriteAsync(byte[] data);
	}
}