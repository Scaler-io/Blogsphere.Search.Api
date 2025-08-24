
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace Blogsphere.Search.Api.Controllers.v1;

[ApiVersion("1")]
public class TestController : ApiBaseController
{
    private readonly IEventRecorderService _eventRecorderService;
    public TestController(ILogger logger, IEventRecorderService eventRecorderService, ISearchServiceFactory factory) : base(logger, factory)
    {
        _eventRecorderService = eventRecorderService;
    }

    [HttpGet("test-azure-storage")]
    public async Task<IActionResult> TestAzureStorage()
    {
        var result = await _eventRecorderService.GetEvent("123");
        return Ok(result);
    }
}
