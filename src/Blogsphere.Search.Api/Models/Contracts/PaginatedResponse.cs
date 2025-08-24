namespace Blogsphere.Search.Api.Models.Contracts;

public class PaginatedResponse<T> where T : class
{
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public List<T> Items { get; set; }
}
