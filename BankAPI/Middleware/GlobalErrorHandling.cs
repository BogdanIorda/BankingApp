using System.Net;
using System.Text.Json;

namespace BankAPI.Middleware
{
    public class GlobalErrorHandling
    {
        private readonly RequestDelegate _next;

        // "next" is the next step in the pipeline (e.g., the Controller)
        public GlobalErrorHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // THE TRY BLOCK: "Try to run the request normally"
            try
            {
                await _next(context); // Let the request go to the Controller
            }
            // THE CATCH BLOCK: "If anything explodes anywhere, catch it here!"
            catch (Exception ex)
            {
                // Call our helper method to send a polite response
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            // 1. Set the response type to JSON (so the frontend doesn't break)
            context.Response.ContentType = "application/json";

            // 2. Set the status code to 500 (Internal Server Error)
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // 3. Create the polite error message
            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error. Please try again later.",

                DetailedError = ex.Message // This will show the throw new Exception message when we test this middleware
            };

            var jsonResponse = JsonSerializer.Serialize(response);

            // 4. Send the JSON back to the user
            return context.Response.WriteAsync(jsonResponse);
        }
    }
}