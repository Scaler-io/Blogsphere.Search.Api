using Newtonsoft.Json;

namespace Blogsphere.Search.Api.Models.Contracts.User.ManagementUser;

public class ManagementUser
{
    public string Id { get; set; }
    public string EmployeeId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public string JobTitle { get; set; }
    public List<RoleDetails> Roles { get; set; }
    public bool IsActive { get; set; }
    
    [JsonProperty("metadata")]
    public ManagementUserMetadata Metadata { get; set; }
    
}

public class ManagementUserMetadata
{
    [JsonProperty("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonProperty("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

public class RoleDetails
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsSystemRole { get; set; }
}
