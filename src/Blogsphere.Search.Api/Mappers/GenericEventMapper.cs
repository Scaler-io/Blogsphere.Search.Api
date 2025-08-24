using AutoMapper;
using Contracts.Events;

namespace Blogsphere.Search.Api.Mappers;

public class GenericEventMapper : Profile
{
    public GenericEventMapper()
    {
        CreateMap<ApiClusterCreated, ApiClusterSummary>();
        CreateMap<ApiClusterUpdated, ApiClusterSummary>();
        CreateMap<ApiClusterDeleted, ApiClusterSummary>();
        CreateMap<ApiRouteCreated, ApiRouteSummary>();
        CreateMap<ApiRouteUpdated, ApiRouteSummary>();
        CreateMap<ApiRouteDeleted, ApiRouteSummary>();
    }
}
