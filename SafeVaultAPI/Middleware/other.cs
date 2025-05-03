// public async Task InvokeAsync(HttpContext context)
// {
//     try
//     {
//         await _next(context);

//         // Handle 404 (Not Found)
//         if (context.Response.StatusCode == StatusCodes.Status404NotFound)
//         {
//             context.Response.ContentType = "application/json";
//             await context.Response.WriteAsync("{\"message\":\"Resource not found\"}");
//         }
//     }
//     catch (Exception ex)
//     {
//         context.Response.ContentType = "application/json";
//         context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//         await context.Response.WriteAsync("{\"message\":\"Internal Server Error. Please try again later.\"}");
//     }
// }