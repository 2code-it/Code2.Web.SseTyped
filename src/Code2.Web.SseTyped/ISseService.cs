using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public interface ISseService
	{
		Task Send<T>(T message, string? clientId = null) where T : class;
	}
}