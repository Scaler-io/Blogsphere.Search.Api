namespace Contracts.Events;

public class ApiRouteUpdated : GenericEvent
{
    public string Id { get; set; }
    public string RouteId { get; set; }
    public string Path { get; set; }
    public string Cluster { get; set; }
    public string RateLimitterPolicy { get; set; }
    public long TransformCount { get; set; }
    public string Status { get; set; }
    protected override GenericEventType Type { get; set; } = GenericEventType.ApiRouteUpdated;
}
