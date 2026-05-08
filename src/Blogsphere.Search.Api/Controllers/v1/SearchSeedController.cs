
using Asp.Versioning;
using Blogsphere.Search.Api.Entities.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace Blogsphere.Search.Api.Controllers.v1;

[ApiVersion("1")]
public class SearchSeedController(
    ILogger logger,
    ISearchServiceFactory factory,
    IOptions<ElasticSearchOption> elasticOptions) : ApiBaseController(logger, factory)
{
    private readonly ElasticSearchOption _elasticSearchOption = elasticOptions.Value;

    [HttpPost("seed/api-clusters")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(Summary = "Seed API Clusters", Description = "Performs re-index and seeds API Clusters ")]
    // 200
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    // 400
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiValidationResponse   ))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    // 500
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiExceptionResponse))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedData()
    {
        Logger.Here().MethodEntered();
        var searchService = Factory.Create<ApiClusterSummary>();
        var result = await searchService.SearchReIndex(_elasticSearchOption.ApiClusterIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("seed/api-routes")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(Summary = "Seed API Routes", Description = "Performs re-index and seeds API Routes ")]
    // 200
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    // 400
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiValidationResponse   ))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    // 500
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiExceptionResponse))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedApiRoutes()
    {
        Logger.Here().MethodEntered();
        var searchService = Factory.Create<ApiRouteSummary>();
        var result = await searchService.SearchReIndex(_elasticSearchOption.ApiRouteIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("seed/management-users")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(Summary = "Seed Management Users", Description = "Performs re-index and seeds Management Users ")]
    // 200
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    // 400
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiValidationResponse   ))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    // 500
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiExceptionResponse))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedManagementUsers()
    {
        Logger.Here().MethodEntered();
        var searchService = Factory.Create<ManagementUserSummary>();
        var result = await searchService.SearchReIndex(_elasticSearchOption.ManagementUserIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }

    [HttpPost("seed/app-users")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(Summary = "Seed App Users", Description = "Performs re-index and seeds App Users ")]
    // 200
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    // 400
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request", typeof(ApiValidationResponse   ))]
    [ProducesResponseType(typeof(ApiValidationResponse), StatusCodes.Status400BadRequest)]
    // 500
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error", typeof(ApiExceptionResponse))]
    [ProducesResponseType(typeof(ApiExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SeedAppUsers()
    {
        Logger.Here().MethodEntered();
        var searchService = Factory.Create<AppUserSummary>();
        var result = await searchService.SearchReIndex(_elasticSearchOption.AppUserIndex);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }
}
