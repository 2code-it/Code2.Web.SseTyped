using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace Code2.Web.SseTyped
{
	public interface ISseService
	{
		Task Send<T>(T message, Func<StringDictionary, bool>? filter = null) where T : class;
	}
}