using Serilog.Context;

namespace Blogsphere.Search.Api.Middlewares;

public class CorrelationHeaderEnricher : IMiddleware
{
   private const string CorrelationIdLogPropertyName = "CorrelationId";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var correlationId = GetOrGenerateCorrelationId(context);
        using (LogContext.PushProperty("ThreadId", Environment.CurrentManagedThreadId))
        {
            LogContext.PushProperty(CorrelationIdLogPropertyName, correlationId);
            context.Request.Headers.Append(CorrelationIdLogPropertyName, correlationId);
            await next(context);
        }
    }

    private static string GetOrGenerateCorrelationId(HttpContext context) => context.Request
        .GetRequestHeaderOrDefault("CorrelationId", $"GEN-{Guid.NewGuid()}"); 
}
