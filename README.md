# Code2.Web.SseTyped
AspNet server sent events tools to emit message types mapped to the request path 


## Options
**RootPath**, (optional) Root path for sse request, default: "/sse"  
**ClientIdKey**, (optional) Key name of the client-id which can be supplied by query or cookie, default: "clientid"  
**AllowedTypeNames**, (optional) Array of allowed type names to filter


## Example setup
```
using Code2.Web.SseTyped;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddSseTyped();

var app = builder.Build();

app.UseCors(o => o.AllowAnyHeader().AllowAnyOrigin().WithMethods("GET"));
app.UseSseTyped();

ISseService sseService = app.Services.GetRequiredService<ISseService>();
Task.Run(() => SendMessage(sseService, 10)).Wait(0);

app.Run();

async void SendMessage(ISseService sseService, int amount)
{
	Message message = new Message { Text = "The message" };
	await sseService.Send(message);
	await Task.Delay(1000);
	if (amount > 0)
	{
		SendMessage(sseService, amount - 1);
	}
}

public class Message
{
	public string? Text { get; set; }
}
```

## Example client 
```
<html>
<body>
<script>
	const eventSource = new EventSource("http://localhost:1099/sse/Message");
	eventSource.onmessage = (e) => {
		const message = JSON.parse(e.data);
		console.log(message);
	}
</script>
</body>
</html>
```