namespace Blogsphere.Search.Api.Services.Factory;

public class SearchServiceFactory(IServiceProvider serviceProvider) : ISearchServiceFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public ISearchService<TDocument> Create<TDocument>() where TDocument : class
    {
        using var scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider;

        return service.GetRequiredService<ISearchService<TDocument>>();
    }

    public IPaginatedSearchService<TDocument> CreatePaginatedService<TDocument>() where TDocument : class
    {
        using var scope = _serviceProvider.CreateScope();
        var service = scope.ServiceProvider;

        return service.GetRequiredService<IPaginatedSearchService<TDocument>>();
    }
}
