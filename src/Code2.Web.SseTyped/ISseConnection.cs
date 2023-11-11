using System.Threading;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public interface ISseConnection
	{
		string? ClientId { get; }
		Task CompletedAsync { get; }
		CancellationToken RequestAborted { get; }

		void Close();
		Task WriteAsync(byte[] data);
	}
}