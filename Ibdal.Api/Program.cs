using Ibdal.Api;
using Ibdal.Api.Hubs;
using Ibdal.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

KeyGen.Invoke();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/chat");
app.MapHub<NotificationHub>("/notification");

app.MapControllers();

app.Run();