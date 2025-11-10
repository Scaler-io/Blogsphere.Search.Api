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

public class ManagementUserUpdatedConsumer(
    ILogger logger,
    IEventRecorderService eventRecorderService,
    IMapper mapper,
    ISearchService<ManagementUserSummary> searchService,
    IPaginatedSearchService<ManagementUserSummary> paginatedSearchService,  
    IOptions<ElasticSearchOption> elasticSearchOption) : ConsumerBase<ManagementUserUpdated>(eventRecorderService), IConsumer<ManagementUserUpdated>
{
    private readonly ILogger _logger = logger;
    private readonly IEventRecorderService _eventRecorderService = eventRecorderService;
    private readonly IMapper _mapper = mapper;
    private readonly ISearchService<ManagementUserSummary> _searchService = searchService;
    private readonly ElasticSearchOption _elasticSearchOption = elasticSearchOption.Value;
    private readonly IPaginatedSearchService<ManagementUserSummary> _paginatedSearchService = paginatedSearchService;
    
    public async Task Consume(ConsumeContext<ManagementUserUpdated> context)
    {
        _logger.Here().MethodEntered();
        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ManagementUserUpdated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing started");

        var document = await _paginatedSearchService.GetPaginatedData(new(){
            PageIndex = 1,
            PageSize = 1,
            SortField = "createdAt",
            SortOrder = "Asc",
            IsFilteredQuery = true,
            MatchPhrase = context.Message.Id,
            MatchPhraseField = "id",
        }, context.Message.CorrelationId, _elasticSearchOption.ManagementUserIndex);

        if(!document.IsSuccess || document.Data.Data.Count == 0 || document.Data.Data.FirstOrDefault() == null)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("MessageType", typeof(ManagementUserUpdated).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", document.ErrorCode, document.ErrorMessage);
            await RecordEvent(context, EventStatus.Failed);
        }

        var documentData = document.Data.Data.FirstOrDefault();

        documentData.Roles = context.Message.Roles;
        documentData.Status = context.Message.Status;

        var result = await _searchService.UpdateDocumentAsync(documentData, new(){["id"] = documentData.Id}, _elasticSearchOption.ManagementUserIndex);
        if(!result.IsSuccess)
        {
            _logger.Here()
                .ForContext("MessageId", context.MessageId)
                .ForContext("MessageType", typeof(ManagementUserUpdated).Name)
                .WithCorrelationId(context.Message.CorrelationId)
                .Information("Message processing failed. {0} - {1}", result.ErrorCode, result.ErrorMessage);
            await RecordEvent(context, EventStatus.Failed);
        }

        _logger.Here().Information("Document data updated: {0}", documentData.Roles);
        await RecordEvent(context, EventStatus.Published);

        _logger.Here()
            .ForContext("MessageId", context.MessageId)
            .ForContext("MessageType", typeof(ManagementUserUpdated).Name)
            .WithCorrelationId(context.Message.CorrelationId)
            .Information("Message processing completed");   
    }
}
