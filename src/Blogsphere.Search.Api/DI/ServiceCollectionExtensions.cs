using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Blogsphere.Search.Api.Configurations;
using Blogsphere.Search.Api.Middlewares;
using Blogsphere.Search.Api.Models.Core;
using Blogsphere.Search.Api.Models.Enums;
using Blogsphere.Search.Api.Swagger;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Swashbuckle.AspNetCore.Filters;

namespace Blogsphere.Search.Api.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment)
    {
        services.AddControllers()
        .AddNewtonsoftJson(config => {
            config.SerializerSettings.ContractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
            config.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.SerializerSettings.Converters.Add(new StringEnumConverter());
        });

        services.AddSingleton(new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            NullValueHandling = NullValueHandling.Ignore,
            Converters = [new StringEnumConverter()]
        });

        services.AddEndpointsApiExplorer();
        services.AddApiVersioning(options => 
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
        }).AddApiExplorer(options => 
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Swagger
        var apiName = SwaggerConfiguration.ExtractApiNameFromEnvironmentVariable();
        var apiDescription = configuration["ApiDescription"];
        var apiHost = configuration["ApiOriginHost"];
        var swaggerConfiguration = new SwaggerConfiguration(apiName, apiDescription, apiHost, isDevelopment);

        services.AddSwaggerExamplesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
        .AddSwaggerExamples();

        services.AddSwaggerGen(options => 
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();
            swaggerConfiguration.SetupSwaggerGenOptions(options, provider);
        });

        // Healthcheck
        services.AddHealthChecks();
        services.AddHealthChecksUI(options => 
        {
            options.AddHealthCheckEndpoint("Blogsphere Search API Health", "/health");
        }).AddInMemoryStorage();

        // Middleware
        services.AddScoped<CorrelationHeaderEnricher>();
        services.AddScoped<RequestLoggerMiddleware>();
        services.AddSingleton<GlobalExceptionMiddleware>();

        services.AddHttpContextAccessor();

        // Automapper
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // Fluent Validation
        services.AddFluentValidationAutoValidation();
        services.AddFluentValidationClientsideAdapters();


        // Cors
        services.AddCors(options => 
        {
            options.AddPolicy("CorsPolicy", builder => 
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
        });

        
        // Masstransit
        services.AddMassTransit(config => 
        {
            config.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));
            // add consumer

            config.UsingRabbitMq((context, cfg) => 
            {
                var eventBus = configuration.GetSection(EventBusOption.OptionName).Get<EventBusOption>();
                cfg.Host(eventBus.Host, eventBus.VirtualHost, host => 
                {
                    host.Username(eventBus.Username);
                    host.Password(eventBus.Password);
                });
                cfg.UseMessageRetry(x => x.Interval(3, 3000));
                cfg.ConfigureEndpoints(context);
            });
        });

        // open telemetry
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("blogsphere.search.api"))
            .WithTracing(tracing => 
            {
                tracing.AddSource("Blogsphere.Search.Api")
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddMassTransitInstrumentation()
                .AddZipkinExporter(options => 
                {
                    options.Endpoint = new Uri(configuration["Zipkin:Url"]);
                });
            });

        services.Configure<ApiBehaviorOptions>(options => 
        {
            options.InvalidModelStateResponseFactory = HandleFrameworkValidationFailure();
        });

        return services;
    }

    private static Func<ActionContext, IActionResult> HandleFrameworkValidationFailure()
    {
        return context =>  
        {
            var errors = context.ModelState
            .Where(m => m.Value.Errors.Count > 0)
            .ToList();

            ApiValidationResponse validationError = new()
            {
                Errors = []
            };

            foreach (var error in errors)
            {
                FieldLevelError fieldLevelError = new()
                {
                    Code = ErrorCodes.BadRequest.ToString(),
                    Field = error.Key,
                    Message = error.Value?.Errors?.First().ErrorMessage
                };
                validationError.Errors.Add(fieldLevelError);
            }

            return new BadRequestObjectResult(validationError);
        };
    }
}
