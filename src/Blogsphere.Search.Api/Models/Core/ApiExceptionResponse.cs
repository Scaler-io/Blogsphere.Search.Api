namespace Blogsphere.Search.Api.Models.Core;

public class ApiExceptionResponse(string errorMessage = null, string stackTrace = "") : ApiResponse(ErrorCodes.InternalServerError, errorMessage)
{
    public string StackTrace { get; set; } = stackTrace;
}
