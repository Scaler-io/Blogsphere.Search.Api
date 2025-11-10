using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blogsphere.Search.Api.Entities.User;  
using Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Options;

namespace Blogsphere.Search.Api.EventBus.User.ManagementUser.Consumers;

public class ManagementUserCreatedConsumer(
    ILogger logger,
    IEventRecorderService eventRecorderService,
    IMapper mapper,
    ISearchService<ManagementUserSummary> searchService,
    IOptions<ElasticSearchOption> elasticSearchOption) : ConsumerBase<ManagementUserCreated>(eventRecorderService), IConsumer<ManagementUserCreated>
{
    private readonly ILogger _logger = logger;
    private readonly IEventRecorderService _eventRecorderService = eventRecorderService;
    private readonly IMapper _mapper = mapper;
    private readonly ISearchService<ManagementUserSummary> _searchService = searchService;
    private readonly ElasticSearchOption _elasticSearchOption = elasticSearchOption.Value;

    public async Task Consume(ConsumeContext<ManagementUserCreated> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ManagementUserCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started");

        var summary = _mapper.Map<ManagementUserSummary>(context.Message);
        var result = await _searchService.SeedDocumentAsync(summary, summary.Id, _elasticSearchOption.ManagementUserIndex);

        if(!result.IsSuccess)
        {
            _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ManagementUserCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);

            await RecordEvent(context, EventStatus.Failed);
            return;
        }

        await RecordEvent(context, EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ManagementUserCreated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");
    }
}
