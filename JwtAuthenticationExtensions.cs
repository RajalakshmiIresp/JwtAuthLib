using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace JwtAuthLibrary
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration, ILogger logger)
        {
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

            // Log configuration loading
            logger.LogInformation("Loading JWT settings: {Settings}", jwtSettings);

            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    // Handle authentication failure
                    OnAuthenticationFailed = context =>
                    {
                        logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                        return System.Threading.Tasks.Task.CompletedTask;
                    },

                    // Handle token validation success
                    OnTokenValidated = context =>
                    {
                        logger.LogInformation("Token validated successfully for user: {User}");
                        return System.Threading.Tasks.Task.CompletedTask;
                    },
                };
            });

            return services;
        }
    }
}
