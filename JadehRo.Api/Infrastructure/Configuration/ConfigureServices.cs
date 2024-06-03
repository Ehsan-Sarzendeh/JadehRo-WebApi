using JadehRo.Api.Infrastructure.ModelBinders;
using JadehRo.Common.Utilities;
using JadehRo.Service.Infrastructure.CustomMapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JadehRo.Api.Infrastructure.Configuration;

public static class ConfigureServices
{
    public static void AddWebServices(this IServiceCollection services, IConfiguration configuration, SiteSettings siteSettings)
    {
        services.AddControllers(options =>
        {
            options.UseYeKeModelBinder();
        }).AddNewtonsoftJson(option =>
        {
            option.SerializerSettings.Formatting = Formatting.None;
            option.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            option.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        });
        services.AddRazorPages();
        services.AddCors(option =>
        {
            option.AddPolicy(name: "PublishedOrigins", policyBuilder =>
            {
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(
                        "http://janamoni.sk.local",
                        "https://localhost:4200",
                        "http://localhost:4200")
                    .AllowCredentials();
            });

            option.AddPolicy(name: "DevelopedOrigins", policyBuilder =>
            {
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(
                        "http://localhost:4200",
                        "https://localhost:4200",
                        "http://localhost:4201",
                        "https://localhost:4201")
                    .AllowCredentials();
            });

        });
        services.AddOptions();
        services.AddLazyResolution();
        services.AddMemoryCache();
        services.AddSwagger(siteSettings.SwaggerSettings);
    }

    public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, SiteSettings siteSettings)
    {
        services.InitializeAutoMapper();
        services.AddDbContext(configuration);
        services.AddCustomIdentity(siteSettings.IdentitySettings);
        services.AddJwtAuthentication(siteSettings.JwtSettings);
    }

    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration, SiteSettings siteSettings)
    {
        services.Configure<SiteSettings>(configuration.GetSection(nameof(SiteSettings)));
        services.AddServices();
    }
}