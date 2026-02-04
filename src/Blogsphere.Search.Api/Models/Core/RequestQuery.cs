namespace Blogsphere.Search.Api.Models.Core;

public class RequestQuery
{
    private int MaxPageSize { get; set; } = 50;
    public int PageIndex { get; set; } = 1;
    private int _pageSize { get; set; } = 5;
    public string SortField { get; set; } = "lastUpdatedOn";
    public string SortOrder { get; set; } = "Asc";

    public Dictionary<string, string> Filters { get; set; }
    public string MatchPhrase { get; set; }
    public string MatchPhraseField { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string TimeField { get; set; }
    public SearchType SearchType { get; set; } = SearchType.Paginated;

    public bool IsFilteredQuery { get; set; } = false;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
