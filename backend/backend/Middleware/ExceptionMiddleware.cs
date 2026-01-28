using System.Net;
using System.Text.Json;
using backend.Exceptions;

namespace backend.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                _logger.LogInformation("Exception middleware hit");

                await _next(context);
            }
            catch (AppException ex)
            {
                await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(
                    context,
                    (int)HttpStatusCode.InternalServerError,
                    "Something went wrong"
                );
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context,
            int statusCode,
            string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new
            {
                message,
                statusCode
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response)
            );
        }
    }
}