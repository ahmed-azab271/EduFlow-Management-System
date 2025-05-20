using APIs.Errors;
using System.Text.Json;

namespace APIs.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate Next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = Next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 500; // == (int)HttpStatusCode.InternalServerError

                var Response = _env.IsDevelopment()
                    ? new ApiExceptionResponse(500, ex.Message) : new ApiExceptionResponse(500);

                var Options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var JsonResponse = JsonSerializer.Serialize(Response, Options);
                await context.Response.WriteAsync(JsonResponse);
            }
        }
    }
}
