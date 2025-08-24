using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Blogsphere.Search.Api.Providers;

public class ApiGatewayProvider(
    IHttpClientFactory httpClientFactory,
    IdentityServiceProvider identityServiceProvider,
    IOptions<ProviderConfigurationOption> providerConfigurationOption,
    ILogger logger) : BaseProvider(logger, httpClientFactory, identityServiceProvider, providerConfigurationOption.Value, ApiProviderNames.ApiGateway)        
{
    public async Task<Result<PaginatedResponse<ApiCluster>>> GetApiClustersAsync()
    {
        _logger.Here().MethodEntered();
        _logger.Here().Debug("Making HTTP call to api gateway endpoint");

        var items = new List<ApiCluster>();

        var client = await GetHttpClientAsync();
        client.DefaultRequestHeaders.Add("api-version", "v1");

        _logger.Here().Information("{baseUrl} - {headers}", client.BaseAddress, client.DefaultRequestHeaders);

        var response = await client.GetAsync("api/v1/proxycluster?pageNumber=1&pageSize=100");

        if (!response.IsSuccessStatusCode)
        {
            _logger.Here().Error("Failed to load api clusters {0} - {1}", response.StatusCode, response.ReasonPhrase);
            return Result<PaginatedResponse<ApiCluster>>.Failure(ErrorCodes.OperationFailed);
        }

        var data = await response.Content.ReadAsStringAsync();
        var clusterResponse = JsonConvert.DeserializeObject<PaginatedResponse<ApiCluster>>(data);
        var totalPages = (int)Math.Ceiling((double)clusterResponse.TotalCount / clusterResponse.PageSize);
       
        items.AddRange(clusterResponse.Items);

        for(var i = 2; i <= totalPages; i++)
        {
            response = await client.GetAsync($"api/v1/proxycluster?pageNumber={i}");
            data = await response.Content.ReadAsStringAsync();
            clusterResponse = JsonConvert.DeserializeObject<PaginatedResponse<ApiCluster>>(data);
            items.AddRange(clusterResponse.Items);
        }

        _logger.Here().MethodExited();

        return Result<PaginatedResponse<ApiCluster>>.Success(new PaginatedResponse<ApiCluster>
        {
            Items = items,
            TotalCount = items.Count,
            PageSize = 1,
            PageNumber = 1
        });
    }

    public async Task<Result<PaginatedResponse<ApiRoute>>> GetApiRoutesAsync()
    {
        _logger.Here().MethodEntered();
        _logger.Here().Debug("Making HTTP call to api gateway endpoint");

        var items = new List<ApiRoute>();

        var client = await GetHttpClientAsync();
        client.DefaultRequestHeaders.Add("api-version", "v1");

        _logger.Here().Information("{baseUrl} - {headers}", client.BaseAddress, client.DefaultRequestHeaders);

        var response = await client.GetAsync("api/v1/proxyroute?pageNumber=1&pageSize=100");

        if (!response.IsSuccessStatusCode)
        {
            _logger.Here().Error("Failed to load api routes {0} - {1}", response.StatusCode, response.ReasonPhrase);
            return Result<PaginatedResponse<ApiRoute>>.Failure(ErrorCodes.OperationFailed);
        }

        var data = await response.Content.ReadAsStringAsync();
        var routeResponse = JsonConvert.DeserializeObject<PaginatedResponse<ApiRoute>>(data);
        var totalPages = (int)Math.Ceiling((double)routeResponse.TotalCount / routeResponse.PageSize);

        items.AddRange(routeResponse.Items);

        for(var i = 2; i <= totalPages; i++)
        {
            response = await client.GetAsync($"api/v1/proxyroute?pageNumber={i}");
            data = await response.Content.ReadAsStringAsync();
            routeResponse = JsonConvert.DeserializeObject<PaginatedResponse<ApiRoute>>(data);
            items.AddRange(routeResponse.Items);
        }

        _logger.Here().MethodExited();
        return Result<PaginatedResponse<ApiRoute>>.Success(new PaginatedResponse<ApiRoute>
        {
            Items = items,
            TotalCount = items.Count,
            PageSize = 1,
            PageNumber = 1
        });
    }
}
