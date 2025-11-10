namespace Blogsphere.Search.Api.Configurations;

public class ProviderConfigurationOption
{
    public const string OptionName = "ProviderSettings";
    public ApiGatewaySettings ApiGatewaySettings { get; set; }
    public UserApiSettings UserApiSettings { get; set; }
}

public class ApiSettings
{
    public string BaseUrl { get; set; }
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string SubscriptionKey { get; set; }
}

public class ApiGatewaySettings : ApiSettings {}
public class UserApiSettings : ApiSettings {}