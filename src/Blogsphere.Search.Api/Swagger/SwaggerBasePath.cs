using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Blogsphere.Search.Api.Swagger;

public class SwaggerBasePath : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var docVersion = swaggerDoc.Info.Version;
        var groupName = context.ApiDescriptions
        .Select(x => x.GroupName)
        .FirstOrDefault(apiVersion => apiVersion == docVersion);

        if(groupName != null)
        {
            swaggerDoc.Servers.Clear();
            swaggerDoc.Servers.Add(new OpenApiServer { Url = $"/{groupName}" });
        }
    }
}
