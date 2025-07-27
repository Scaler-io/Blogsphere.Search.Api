namespace Blogsphere.Search.Api.Configurations;

public class AppConfigOption
{
    public const string OptionName = "AppConfigurations";
    public string ApplicationIdentifier { get; set; }
    public string ApplicationEnvironment { get; set; }
    public int HealthCheckTimeoutInSeconds { get; set; }
}
