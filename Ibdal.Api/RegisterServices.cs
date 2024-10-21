using FluentValidation.AspNetCore;
using Identity.Mongo;
using Microsoft.AspNetCore.Identity;

namespace Ibdal.Api;

public static class RegisterServices
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddFluentValidationAutoValidation();
        
        builder.Services.AddMongoDb<AppDbContext>(
            connectionString: builder.Configuration["MongoDb:ConnectionString"]!,
            databaseName: builder.Configuration["MongoDb:DatabaseName"]!);

        builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddMongoStores()
            .AddDefaultTokenProviders();
    }
}