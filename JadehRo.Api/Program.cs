using JadehRo.Api.Infrastructure.Configuration;
using JadehRo.Api.Infrastructure.Middleware;
using JadehRo.Api.Infrastructure.Pipeline;
using JadehRo.Common.Utilities;
using NLog;
using NLog.Web;


var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
    var siteSetting = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();

    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Warning);
    builder.Host.UseNLog();

    builder.Services.AddWebServices(builder.Configuration, siteSetting!);
    builder.Services.AddApplicationServices(builder.Configuration, siteSetting!);
    builder.Services.AddInfrastructureServices(builder.Configuration, siteSetting!);

    var app = builder.Build();
    var env = app.Environment;

    app.UseCustomExceptionHandler();

	if (env.IsDevelopment())
        app.UseCors("DevelopedOrigins");

    if (env.IsProduction())
        app.UseCors("PublishedOrigins");

    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();

    app.UseAuthentication();
    app.UseAuthorization();

    app.InitializeDatabase();

    app.MapControllers();
    app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();

}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    LogManager.Shutdown();
}