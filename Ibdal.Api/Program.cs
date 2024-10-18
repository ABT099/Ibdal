using Ibdal.Api;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

var app = builder.Build();

app.MapControllers();

app.Run();