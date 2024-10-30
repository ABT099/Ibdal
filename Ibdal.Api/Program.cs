using Ibdal.Api;

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

app.MapControllers();

app.Run();