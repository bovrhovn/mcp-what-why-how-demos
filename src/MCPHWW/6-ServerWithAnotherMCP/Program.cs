var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services
    .AddMcpServer()
    .WithHttpTransport(o => o.Stateless = true)
    .WithToolsFromAssembly();

builder.Services.AddHttpClient();

var app = builder.Build();

app.MapMcp("/mcp");
app.Run();