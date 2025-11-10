namespace Blogsphere.Search.Api.Entities.User;

public class ManagementUserSummary
{
    public string Id { get; set; }
    public string EmployeeId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public string JobTitle { get; set; }
    public List<string> Roles { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
