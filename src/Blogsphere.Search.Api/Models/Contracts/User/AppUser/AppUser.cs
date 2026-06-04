using Newtonsoft.Json;

namespace Blogsphere.Search.Api.Models.Contracts.User.AppUser;

public class AppUser
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List<RoleDetails> Roles { get; set; }
    public bool IsActive { get; set; }
    public string ImageUrl { get; set; }
    [JsonProperty("metadata")]
    public AppUserMetadata Metadata { get; set; }
}

public class AppUserMetadata
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
