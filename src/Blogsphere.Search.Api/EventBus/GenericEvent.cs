namespace Blogsphere.Search.Api.EventBus;

public abstract class GenericEvent
{
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
    public string CorrelationId { get; set; }
    public object AdditionalProperties { get; set; }
    protected abstract GenericEventType Type { get; set; }
}
