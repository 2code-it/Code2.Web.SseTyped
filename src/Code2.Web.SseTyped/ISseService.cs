using System.Collections.Generic;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public interface ISseService
	{
		Task Send<T>(T message, IDictionary<string, string>? filter = null) where T : class;
	}
}