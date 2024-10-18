using Ibdal.Api;
using Identity.Mongo;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongoDb<AppDbContext>(
    connectionString: builder.Configuration["MongoDb:ConnectionString"]!,
    databaseName: builder.Configuration["MongoDb:DatabaseName"]!);

builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddMongoStores()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

app.Run();