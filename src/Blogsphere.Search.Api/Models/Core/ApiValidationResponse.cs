namespace Blogsphere.Search.Api.Models.Core;

public class ApiValidationResponse : ApiResponse
{
   public ApiValidationResponse(string errorMessage = null)
        : base(ErrorCodes.BadRequest)
    {
        ErrorMessage = !string.IsNullOrEmpty(errorMessage) ? errorMessage : GetDefaultMessage(Code);
    }

    public List<FieldLevelError> Errors { get; set; }

    protected override string GetDefaultMessage(ErrorCodes code)
    {
        return "Invalid data provided";
    } 
}
