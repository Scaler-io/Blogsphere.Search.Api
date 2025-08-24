using Nest;

namespace Blogsphere.Search.Api.Services;

public class QueryBuilderBaseService
{
    protected BoolQuery BuildBoolQuery(RequestQuery query)
    {
        var mustQueries = new List<QueryContainer>();
        var filterQueries = new List<QueryContainer>();

        var fieldQuery = BuildTermQuery(query.Filters);
        if(fieldQuery != null)
        {
            mustQueries.Add(fieldQuery);
        }

        var timeRangeQuery = BuildTimeRangeQuery(query.StartTime, query.EndTime, query.TimeField);
        if(timeRangeQuery != null)
        {
            filterQueries.Add(timeRangeQuery);
        }

        var fullTextSearchQuery = BuildFullTextSearchQuery(query.MatchPhrase, query.MatchPhraseField);
        if(fullTextSearchQuery != null)
        {
            mustQueries.Add(fullTextSearchQuery);
        }

        return new()
        {
            Must = mustQueries,
            Filter = filterQueries,
        };
    }

    private QueryContainer BuildFullTextSearchQuery(string matchPhrase, string matchPhraseField)
    {
        if (string.IsNullOrEmpty(matchPhrase) || string.IsNullOrEmpty(matchPhraseField))
        {
            return null;
        }
        var fields = matchPhraseField.Split([',', ' '], StringSplitOptions.RemoveEmptyEntries);

        return new MultiMatchQuery
        {
            Fields = fields,
            Query = matchPhrase,
            Type = TextQueryType.BestFields
        };
    }

    private QueryContainer BuildTimeRangeQuery(DateTime? startTime, DateTime? endTime, string timeField)
    {
        if (!startTime.HasValue && !endTime.HasValue)
        {
            return null;
        }

        var dateRangeQuery = new DateRangeQuery
        {
            Field = timeField
        };

        if (startTime.HasValue)
        {
            dateRangeQuery.GreaterThanOrEqualTo = DateMath.Anchored(startTime.Value);
        }

        if (endTime.HasValue)
        {
            dateRangeQuery.LessThanOrEqualTo = DateMath.Anchored(endTime.Value);
        }

        return dateRangeQuery;
    }

    private QueryContainer BuildTermQuery(Dictionary<string, string> filters)
    {
        if (filters == null) return null;
        var queryContainer = new QueryContainer();

        foreach (var filter in filters)
        {
            queryContainer &= new TermQuery
            {
                Field = filter.Key,
                Value = filter.Value
            };
        }

        return queryContainer;
    }
}
