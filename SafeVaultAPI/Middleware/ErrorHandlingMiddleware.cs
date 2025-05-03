
namespace SafeVaultAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);  // Proceed with request processing
            }
            catch (Exception ex)
            {
                // Log error details
                _logger.LogError(ex, "An error occurred while processing the request.");

                // Handle specific errors
                context.Response.ContentType = "application/json";

                if (_env.IsDevelopment())
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new { message = ex.Message, details = ex.StackTrace }));
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("{\"message\":\"Internal Server Error. Please try again later.\"}");
                }
            }
        }
    }
}