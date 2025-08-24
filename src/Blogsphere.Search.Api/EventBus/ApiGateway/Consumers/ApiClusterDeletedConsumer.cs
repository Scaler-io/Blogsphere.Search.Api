using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Blogsphere.Search.Api.EventBus.ApiGateway.Consumers;

public class ApiClusterDeletedConsumer(
    ILogger logger,
    IEventRecorderService eventRecorderService,
    ISearchService<ApiClusterSummary> searchService,
    IOptions<ElasticSearchOption> elasticSearchOption) : 
    ConsumerBase<ApiClusterDeleted>(eventRecorderService), IConsumer<ApiClusterDeleted>
{
    private readonly ILogger _logger = logger;
    private readonly ISearchService<ApiClusterSummary> _searchService = searchService;
    private readonly ElasticSearchOption _elasticSearchOption = elasticSearchOption.Value;

    public async Task Consume(ConsumeContext<ApiClusterDeleted> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ApiClusterDeleted).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for the event {type}", typeof(ApiClusterDeleted).Name);

        var result = await _searchService.RemoveDocumentAsync(new() { ["id"] = context.Message.Id  }, _elasticSearchOption.ApiClusterIndex);
        if(!result.IsSuccess)
        {
           _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("MessageType", typeof(ApiClusterDeleted).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);
            await RecordEvent(context, Models.Enums.EventStatus.Failed);
        }

        await RecordEvent(context, Models.Enums.EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ApiClusterDeleted).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
}
