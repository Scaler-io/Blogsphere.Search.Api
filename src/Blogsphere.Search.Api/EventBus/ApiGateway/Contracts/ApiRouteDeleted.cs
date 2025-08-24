namespace Contracts.Events;

public class ApiRouteDeleted : GenericEvent
{
    public string Id { get; set; }
    protected override GenericEventType Type { get; set; } = GenericEventType.ApiRouteDeleted;
}
