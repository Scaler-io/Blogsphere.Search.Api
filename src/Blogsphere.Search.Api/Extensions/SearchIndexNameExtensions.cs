namespace Blogsphere.Search.Api.Extensions;

public static class SearchIndexNameExtensions
{
    public static bool IsApiClusterIndex(this string indexName) => indexName == "apicluster-search-index";
    public static bool IsApiRouteIndex(this string indexName) => indexName == "apiroute-search-index";
    public static bool IsManagementUserIndex(this string indexName) => indexName == "managementuser-search-index";
    public static bool IsAppUserIndex(this string indexName) => indexName == "appuser-search-index";
}
