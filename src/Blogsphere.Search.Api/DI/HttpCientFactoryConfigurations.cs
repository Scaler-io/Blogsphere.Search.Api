using System.Net.Http.Headers;
namespace Blogsphere.Search.Api.DI;

public static class HttpCientFactoryConfigurations
{
    public static IServiceCollection AddHttpClientFactoryConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var providerSettings = configuration.GetSection(ProviderConfigurationOption.OptionName).Get<ProviderConfigurationOption>();
        var identityAuthority = configuration["IdentityGroupAccess:Authority"];

        services.AddHttpClient(ApiProviderNames.ApiGateway, client => 
        {
            client.BaseAddress = new Uri(providerSettings.ApiGatewaySettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-M2M-Request", "yes");
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddHttpClient(ApiProviderNames.IdentityApi, client => 
        {
            client.BaseAddress = new Uri(identityAuthority);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient(ApiProviderNames.UserApi, client => 
        {
            client.BaseAddress = new Uri(providerSettings.UserApiSettings.BaseUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("X-M2M-Request", "yes");
            client.DefaultRequestHeaders.Add("ocp-apim-subscriptionkey", providerSettings.UserApiSettings.SubscriptionKey);
            client.Timeout = TimeSpan.FromSeconds(15);
        });

        services.AddTransient<IdentityServiceProvider>();
        services.AddTransient<ApiGatewayProvider>();
        services.AddTransient<UserApiProvider>();

        return services;
    }
}
