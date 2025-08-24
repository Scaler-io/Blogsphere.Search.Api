namespace Contracts.Events;

public class ApiClusterDeleted : GenericEvent
{
    public string Id { get; set; }
    protected override GenericEventType Type { get; set; } = GenericEventType.ApiClusterDeleted;
}
