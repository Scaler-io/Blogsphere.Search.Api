namespace Blogsphere.Search.Api.Configurations;

public class ElasticSearchOption
{
    public const string OptionName = "ElasticSearch";
    public string Uri { get; set; }
    public string ApiClusterIndex { get; set; }
    public string ApiRouteIndex { get; set; }
    public string ManagementUserIndex { get; set; }
}
