namespace Contracts.Events;

public class ApiClusterUpdated : GenericEvent
{
    public string Id { get; set; }
    public string ClusterId { get; set; }
    public string LoadBalancerName { get; set; }
    public string HealthCheckEnabled { get; set; }
    public string HealthCheckPath { get; set; }
    public long DestinationCount { get; set; }
    public long RouteCount { get; set; }
    public string Status { get; set; }
    protected override GenericEventType Type { get; set; } = GenericEventType.ApiClusterUpdated;
}
