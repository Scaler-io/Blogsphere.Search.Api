using Blogsphere.Search.Api.Models.Enums;

namespace Blogsphere.Search.Api.Models.Core;

public class ApiExceptionResponse : ApiResponse
{
    public ApiExceptionResponse(string errorMessage = null, string stackTrace = "") : 
        base(ErrorCodes.InternalServerError, errorMessage)
    {
        StackTrace = stackTrace;
    }

    public string StackTrace { get; set; }
}
