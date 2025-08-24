namespace Blogsphere.Search.Api.Services.Interfaces;

public interface ISearchServiceFactory
{
    ISearchService<TDocument> Create<TDocument>() where TDocument : class;
    IPaginatedSearchService<TDocument> CreatePaginatedService<TDocument>() where TDocument: class;
}
