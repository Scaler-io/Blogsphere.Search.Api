namespace Blogsphere.Search.Api.DI;

public static class ServiceCollectionConfigurationExtensions
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AppConfigOption>().Bind(configuration.GetSection(AppConfigOption.OptionName)).ValidateDataAnnotations();
        services.AddOptions<ElasticSearchOption>().Bind(configuration.GetSection(ElasticSearchOption.OptionName)).ValidateDataAnnotations();
        services.AddOptions<EventBusOption>().Bind(configuration.GetSection(EventBusOption.OptionName)).ValidateDataAnnotations();
        services.AddOptions<LoggingOption>().Bind(configuration.GetSection(LoggingOption.OptionName)).ValidateDataAnnotations();
        services.AddOptions<ProviderConfigurationOption>().Bind(configuration.GetSection(ProviderConfigurationOption.OptionName)).ValidateDataAnnotations();
    
        return services;
    }
}
