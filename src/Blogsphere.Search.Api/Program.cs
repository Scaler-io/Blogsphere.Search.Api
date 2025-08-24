using Serilog;

var builder = WebApplication.CreateBuilder(args);

var logger = Logging.GetLogger(builder.Configuration);
builder.Services.AddSingleton(logger);
builder.Host.UseSerilog(logger);

builder.Services.AddApplicationServices(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddConfiguration(builder.Configuration);
builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddHttpClientFactoryConfigurations(builder.Configuration);

#if DEBUG
builder.WebHost.UseUrls("http://localhost:5002");
#endif

var app = builder.Build();

app.UseApplicationPipeline();

try
{
    await app.RunAsync();
}
finally
{
    Log.CloseAndFlush();
}
