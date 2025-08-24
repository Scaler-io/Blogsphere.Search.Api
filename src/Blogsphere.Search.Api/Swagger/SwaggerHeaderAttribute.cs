namespace Blogsphere.Search.Api.Swagger;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SwaggerHeaderAttribute(string name, string description = "", string type = "" , bool required = false) : Attribute
{
    public string Name { get; set; } = name;
    public string Type { get; set; } = type;
    public string Description { get; set; } = description;
    public bool Required { get; set; } = required;
}
