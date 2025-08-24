namespace Blogsphere.Search.Api.Services.Interfaces;

public interface ISearchService<TDocument> where TDocument : class
{
    Task<Result<bool>> SeedDocumentAsync(TDocument document, string id, string index);
    Task<Result<bool>> UpdateDocumentAsync(TDocument document, Dictionary<string, string> fieldValue, string index);
    Task<Result<bool>> SearchReIndex(string index);
    Task<Result<bool>> RemoveDocumentAsync(Dictionary<string, object> query, string index);
}
