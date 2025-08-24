using Azure.Data.Tables;

namespace Blogsphere.Search.Api.DI;

public static class ServiceCollectionDataExtensions
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(new TableServiceClient(
            configuration.GetConnectionString("BlogsphereStorageConnection")
        ));

        services.AddScoped(typeof(IDataTableRepository<>), typeof(DataTableRepository<>));
        return services;
    }
}
