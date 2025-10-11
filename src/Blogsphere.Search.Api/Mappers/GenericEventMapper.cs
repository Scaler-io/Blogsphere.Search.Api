using AutoMapper;
using Contracts.Events;

namespace Blogsphere.Search.Api.Mappers;

public class GenericEventMapper : Profile
{
    public GenericEventMapper()
    {
        CreateMap<ApiClusterCreated, ApiClusterSummary>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastUpdatedAt));
        CreateMap<ApiClusterUpdated, ApiClusterSummary>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastUpdatedAt));
        CreateMap<ApiClusterDeleted, ApiClusterSummary>();
        CreateMap<ApiRouteCreated, ApiRouteSummary>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastUpdatedAt));
        CreateMap<ApiRouteUpdated, ApiRouteSummary>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastUpdatedAt));
        CreateMap<ApiRouteDeleted, ApiRouteSummary>();
    }
}
