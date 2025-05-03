using SafeVaultAPI.Repository;
using SafeVaultAPI.Service;
using SafeVaultAPI.Extension;
using SafeVaultAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add OpenAPI support (Swagger)
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Extension
builder.Services.AddCustomConfiguration(builder.Configuration, builder.Environment); // Load configuration
builder.Services.AddJwtAuthentication(builder.Configuration); // JWT Auth setup
builder.Services.AddCustomSwagger(); // Remove Token since by default Swagger 

// Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserPolicy", policy =>
        policy.RequireRole("User")  // This checks if the user has the "User" role
            //.RequireClaim("CustomClaim", "ExpectedValue")  // Example of a custom claim requirement
    );

    // Additional policies
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin")  // Check if the user has the "Admin" role
    );

    options.AddPolicy("ClientPolicy", policy =>
        policy.RequireRole("Client")  // Check if the user has the "Client" role
    );

    options.AddPolicy("UserClientAndAdminPolicy", policy =>
    {
         policy.RequireRole("Admin", "User", "Client");
    });

    options.AddPolicy("UserAndClientPolicy", policy =>
    {
        policy.RequireRole("User");
        policy.RequireRole("Client");
    });

});

// Add OpenAPI
builder.Services.AddEndpointsApiExplorer();

// Cache Memory or IMemory
builder.Services.AddMemoryCache(); // Add caching support

// Register your dependencies   
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IService, Service>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<ILoginRepository, LoginRepository>();


// Add CORS policy for Front-End Application for SafeVaultClient
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();

// Seed roles (this will populate the in-memory store with roles)
// Call this to ensure roles are seeded
DataSeeder.SeedRoles();  

//-------------------------------------------------------------------------------------------------------------------
// User Roles here using a scoped service provider
// using (var scope = app.Services.CreateScope())
// {
//     var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
//     await DataSeeder.SeedRolesAsync(roleManager);
// }
//--------------------------------------------------------------------------------------------------------------------

// Customize Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// JWT Token authentication and authorization
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Your existing 403 handling middleware (custom 403 response)
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
    {
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"message\":\"Forbidden: You do not have permission to access this resource.\"}");
    }
});

// Configure the HTTP request pipeline or Enable Swagger in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Enable Swagger
    app.UseSwaggerUI(); // Swagger UI for easier route inspection
}

// Enable CORS for frontend. This is what SafeVaultClient communicate to SafeVaultAPI 
app.UseCors("AllowFrontend");

// Map controllers
app.MapControllers();
app.Run();
