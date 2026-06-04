
using Blogsphere.Search.Api.Converters;
using Blogsphere.Search.Api.Models.Contracts.User.AppUser;
using Blogsphere.Search.Api.Models.Contracts.User.ManagementUser;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Blogsphere.Search.Api.Providers;

public class UserApiProvider(
    ILogger logger,
    IHttpClientFactory httpClientFactory,
    IdentityServiceProvider identityServiceProvider,
    IOptions<ProviderConfigurationOption> providerConfigurationOption
    ) 
    : BaseProvider(logger, httpClientFactory, identityServiceProvider, providerConfigurationOption.Value, ApiProviderNames.UserApi)
{

    public async Task<Result<PaginatedResponse<ManagementUser>>> GetManagementUsersAsync()
    {
        _logger.Here().MethodEntered();
        _logger.Here().Debug("Making HTTP call to user api endpoint");

        var items = new List<ManagementUser>();
        var client = await GetHttpClientAsync();
        client.DefaultRequestHeaders.Add("api-version", "v2");

        _logger.Here().Information("{baseUrl} - {headers}", client.BaseAddress, client.DefaultRequestHeaders);

        var response = await client.GetAsync("managementuser?pageNumber=1&pageSize=100");

        if (!response.IsSuccessStatusCode)
        {
            _logger.Here().Error("Failed to load management users {0} - {1}", response.StatusCode, response.ReasonPhrase);
            return Result<PaginatedResponse<ManagementUser>>.Failure(ErrorCodes.OperationFailed);
        }

        var jsonSettings = new JsonSerializerSettings
        {
            Converters = { new CustomDateTimeConverter() }
        };

        var data = await response.Content.ReadAsStringAsync();
        var managementUserResponse = JsonConvert.DeserializeObject<PaginatedResponse<ManagementUser>>(data, jsonSettings);
        
        
        var totalPages = (int)Math.Ceiling((double)managementUserResponse.TotalCount / managementUserResponse.PageSize);

        items.AddRange(managementUserResponse.Items);
        for(var i = 2; i <= totalPages; i++)
        {
            response = await client.GetAsync($"/managementuser?pageNumber={i}");
            data = await response.Content.ReadAsStringAsync();
            managementUserResponse = JsonConvert.DeserializeObject<PaginatedResponse<ManagementUser>>(data, jsonSettings);
            items.AddRange(managementUserResponse.Items);
        }
        _logger.Here().MethodExited();
        
        return Result<PaginatedResponse<ManagementUser>>.Success(new PaginatedResponse<ManagementUser>
        {
            Items = items,
            TotalCount = items.Count,
            PageSize = 1,
            PageNumber = 1
        });
    }

    public async Task<Result<PaginatedResponse<AppUser>>> GetAppUsersAsync()
    {
        _logger.Here().MethodEntered();
        _logger.Here().Debug("Making HTTP call to user api endpoint");

        var items = new List<AppUser>();
        var client = await GetHttpClientAsync();
        client.DefaultRequestHeaders.Add("api-version", "v2");

        _logger.Here().Information("{baseUrl} - {headers}", client.BaseAddress, client.DefaultRequestHeaders);

        var response = await client.GetAsync("appuser?pageNumber=1&pageSize=100");

        if (!response.IsSuccessStatusCode)
        {
            _logger.Here().Error("Failed to load app users {0} - {1}", response.StatusCode, response.ReasonPhrase);
        }

        var jsonSettings = new JsonSerializerSettings
        {
            Converters = { new CustomDateTimeConverter() }
        };

        var data = await response.Content.ReadAsStringAsync();
        var appUserResponse = JsonConvert.DeserializeObject<PaginatedResponse<AppUser>>(data, jsonSettings);

        var totalPages = (int)Math.Ceiling((double)appUserResponse.TotalCount / appUserResponse.PageSize);

        items.AddRange(appUserResponse.Items);
        for(var i = 2; i <= totalPages; i++)
        {
            response = await client.GetAsync($"/appuser?pageNumber={i}");
            data = await response.Content.ReadAsStringAsync();
            appUserResponse = JsonConvert.DeserializeObject<PaginatedResponse<AppUser>>(data, jsonSettings);
            items.AddRange(appUserResponse.Items);
        }

        _logger.Here().MethodExited();

        return Result<PaginatedResponse<AppUser>>.Success(new PaginatedResponse<AppUser>
        {
            Items = items,
            TotalCount = items.Count,
            PageSize = 1,
            PageNumber = 1
        });
    }
}
