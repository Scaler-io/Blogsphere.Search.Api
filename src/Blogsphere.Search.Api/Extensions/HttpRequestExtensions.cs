namespace Blogsphere.Search.Api.Extensions;

public static class HttpRequestExtensions
{
    public static string GetRequestHeaderOrDefault(this HttpRequest request, string key, string defaultValue = null)
    {
        var header = request?.Headers?.FirstOrDefault(x => x.Key.Equals(key)).Value.FirstOrDefault();
        return header ?? defaultValue;
    }
}
