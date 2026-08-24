using Serilog.Context;
using System.Diagnostics;

namespace Apex_Website_API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next,ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = context.TraceIdentifier;

            var stopwatch = Stopwatch.StartNew();

            using (LogContext.PushProperty("TraceId", traceId))
            {
                _logger.LogInformation("============================================================");

                _logger.LogInformation("REQUEST START | Method: {Method} | Path: {Path}",context.Request.Method,context.Request.Path);

                try
                {
                    await _next(context);

                    stopwatch.Stop();

                    _logger.LogInformation("RESPONSE | StatusCode: {StatusCode} | Duration: {Duration} ms",context.Response.StatusCode,stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _logger.LogError(ex,"REQUEST FAILED | StatusCode: 500 | Duration: {Duration} ms",stopwatch.ElapsedMilliseconds);
                    throw;
                }

                _logger.LogInformation("REQUEST END");

                _logger.LogInformation("============================================================");
            }
        }
    }
}