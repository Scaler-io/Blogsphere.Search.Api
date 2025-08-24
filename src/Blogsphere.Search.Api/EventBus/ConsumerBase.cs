using MassTransit;
using Newtonsoft.Json;

namespace Blogsphere.Search.Api.EventBus;

public class ConsumerBase<TEvent>(IEventRecorderService eventRecorderService) where TEvent : GenericEvent
{
    private readonly IEventRecorderService _eventRecorderService = eventRecorderService;

    protected async Task RecordEvent(ConsumeContext<TEvent> context, EventStatus status)
    {
        var recordExist = await _eventRecorderService.GetEvent(context.Message.CorrelationId);
        if (!recordExist.IsSuccess)
        {
            var jsonData = JsonConvert.SerializeObject(context.Message);
            EventPublishHistory history = new()
            {
                PartitionKey = context.Message.CorrelationId,
                RowKey = context.Message.CorrelationId,
                UpdatedAt = DateTime.UtcNow,
                EventStatus = status,
                FailureSource = "Blogsphere.Search.Api",
                EventType = typeof(TEvent).Name,
                Data = jsonData,
            };
            await _eventRecorderService.CreateEvent(history);
        }
        else
        {
            recordExist.Data.EventStatus = status;
            await _eventRecorderService.UpdateGenericEvent(recordExist.Data);
        }
    }
}
