
using System.Net.Mime;
using Blogsphere.Search.Api.Extensions;
using Blogsphere.Search.Api.Models.Core;
using Blogsphere.Search.Api.Models.Enums;
using Newtonsoft.Json;

namespace Blogsphere.Search.Api.Middlewares;

public class GlobalExceptionMiddleware(ILogger logger, IWebHostEnvironment environment, JsonSerializerSettings jsonSettings) : IMiddleware
{
    private readonly ILogger _logger = logger;
    private readonly IWebHostEnvironment _environment = environment; 
    private readonly JsonSerializerSettings _jsonSettings = jsonSettings;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleGlobalException(context, ex);
        }
    }

    private async Task HandleGlobalException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        var response = _environment.IsDevelopment()
                        ? new ApiExceptionResponse(ex.Message, ex.StackTrace)
                        : new ApiExceptionResponse(ex.Message);

        var jsonResponse = JsonConvert.SerializeObject(response, _jsonSettings);
        _logger.Here().Error("{@InternalServerError} - {@response}", ErrorCodes.InternalServerError, jsonResponse);
        await context.Response.WriteAsync(jsonResponse);
    }
}
