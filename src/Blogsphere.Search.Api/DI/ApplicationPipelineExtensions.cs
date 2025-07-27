using Asp.Versioning.ApiExplorer;
using Blogsphere.Search.Api.Middlewares;
using Blogsphere.Search.Api.Swagger;
using HealthChecks.UI.Client;

namespace Blogsphere.Search.Api.DI;

public static class ApplicationPipelineExtensions
{
    public static WebApplication UseApplicationPipeline(this WebApplication app)
    {
        app.UseSwagger(SwaggerConfiguration.SetupSwaggerOptions);
        app.UseSwaggerUI(options => 
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            SwaggerConfiguration.SetupSwaggerUiOptions(options, provider);
        });

        // Configure security headers
        app.Use(async (context, next) =>
        {
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Append("X-Frame-Options", "DENY");
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
            
            // More permissive CSP for development and to allow Health Check UI and Zipkin dashboard
            var csp = "default-src 'self'; " +
                     "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
                     "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https://cdn.jsdelivr.net; " +
                     "font-src 'self' https://fonts.gstatic.com; " +
                     "img-src 'self' data: https:; " +
                     "connect-src 'self' ws: wss:;";
            
            context.Response.Headers.Append("Content-Security-Policy", csp);
            await next();
        });

        app.UseCors("CorsPolicy");

        app.UseMiddleware<CorrelationHeaderEnricher>();
        app.UseMiddleware<RequestLoggerMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.MapHealthChecks("/health", new()
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });

        app.MapHealthChecksUI(options =>
        {
            options.UIPath = "/dashboard";
        });

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        return app;
    }
}
