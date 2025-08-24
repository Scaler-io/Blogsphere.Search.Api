using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Blogsphere.Search.Api.EventBus.ApiGateway.Consumers;

public class ApiRouteDeletedConsumer(
    ILogger logger,
    IEventRecorderService eventRecorderService,
    ISearchService<ApiRouteSummary> searchService,
    IOptions<ElasticSearchOption> elasticSearchOption) : 
    ConsumerBase<ApiRouteDeleted>(eventRecorderService), IConsumer<ApiRouteDeleted>
{
    private readonly ILogger _logger = logger;
    private readonly ISearchService<ApiRouteSummary> _searchService = searchService;
    private readonly ElasticSearchOption _elasticSearchOption = elasticSearchOption.Value;

    public async Task Consume(ConsumeContext<ApiRouteDeleted> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ApiRouteDeleted).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started for the event {type}", typeof(ApiRouteDeleted).Name);

        var result = await _searchService.RemoveDocumentAsync(new() { ["id"] = context.Message.Id  }, _elasticSearchOption.ApiRouteIndex);
        if(!result.IsSuccess)
        {
           _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("MessageType", typeof(ApiRouteDeleted).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);
            await RecordEvent(context, Models.Enums.EventStatus.Failed);
        }

        await RecordEvent(context, Models.Enums.EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ApiRouteDeleted).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
} 