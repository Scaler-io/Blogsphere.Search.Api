namespace Blogsphere.Search.Api.Services.Interfaces;

public interface IPaginatedSearchService<TDocument> where TDocument : class
{
    Task<Result<Pagination<TDocument>>> GetPaginatedData(RequestQuery query, string correlationId, string searchIndex);
    Task<Result<long>> GetCount(string CorrelationId, string searchIndex, RequestQuery query = null);
}
