namespace Code2.Web.SseTyped
{
	public class SseMiddlewareOptions
	{
		public string RootPath { get; set; } = default!;
		public string[]? AllowedTypeNames { get; set; }
	}
}
