using JadehRo.Api.Infrastructure.ModelBinders;

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
                        "http://janamoni.sahand-kavir.com:1086",
                        "http://api.janamoni.sahand-kavir.com:3510",
                        "http://109.125.147.221:4510",
                        "http://192.168.168.111:3732",
                        "http://m.janamoni.sk.local",
                        "http://janamoni.sk.local",
                        "http://api.janamuni.ir", 
                        "https://localhost:4200",
                        "http://localhost:4200")
                    .AllowCredentials();
            });

            option.AddPolicy(name: "SahandOrigins", policyBuilder =>
            {
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(
                        "http://janamoni.sahand-kavir.com:1086",
                        "http://api.janamoni.sahand-kavir.com:3510",
                        "http://192.168.168.111:3732",
                        "http://109.125.147.221:4510",
                        "http://janamoni.sk.local", 
                        "http://m.janamoni.sk.local",
                        "http://localhost:4200",
                        "https://localhost:4200",
                        "http://localhost:4201",
                        "https://localhost:4201",
                        "https://localhost:5000")
                    .AllowCredentials();
            });

            option.AddPolicy(name: "DevelopedOrigins", policyBuilder =>
            {
                policyBuilder
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(
                        "http://localhost:4200",
                        "http://janamoni.sahand-kavir.com:1086",
                        "http://api.janamoni.sahand-kavir.com:3510",
                        "http://192.168.168.111:3732",
                        "http://janamoni.sk.local",
                        "http://m.janamoni.sk.local",
                        "http://109.125.147.221:4510",
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
        services.AddHangfire(configuration);
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