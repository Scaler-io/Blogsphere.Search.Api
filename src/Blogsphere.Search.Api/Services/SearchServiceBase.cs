using Blogsphere.Search.Api.Entities.User;
using Microsoft.Extensions.Options;
using Nest;

namespace Blogsphere.Search.Api.Services;

public class SearchServiceBase : QueryBuilderBaseService
{
    protected ElasticClient ElasticSearchClient { get; set; }
    protected ILogger _logger;
    protected ElasticSearchOption _elasticSearchOption;

    public SearchServiceBase(ILogger logger, IOptions<ElasticSearchOption> elasticSearchOption)
    {
        _logger = logger;
        _elasticSearchOption = elasticSearchOption.Value;
        var elasticUri = new Uri(_elasticSearchOption.Uri);
        var connectionString = new ConnectionSettings(elasticUri);
        ElasticSearchClient = new ElasticClient(connectionString);
    }

    protected async Task<bool> CreateNewIndex<TDocument>(string index) where TDocument : class
    {
        _logger.Here().Information("Creating new index with name {index}", index);

        var createIndexResponse = await ElasticSearchClient.Indices.CreateAsync(index, c => c
            .Settings(s => s
                .NumberOfShards(1)
                .NumberOfReplicas(1)
                .Analysis(a => a
                    .Tokenizers(t => t
                        .EdgeNGram("ngram_tokenizer", e => e
                            .MinGram(3)
                            .MaxGram(5)
                            .TokenChars(TokenChar.Letter, TokenChar.Digit, TokenChar.Symbol, TokenChar.Whitespace)
                        )
                    )
                    .Analyzers(an => an
                        .Custom("ngram_analyzer", ca => ca
                            .Tokenizer("ngram_tokenizer")
                            .Filters("lowercase")
                        )
                    )
                )
            )
            .Map<TDocument>(m => m
            .AutoMap())
            .Map<TDocument>(m => CreateMapping(m))
        );

        if (!createIndexResponse.IsValid)
        {
            _logger.Here().Error("Failed to create index {index} with error {error}", index, createIndexResponse.DebugInformation);
            return false;
        }

        return true;
    }

    protected async Task<bool> IndexExist(string index)
    {
        _logger.Here().Information("No index found with name {index}", index);
        var indexResponse = await ElasticSearchClient.Indices.ExistsAsync(index);
        return indexResponse.Exists;
    }

    protected Func<SortDescriptor<TDoc>, IPromise<IList<ISort>>> BuildSortDescriptor<TDoc>(string sortField, string sortOrder) where TDoc : class
    {
        return sort => sort
            .Field(sortField,
                sortOrder == "Asc"
                    ? SortOrder.Ascending
                    : SortOrder.Descending)
            // mandatory tie-breaker
            .Field("_id", SortOrder.Ascending);
    }

    private static ITypeMapping CreateMapping<TDocument>(TypeMappingDescriptor<TDocument> m) where TDocument : class
    {
        var mappingActions = new Dictionary<Type, Action<TypeMappingDescriptor<TDocument>>>
        {
            [typeof(ApiClusterSummary)] = descriptor => descriptor.Properties<ApiClusterSummary>(p => p
                .Text(k => k.Name(n => n.ClusterId).Analyzer("ngram_analyzer"))
                .Keyword(k => k.Name(n => n.LoadBalancerName))
                .Keyword(k => k.Name(n => n.Status))
            ),
            [typeof(ApiRouteSummary)] = descriptor => descriptor.Properties<ApiRouteSummary>(p => p
                .Text(k => k.Name(n => n.RouteId).Analyzer("ngram_analyzer"))
                .Text(k => k.Name(n => n.Path).Analyzer("ngram_analyzer"))
                .Keyword(k => k.Name(n => n.Cluster))
                .Keyword(k => k.Name(n => n.RateLimitterPolicy))
                .Keyword(k => k.Name(n => n.Status))
            ),
            [typeof(ManagementUserSummary)] = descriptor => descriptor.Properties<ManagementUserSummary>(p => p
                .Keyword(k => k.Name(n => n.EmployeeId))
                .Text(k => k.Name(n => n.FullName).Analyzer("ngram_analyzer"))
                .Text(k => k.Name(n => n.Email).Analyzer("ngram_analyzer"))
                .Keyword(k => k.Name(n => n.Department))
                .Keyword(k => k.Name(n => n.JobTitle))
                .Keyword(k => k.Name(n => n.Roles))
                .Keyword(k => k.Name(n => n.Status))
            )
        };

        // Check if a mapping action exists for the given type
        if (mappingActions.TryGetValue(typeof(TDocument), out var action)) action(m);
        return m;
    }
}
