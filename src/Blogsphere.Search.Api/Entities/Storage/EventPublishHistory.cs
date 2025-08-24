namespace Blogsphere.Search.Api.Entities.Storage;

public class EventPublishHistory : BaseTabeEntity  
{
    public string EventType { get; set; }
    public string FailureSource { get; set; }
    public string Data { get; set; }
    public EventStatus EventStatus { get; set; }
}
