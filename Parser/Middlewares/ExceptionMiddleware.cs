using System.Globalization;
using System.Net;
using System.Text.Json;

namespace Parser.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, 
            ILogger<ExceptionMiddleware> logger, 
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                object errorResponse = _env.IsDevelopment()
                    ? new { StatusCode = (int)HttpStatusCode.InternalServerError, ex.Message, StrackTrack = ex.StackTrace?.ToString() }
                    : new { StatusCode = (int)HttpStatusCode.InternalServerError };

                JsonSerializerOptions options = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                string jsonResponse = JsonSerializer.Serialize(errorResponse, options);

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
