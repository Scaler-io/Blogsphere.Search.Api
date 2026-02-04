using Microsoft.Extensions.Options;
using Nest;

namespace Blogsphere.Search.Api.Services.Pagination;

public class PaginatedSearchService<TDocument>(
    ILogger logger,
    IOptions<ElasticSearchOption> elasticSearchOption)
    : SearchServiceBase(logger, elasticSearchOption), IPaginatedSearchService<TDocument> where TDocument : class
{
    public async Task<Result<long>> GetCount(string correlationId, string searchIndex, RequestQuery query = null)
    {
        _logger.Here().MethodEntered();
        _logger.Here().WithCorrelationId(correlationId)
            .Information("Request - get requested data count in total from elastic search");

        if (string.IsNullOrEmpty(searchIndex))
        {
            _logger.Here().WithCorrelationId(correlationId).Error("No actual index found with index type {serachIndex}", searchIndex);
            return Result<long>.Failure(ErrorCodes.NotFound, "Search index not found");
        }

        CountResponse countResponse = new();

        if (query != null)
            countResponse = await ElasticSearchClient.CountAsync<TDocument>(s => s.Index(searchIndex).Query(q => !query.IsFilteredQuery ? q.MatchAll() : BuildBoolQuery(query)));
        else
            countResponse = await ElasticSearchClient.CountAsync<TDocument>(s => s.Index(searchIndex));

        if (!countResponse.IsValid)
        {
            _logger.Here().WithCorrelationId(correlationId).Error("{documentType} search failed", typeof(TDocument).Name);
            return Result<long>.Failure(ErrorCodes.InternalServerError, ErrorMessages.InternalServerError);
        }

        _logger.Here().WithCorrelationId(correlationId).Information("total {count} items found of type {documentType}", typeof(TDocument).Name, countResponse.Count);
        _logger.Here().MethodExited();

        return Result<long>.Success(countResponse.Count);
    }

    public async Task<Result<Pagination<TDocument>>> GetPaginatedData(RequestQuery query, string correlationId, string searchIndex)
    {
        _logger.Here().MethodEntered();
        _logger.Here().WithCorrelationId(correlationId).Information("Request - get pagincated data from elastic search");

        if (string.IsNullOrEmpty(searchIndex))
        {
            _logger.Here().WithCorrelationId(correlationId).Error("No actual index found with index type {serachIndex}", searchIndex);
            return Result<Pagination<TDocument>>.Failure(ErrorCodes.NotFound, "Search index not found");
        }

        Pagination<TDocument> paginatedResult;
        double maxSearchScore;

        if (query.SearchType == SearchType.Paginated)
        {
            var searchResponse = await ElasticSearchClient.SearchAsync<TDocument>(s => s
               .Index(searchIndex)
               .Size(query.PageSize)
               .From((query.PageIndex - 1) * query.PageSize)
               .Sort(sort => sort.Field(query.SortField, query.SortOrder == "Asc" ? SortOrder.Ascending : SortOrder.Descending))
               .Query(q => !query.IsFilteredQuery ? q.MatchAll() : BuildBoolQuery(query)));

            if (!searchResponse.IsValid)
            {
                _logger.Here().WithCorrelationId(correlationId).Error("{documentType} search failed", typeof(TDocument).Name);
                return Result<Pagination<TDocument>>.Failure(ErrorCodes.InternalServerError, ErrorMessages.InternalServerError);
            }
            paginatedResult = new Pagination<TDocument>(query.PageIndex, query.PageSize, searchResponse.Hits.Count, [.. searchResponse.Documents]);
            maxSearchScore = searchResponse.MaxScore;
        }
        else
        {
            var (pagination, maxSearchScoreInternal) = await GetPaginatedDataInternal(query, correlationId, searchIndex);
            if (pagination == null)
            {
                _logger.Here().WithCorrelationId(correlationId).Error("{documentType} search failed", typeof(TDocument).Name);
                return Result<Pagination<TDocument>>.Failure(ErrorCodes.InternalServerError, ErrorMessages.InternalServerError);
            }
            paginatedResult = pagination;
            maxSearchScore = maxSearchScoreInternal;
        }

        _logger.Here().WithCorrelationId(correlationId)
                .ForContext("maxSearchScore", maxSearchScore)
                .Information("{documentType} document search successfull", typeof(TDocument).Name);
        _logger.Here().MethodExited();

        return Result<Pagination<TDocument>>.Success(paginatedResult);
    }

    private async Task<(Pagination<TDocument>, double)> GetPaginatedDataInternal(RequestQuery query, string correlationId, string searchIndex)
    {
        var allDocuments = new List<TDocument>();
        IReadOnlyCollection<object> searchAfter = null;
        double maxSearchScoreInternal = 0;
        while (true)
        {
            var response = await ElasticSearchClient.SearchAsync<TDocument>(s => s
            .Index(searchIndex)
            .Size(query.PageSize) // batch size
            .Sort(BuildSortDescriptor<TDocument>(query.SortField, query.SortOrder))
            .SearchAfter(searchAfter)
            .Query(q =>
                !query.IsFilteredQuery
                    ? q.MatchAll()
                    : BuildBoolQuery(query))
            );

            if (!response.IsValid)
            {
                _logger.Here().WithCorrelationId(correlationId)
                    .Error("{documentType} search_after failed", typeof(TDocument).Name);

                return (null, 0);
            }

            if (!response.Hits.Any())
                break;

            allDocuments.AddRange(response.Documents);

            // IMPORTANT: use the full sort array
            searchAfter = response.Hits.Last().Sorts;
            maxSearchScoreInternal = response.MaxScore;
        }
        return (new Pagination<TDocument>(query.PageIndex, query.PageSize, allDocuments.Count, [.. allDocuments]), maxSearchScoreInternal);
    }
}
