using FluentValidation.AspNetCore;
using Ibdal.Api.Services;
using Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

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

        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddMongoStores()
            .AddDefaultTokenProviders();
        
        builder.Services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "jwt";
                o.DefaultChallengeScheme = "jwt";
            })
            .AddJwtBearer("jwt", o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        var authorizationHeader = ctx.Request.Headers.Authorization.ToString();
                        if (string.IsNullOrWhiteSpace(authorizationHeader) || 
                            !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        {
                            return Task.CompletedTask;
                        }

                        var token = authorizationHeader["Bearer ".Length..].Trim();

                        if (string.IsNullOrWhiteSpace(token))
                        {
                            return Task.CompletedTask;
                        };
                            
                        var hashedToken = HashToken.Invoke(token);
                        if (Constants.BlackList.Contains(hashedToken))
                        {
                            ctx.Fail("This token has been invalidated.");
                            return Task.CompletedTask;
                        }
                        
                        ctx.Token = token;
                        return Task.CompletedTask;
                    }
                };

                o.Configuration = new OpenIdConnectConfiguration
                {
                    SigningKeys =
                    {
                        new RsaSecurityKey(Constants.Keys.RsaKey)
                    }
                };

                o.MapInboundClaims = false;
            });

        builder.Services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("jwt")
                .Build())
            .AddPolicy(Constants.Policies.Admin, policy => policy.RequireClaim(Constants.Claims.Role ,Constants.Roles.Admin))
            .AddPolicy(Constants.Policies.Station, policy => policy.RequireClaim(Constants.Claims.Role ,Constants.Roles.Station))
            .AddPolicy(Constants.Policies.Customer, policy => policy.RequireClaim(Constants.Claims.Role ,Constants.Roles.Customer));

        builder.Services.AddSingleton<AuthService>();

    }
}