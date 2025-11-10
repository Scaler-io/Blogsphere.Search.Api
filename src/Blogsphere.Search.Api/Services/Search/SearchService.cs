using Blogsphere.Search.Api.Models.Contracts.User.ManagementUser;
using Elasticsearch.Net;
using AutoMapper;
using Microsoft.Extensions.Options;
using Nest;
using Blogsphere.Search.Api.Entities.User;

namespace Blogsphere.Search.Api.Services.Search;

public class SearchService<TDocument>(
    ILogger logger,
    IOptions<ElasticSearchOption> elasticSearchOption,
    ApiGatewayProvider apiGatewayProvider,
    UserApiProvider userApiProvider,
    IMapper mapper) 
: SearchServiceBase(logger, elasticSearchOption), ISearchService<TDocument> where TDocument : class
{
    private readonly ApiGatewayProvider _apiGatewayProvider = apiGatewayProvider;
    private readonly UserApiProvider _userApiProvider = userApiProvider;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<bool>> SeedDocumentAsync(TDocument document, string id, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Debug("Requesting - storing data to elastic search {index}", index);

        if (!await IndexExist(index))
        {
            await CreateNewIndex<TDocument>(index);
        }

        var indexResponse = await ElasticSearchClient.IndexAsync(document, idx => idx.Index(index)
            .Id(id)
            .Refresh(Refresh.WaitFor));

        if (!indexResponse.IsValid)
        {
            _logger.Here().Error("Failed to store data to elastic search {index} with error {error}", index, indexResponse.DebugInformation);
            return Result<bool>.Failure(ErrorCodes.OperationFailed);
        }

        _logger.Here().Debug("Data seeding completed successfully for document {id}", id);
        _logger.Here().MethodExited();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UpdateDocumentAsync(TDocument document, Dictionary<string, string> fieldValue, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Debug("Request - update document in elastic search {@index}", index);

         var documentResponse = await ElasticSearchClient
                .SearchAsync<TDocument>(s => s.Index(index)
                .Query(q => q.Match(m => m
                    .Field(fieldValue.Keys.First())
                    .Query(fieldValue.Values.First())
                )));

        var docId = documentResponse.Hits.First().Id;
        var data = documentResponse.Documents.First();
        var documentUpdateResponse = await ElasticSearchClient.UpdateAsync<TDocument, object>(
                new DocumentPath<TDocument>(docId).Index(index),
                u => u.Doc(document)
                    .DocAsUpsert()
            );
        var docData = documentUpdateResponse.Result;

        if (!documentUpdateResponse.IsValid)
        {
            _logger.Here().Error("Elastic search docuemnt update failed");
            return Result<bool>.Failure(ErrorCodes.OperationFailed, "Elastic document update failed");
        }

        _logger.Here().Debug("Elastic document update successfull");
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RemoveDocumentAsync(Dictionary<string, object> query, string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Debug("Request - remove data from elastic search {index}", index);

        if (!await IndexExist(index))
        {
            return Result<bool>.Failure(ErrorCodes.OperationFailed, $"Elastic operation failed, index {index} was not found");
        }

        var fieldName = query.Keys.First();
        var fieldValue = query.Values.First();

        var deleteResponse = await ElasticSearchClient.DeleteByQueryAsync<TDocument>(d => d
                .Index(index)
                .Query(q => q
                .Match(m => m.Field(fieldName).Query(fieldValue.ToString()))));

        if (!deleteResponse.IsValid)
        {
            _logger.Here().Error("Elastic search delete operation failed");
            return Result<bool>.Failure(ErrorCodes.OperationFailed, "Elastic document delete operation failed");
        }

        _logger.Here().Debug("Elastic document delete successful");
        _logger.Here().MethodExited();
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SearchReIndex(string index)
    {
        _logger.Here().MethodEntered();
        _logger.Here().Debug("Request - reindex search data for {@index}", index);

        if (await IndexExist(index))
        {
            var deleteResposne = await ElasticSearchClient.Indices.DeleteAsync(index);
            if (!deleteResposne.IsValid)
            {
                _logger.Here().Error("Index deletion failed");
                return Result<bool>.Failure(ErrorCodes.OperationFailed, "Index deletion failed");
            }
        }

        await CreateNewIndex<TDocument>(index);
        BulkResponse bulkResponse = new();

        switch(index)
        {
            case "apicluster-search-index":
                var apiClusterSummaries = await GetApiClusterAsync();
                bulkResponse = await ElasticSearchClient.BulkAsync(b => b.Index(index).IndexMany(apiClusterSummaries.Items));
                break;
            case "apiroute-search-index":
                var apiRouteSummaries = await GetApiRouteAsync();
                bulkResponse = await ElasticSearchClient.BulkAsync(b => b.Index(index).IndexMany(apiRouteSummaries.Items));
                break;
            case "managementuser-search-index":
                var managementUserSummaries = _mapper.Map<List<ManagementUserSummary>>((await GetManagementUserAsync()).Items);
                bulkResponse = await ElasticSearchClient.BulkAsync(b => b.Index(index).IndexMany(managementUserSummaries));
                break;
            default:
                break;
        }

        if (!bulkResponse.IsValid)
        {
            _logger.Here().Error("Re-indexing failed");
            return Result<bool>.Failure(ErrorCodes.OperationFailed, "Re-index operation failed");
        }

        _logger.Here().Information("Re-index for {index} successful", index);
        return Result<bool>.Success(true);
    }

    private async Task<PaginatedResponse<ApiCluster>> GetApiClusterAsync()
    {
        var results = await _apiGatewayProvider.GetApiClustersAsync();
        return results.Data;
    }

    private async Task<PaginatedResponse<ApiRoute>> GetApiRouteAsync()
    {
        var results = await _apiGatewayProvider.GetApiRoutesAsync();
        return results.Data;
    }

    private async Task<PaginatedResponse<ManagementUser>> GetManagementUserAsync()
    {
        var results = await _userApiProvider.GetManagementUsersAsync();
        return results.Data;
    }
}
