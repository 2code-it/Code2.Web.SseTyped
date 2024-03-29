# Code2.Web.SseTyped
AspNet server sent events tools to emit message types mapped to the request path 

## Options
**RootPath**, (optional) Root path for sse request, default: "/sse"  
**AllowedTypeNames**, (optional) Array of allowed type names to filter

## Sample app
Sample chat app is available at https://github.com/2code-it/Web1.Sse.Chat  
(webApi server with vue/vite client)

## Client identification
Clients can be identified with the url opening the EventSource using a querystring,
all query items are added as a connection property which can be used to filter target
connections when sending a message.  

user 1 uses url: /sse/Message?userId=1&groupId=1  
user 2 uses url: /sse/Message?userId=2&groupId=1  

both users can be reached using
```
sseService.Send(message);
//or
sseService.Send(message, p => p["groupId"]=="1");
```

user 1 only
```
sseService.Send(message, p => p["userId"]=="1");
```


## Example setup
```
using Code2.Web.SseTyped;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddSseTyped();

var app = builder.Build();

app.UseCors(o => o.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
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