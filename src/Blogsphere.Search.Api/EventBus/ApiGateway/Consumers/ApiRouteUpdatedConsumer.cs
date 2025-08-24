using AutoMapper;
using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Blogsphere.Search.Api.EventBus.ApiGateway.Consumers;

public class ApiRouteUpdatedConsumer(
    ILogger logger,
    IEventRecorderService eventRecorderService,
    IMapper mapper,
    ISearchService<ApiRouteSummary> searchService,
    IOptions<ElasticSearchOption> elasticSearchOption) : 
    ConsumerBase<ApiRouteUpdated>(eventRecorderService), IConsumer<ApiRouteUpdated>
{
    private readonly ILogger _logger = logger;
    private readonly IMapper _mapper = mapper;
    private readonly ISearchService<ApiRouteSummary> _searchService = searchService;
    private readonly ElasticSearchOption _elasticSearchOption = elasticSearchOption.Value;

    public async Task Consume(ConsumeContext<ApiRouteUpdated> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ApiRouteUpdated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started");

        var summary = _mapper.Map<ApiRouteSummary>(context.Message);
        var result = await _searchService.UpdateDocumentAsync(summary, new(){["id"] = summary.Id}, _elasticSearchOption.ApiRouteIndex);
        if(!result.IsSuccess)
        {
             _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("MessageType", typeof(ApiRouteUpdated).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);

            await RecordEvent(context, EventStatus.Failed);
        }

        await RecordEvent(context, EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ApiRouteUpdated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
} 