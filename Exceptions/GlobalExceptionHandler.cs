using Microsoft.AspNetCore.Diagnostics;

namespace Apex_Website_API.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,Exception exception,CancellationToken cancellationToken)
        {
            var traceId = httpContext.TraceIdentifier;
            _logger.LogError(exception,"UNHANDLED EXCEPTION | Method: {Method} | Path: {Path}",httpContext.Request.Method,httpContext.Request.Path);

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = "An unexpected error occurred.",
                traceId = traceId
            };

            await httpContext.Response.WriteAsJsonAsync(response,cancellationToken);

            return true;
        }
    }
}