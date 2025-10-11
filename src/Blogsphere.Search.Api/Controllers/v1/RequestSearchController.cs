
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace Blogsphere.Search.Api.Controllers.v1;

[ApiVersion("1")]
public class RequestSearchController(ILogger logger, ISearchServiceFactory factory) : ApiBaseController(logger, factory)
{

    [HttpPost("{indexName}")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "Search", Summary = "Search document", Description = "Search for data in the specified index")]
    public async Task<IActionResult> Search([FromBody] RequestQuery query, [FromRoute] string indexName)
    {
        Logger.Here().MethodEntered();
        var response = indexName switch
        {
            var name when name.IsApiClusterIndex() => await HandleSearchSummary<ApiClusterSummary>(query, indexName),
            var name when name.IsApiRouteIndex() => await HandleSearchSummary<ApiRouteSummary>(query, indexName),
            _ => BadRequest(new ApiValidationResponse("Index invalid name provided"))
        };

        Logger.Here().MethodExited();
        return response;
    }

    [HttpPost("count/{indexName}")]
    [SwaggerHeader("CorrelationId", Description = "Unique identifier for tracing the request through the system")]
    [SwaggerOperation(OperationId = "SearchCount", Summary = "Count document", Description = "Fetches the total document count from elastic search")]
    public async Task<IActionResult> SearchCount([FromRoute] string indexName, [FromBody] RequestQuery query = null)
    {
        Logger.Here().MethodEntered();
        var result = await ExecuteTypedCountAsync(indexName, query);
        Logger.Here().MethodExited();
        return OkOrFailure(result);
    }
}
