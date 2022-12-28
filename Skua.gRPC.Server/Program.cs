using Skua.gRPC.Server;
using Skua.gRPC.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<ScriptContainer>();
builder.Services.AddSingleton<SyncManager>();

var app = builder.Build();

app.MapGrpcService<ScriptService>();
app.MapGrpcService<SynchronizerService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

app.Run();
