using Asp.Versioning.ApiExplorer;
using HealthChecks.UI.Client;
using Scalar.AspNetCore;

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
            foreach(var description in provider.ApiVersionDescriptions)
            {
                app.MapScalarApiReference($"scalar/{description.GroupName}", options => 
                {
                    SwaggerConfiguration.SetupScalarUiOptions(options, description);
                });
            }
        });

        app.UseCors("CorsPolicy");

        app.UseMiddleware<CorrelationHeaderEnricher>();
        app.UseMiddleware<RequestLoggerMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.MapHealthChecks("/healthcheck", new()
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
