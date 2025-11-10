using AutoMapper;
using Blogsphere.Search.Api.Entities.User;
using Blogsphere.Search.Api.Models.Contracts.User.ManagementUser;
using Contracts.Events;

namespace Blogsphere.Search.Api.Mappers;

public class GenericEventMapper : Profile
{
    public GenericEventMapper()
    {
        // Api Cluster and Api Route
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

        // Management User
        CreateMap<ManagementUserCreated, ManagementUserSummary>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.LastUpdatedAt));
        CreateMap<ManagementUser, ManagementUserSummary>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Metadata.CreatedAt, DateTimeKind.Utc)))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.SpecifyKind(src.Metadata.UpdatedAt, DateTimeKind.Utc)))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles.Select(r => r.Name).ToList()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.IsActive ? "Active" : "Inactive"));
    }
}
