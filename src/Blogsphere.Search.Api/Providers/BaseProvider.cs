namespace Blogsphere.Search.Api.Providers;

public class BaseProvider(
    ILogger logger,
    IHttpClientFactory httpClientFactory,
    IdentityServiceProvider identityServiceProvider,
    ProviderConfigurationOption providerConfigurationOption,
    string apiProviderName)
{
    protected readonly ILogger _logger = logger;
    protected readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    protected readonly IdentityServiceProvider _identityServiceProvider = identityServiceProvider;
    protected readonly ProviderConfigurationOption _providerConfigurationOption = providerConfigurationOption;

    protected async Task<HttpClient> GetHttpClientAsync(bool isPublic = false)
    {
        try
        {
            _logger.Here().Debug("Creating HTTP client for provider: {ProviderName}", apiProviderName);
            var client = _httpClientFactory.CreateClient(apiProviderName);
            
            if (client == null)
            {
                _logger.Here().Error("Failed to create HTTP client for provider: {ProviderName}", apiProviderName);
                throw new InvalidOperationException($"HTTP client '{apiProviderName}' was not configured");
            }
            
            _logger.Here().Debug("HTTP client created successfully. BaseAddress: {BaseAddress}", client.BaseAddress);
            
            if (!isPublic)
            {
                _logger.Here().Debug("Getting access token for provider: {ProviderName}", apiProviderName);
                var token = await GetAccessTokenAsync();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                _logger.Here().Debug("Access token added to HTTP client headers");
            }
            
            return client;
        }
        catch (Exception ex)
        {
            _logger.Here().Error(ex, "Error creating HTTP client for provider: {ProviderName}", apiProviderName);
            throw;
        }
    }
    private async Task<string> GetAccessTokenAsync()
    {
        return await _identityServiceProvider.GetAccessTokenAsync(_providerConfigurationOption, apiProviderName);
    }
}
