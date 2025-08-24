using Microsoft.AspNetCore.Mvc;

namespace Blogsphere.Search.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}")]
public class ApiBaseController(ILogger logger, ISearchServiceFactory factory) : ControllerBase
{
    protected ILogger Logger { get; set; } = logger;
    protected ISearchServiceFactory Factory { get; set; } = factory;
    protected RequestInformation RequestInformation => new(){
        CorrelationId = GetOrGenerateCorrelationId()
    };

    private string GetOrGenerateCorrelationId() => Request?.GetRequestHeaderOrDefault("CorrelationId", $"GEN-{Guid.NewGuid()}");

    protected IActionResult OkOrFailure<T>(Result<T> result)
    {
        if(result == null) return NotFound(new ApiResponse(ErrorCodes.NotFound));
        if(result.IsSuccess && result.Data == null) return NotFound(new ApiResponse(ErrorCodes.NotFound));
        if(result.IsSuccess && result.Data != null) return Ok(result.Data);

        return result.ErrorCode switch
        {
            ErrorCodes.BadRequest => BadRequest(new ApiValidationResponse(result.ErrorMessage)),
            ErrorCodes.NotFound => NotFound(new ApiResponse(ErrorCodes.NotFound, result.ErrorMessage)),
            ErrorCodes.Unauthorized => Unauthorized(new ApiResponse(ErrorCodes.Unauthorized, result.ErrorMessage)),
            ErrorCodes.OperationFailed => BadRequest(new ApiResponse(ErrorCodes.OperationFailed, result.ErrorMessage)),
            ErrorCodes.InternalServerError => StatusCode(500, new ApiExceptionResponse(result.ErrorMessage)),
            ErrorCodes.NotAllowed => Unauthorized(new ApiResponse(ErrorCodes.NotAllowed, result.ErrorMessage)),
            _ => BadRequest(new ApiResponse(ErrorCodes.BadRequest, ErrorMessages.BadRequest))
        };
    }

    protected async Task<IActionResult> HandleSearchSummary<T>(RequestQuery query, string indexName) where T : class
    {
        var result = await ExecuteSearchAsync<T>(query, indexName);
        return OkOrFailure(result);
    }

    protected async Task<Result<Pagination<T>>> ExecuteSearchAsync<T>(RequestQuery query, string indexName) where T : class
    {
        var service = Factory.CreatePaginatedService<T>();
        return await service.GetPaginatedData(query, RequestInformation.CorrelationId, indexName);
    }

    protected async Task<Result<long>> ExecuteTypedCountAsync(string indexName, RequestQuery query)
    {
        if (indexName.IsApiClusterIndex())
            return await ExecuteCountAsync<ApiClusterSummary>(indexName, query);
        if (indexName.IsApiRouteIndex())
            return await ExecuteCountAsync<ApiRouteSummary>(indexName, query);

        return Result<long>.Failure(Models.Enums.ErrorCodes.BadRequest, "Invalid index name provided");
    }

    protected async Task<Result<long>> ExecuteCountAsync<T>(string indexName, RequestQuery query) where T : class
    {
        var service = Factory.CreatePaginatedService<T>();
        var result = await service.GetCount(RequestInformation.CorrelationId, indexName, query);
        return result;
    }
}
