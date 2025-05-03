using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SafeVaultAPI.Utilities;
using SafeVaultAPI.Service;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace SafeVaultAPI.Extension
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomConfiguration(this IServiceCollection services, IConfigurationBuilder configBuilder, IWebHostEnvironment env)
        {
            configBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtClients = configuration.GetSection("JwtSettings:Clients")
                                          .Get<List<ClientSettings>>() ?? new List<ClientSettings>();

            services.AddSingleton(jwtClients);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            ValidateLifetime = true, // Validate token expiration
                            ValidateIssuerSigningKey = true,
                            ClockSkew = TimeSpan.Zero, // Strict expiration (no default 5-minute buffer) To make sure the token is not valid even with a small delay



                            // Support multiple issuers/audiences/keys for multi-client config
                            ValidIssuers = jwtClients.Select(c => c.Issuer),
                            ValidAudiences = jwtClients.Select(c => c.Audience),
                            
                            // Dynamically resolve the correct signing key
                            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                            {
                                return jwtClients.Select(c =>
                                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(c.Key)));
                            }
                        };

                        options.Events = new JwtBearerEvents
                        {
                            OnChallenge = context =>
                            {
                                var endpoint = context.HttpContext.GetEndpoint();
                                var allowAnonymous = endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null;

                                if (allowAnonymous)
                                    return Task.CompletedTask;

                                context.HandleResponse();
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                context.Response.ContentType = "application/json";

                                var message = context.AuthenticateFailure is SecurityTokenExpiredException
                                    ? "{\"message\":\"Token has expired\"}"
                                    : "{\"message\":\"Unauthorized access\"}";

                                return context.Response.WriteAsync(message);
                            }
                        };
                    });

            // Register JwtTokenGenerator as both concrete and interface
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            return services;
        }
    
        
        // THIS IS TO REMOVE TOKEN OR AUTHORIZATION TO THOSE ENDPOINT TAGGED [AllowAnonymous] or PUBLIC ENDPOINT
        public class RemoveBearerTokenFromAnonymousEndpoints : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var isAnonymous = context.MethodInfo
                    .GetCustomAttributes(typeof(AllowAnonymousAttribute), true)
                    .Any();

                // Remove Authorization header if the endpoint is [AllowAnonymous]
                if (isAnonymous)
                {
                    operation.Parameters = operation.Parameters
                        .Where(p => p.Name != "Authorization") // Remove Authorization parameter
                        .ToList();
                }
            }
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Enter 'Bearer' followed by a space and the JWT token"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });

                // Add the custom operation filter
                options.OperationFilter<RemoveBearerTokenFromAnonymousEndpoints>();
            });

            return services;
        }
    }
}