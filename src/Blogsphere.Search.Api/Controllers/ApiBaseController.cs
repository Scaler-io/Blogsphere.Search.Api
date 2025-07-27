using Blogsphere.Search.Api.Extensions;
using Blogsphere.Search.Api.Models.Constants;
using Blogsphere.Search.Api.Models.Core;
using Blogsphere.Search.Api.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Blogsphere.Search.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ApiBaseController(ILogger logger) : ControllerBase
{
    protected ILogger Logger { get; set; } = logger;
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
}
