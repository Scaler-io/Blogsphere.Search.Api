namespace Blogsphere.Search.Api.Models.Core;

public class Result<T>
{
    public T Data { get; set; }
    public bool IsSuccess { get; set; }
    public ErrorCodes ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public static Result<T> Success(T data)
    {
        return new Result<T> { Data = data, IsSuccess = true };
    }

    public static Result<T> Failure(ErrorCodes errorCode, string errorMessage = null)
    {
        return new Result<T> { ErrorCode = errorCode, ErrorMessage = errorMessage, IsSuccess = false };
    }
}
