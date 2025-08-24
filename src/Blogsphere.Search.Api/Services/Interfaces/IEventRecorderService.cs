namespace Blogsphere.Search.Api.Services.Interfaces;

public interface IEventRecorderService
{
    Task<Result<EventPublishHistory>> GetEvent(string correlationId);
    Task<Result<bool>> CreateEvent(EventPublishHistory history);
    Task<Result<bool>> UpdateGenericEvent(EventPublishHistory history);
}
