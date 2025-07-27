using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Blogsphere.Search.Api.Swagger;

public class SwaggerRemoveVersionFromRoute : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var modifiedPath = new OpenApiPaths();
        foreach(var path in swaggerDoc.Paths)
        {
            string pathWithoutVersion = path.Key[7..];
            if(string.IsNullOrEmpty(pathWithoutVersion))
            {
                pathWithoutVersion = path.Key;
            }
            modifiedPath.Add(pathWithoutVersion, path.Value);
        }

        swaggerDoc.Paths = modifiedPath;
    }
}
