using IdentityModel.Client;

namespace Blogsphere.Search.Api.Providers;

public class IdentityServiceProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger logger)
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = logger;

    public async Task<string> GetAccessTokenAsync(ProviderConfigurationOption providerConfigurationOption, string requestedClientName)
    {
        var client = _httpClientFactory.CreateClient(ApiProviderNames.IdentityApi);
        var identityGroupAccess = _configuration["IdentityGroupAccess:Authority"];
        var discoveryDocument = await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest{
            Address = identityGroupAccess,
            Policy = new DiscoveryPolicy { RequireHttps = false, ValidateIssuerName = false, ValidateEndpoints = true }
        });

        if(discoveryDocument.IsError)
        {
            throw new HttpRequestException($"Error retrieving discovery document: {discoveryDocument.Error}");
        }

        var response = await client.RequestClientCredentialsTokenAsync(GetTokenRequest(requestedClientName, providerConfigurationOption, discoveryDocument));
    
        _logger.Here().Information("token response {@tokenResponse}", response);

        if(response.IsError)
        {
            throw new HttpRequestException($"Error requesting token: {response.Error}");
        }

        return response.AccessToken;
    }

    private static ClientCredentialsTokenRequest GetTokenRequest(string requestedClientName, ProviderConfigurationOption providerConfigurationOption, DiscoveryDocumentResponse discoveryDocument)
    {
        return requestedClientName switch
        {
            "ApiGateway" => new()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = providerConfigurationOption.ApiGatewaySettings.ClientId,
                ClientSecret = providerConfigurationOption.ApiGatewaySettings.ClientSecret,
            },
            "UserApi" => new()
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = providerConfigurationOption.UserApiSettings.ClientId,
                ClientSecret = providerConfigurationOption.UserApiSettings.ClientSecret,
            },
            _ => throw new ArgumentException($"Invalid client name: {requestedClientName}")
        };
    }
}
