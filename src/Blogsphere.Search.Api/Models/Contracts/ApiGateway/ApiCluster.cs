namespace Blogsphere.Search.Api.Models.Contracts.ApiGateway;
public class ApiCluster
{
    public string Id { get; set; }
    public string ClusterId { get; set; }
    public string LoadBalancerName { get; set; }
    public string HealthCheckEnabled { get; set; }
    public string HealthCheckPath { get; set; }
    public long DestinationCount { get; set; }
    public long RouteCount { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}