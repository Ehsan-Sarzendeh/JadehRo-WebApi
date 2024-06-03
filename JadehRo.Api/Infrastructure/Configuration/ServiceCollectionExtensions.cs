using System.Net;
using System.Security.Claims;
using System.Text;
using JadehRo.Common.Exceptions;
using JadehRo.Common.Extensions;
using JadehRo.Common.Utilities;
using JadehRo.Database.Context;
using JadehRo.Database.Entities.Users;
using JadehRo.Database.Interceptors;
using JadehRo.Database.Repositories;
using JadehRo.Database.Repositories.RepositoryWrapper;
using JadehRo.Service.SmsService.Panels;
using JadehRo.Service.SmsService.Panels.IppanelPanel;
using JadehRo.Service.SmsService.Panels.KavenegarPanel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace JadehRo.Api.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static void AddDbContext(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlServer"));

            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
            options.AddInterceptors(new PropertyAuditSaveChangesInterceptor(httpContextAccessor));

            options.ConfigureWarnings(builder =>
            {
                builder.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning);
            });
        });
    }

    public static void AddJwtAuthentication(this IServiceCollection services, JwtSettings jwtSettings)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero, // default: 5 min
                RequireSignedTokens = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKey),

                RequireExpirationTime = true,
                ValidateLifetime = true,

                ValidateAudience = true, //default : false
                ValidAudience = jwtSettings.Audience,

                ValidateIssuer = true, //default : false
                ValidIssuer = jwtSettings.Issuer,
            };

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = validationParameters;
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"];

                    // If the request is for our hub...
                    var path = context.HttpContext.Request.Path;
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        // Read the token out of the query string
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context => throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized),
                OnTokenValidated = async context =>
                {
                    var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<User>>();
                    var repository = context.HttpContext.RequestServices.GetRequiredService<IRepositoryWrapper>();

                    var claimsIdentity = context.Principal!.Identity as ClaimsIdentity;
                    if (claimsIdentity!.Claims.Any() != true)
                        context.Fail("This token has no claims.");

                    var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                    if (!securityStamp.HasValue())
                        context.Fail("This token has no secuirty stamp");

                    var userId = claimsIdentity.GetUserId<long>();

                    var user = await repository.User.GetByIdAsync(context.HttpContext.RequestAborted, userId);

                    if (user.SecurityStamp != securityStamp)
                        context.Fail("Token secuirty stamp is not valid.");

                    var validatedUser = await signInManager.ValidateSecurityStampAsync(context.Principal);
                    if (validatedUser == null)
                        context.Fail("Token secuirty stamp is not valid.");

                    if (!user.IsActive)
                        context.Fail("User is not active.");

                },
                OnChallenge = context =>
                {
                    if (context.AuthenticateFailure != null)
                        throw new AppException(ApiResultStatusCode.UnAuthorized, "Authenticate failure.", HttpStatusCode.Unauthorized, context.AuthenticateFailure, null);

                    throw new AppException(ApiResultStatusCode.UnAuthorized, "You are unauthorized to access this resource.", HttpStatusCode.Unauthorized);
                }
            };
        });
    }

    public static void AddCustomIdentity(this IServiceCollection services, IdentitySettings settings)
    {
        services.AddIdentity<User, Role>(identityOptions =>
        {
            //Password Settings
            identityOptions.Password.RequireDigit = settings.PasswordRequireDigit;
            identityOptions.Password.RequiredLength = settings.PasswordRequiredLength;
            identityOptions.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanumeric; //#@!
            identityOptions.Password.RequireUppercase = settings.PasswordRequireUppercase;
            identityOptions.Password.RequireLowercase = settings.PasswordRequireLowercase;

            //UserName Settings
            identityOptions.User.RequireUniqueEmail = settings.RequireUniqueEmail;


            //Singin Settings
            //identityOptions.SignIn.RequireConfirmedEmail = false;
            //identityOptions.SignIn.RequireConfirmedPhoneNumber = false;

            //Lockout Settings
            identityOptions.Lockout.MaxFailedAccessAttempts = 5;
            identityOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            identityOptions.Lockout.AllowedForNewUsers = true;

        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AdminUser,
                policy => policy.RequireClaim("userType",
                    Convert.ToString((int)UserType.Admin)));

            options.AddPolicy(Policies.SupporterUser,
                policy => policy.RequireClaim("userType",
                    Convert.ToString((int)UserType.Supporter)));

            options.AddPolicy(Policies.DriverUser,
                policy => policy.RequireClaim("userType",
                    Convert.ToString((int)UserType.Driver)));

            options.AddPolicy(Policies.PassengerUser,
                policy => policy.RequireClaim("userType",
                    Convert.ToString((int)UserType.Passenger)));
        });
    }

    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        var scopedType = typeof(IScopedDependency);
        var singletonType = typeof(ISingletonDependency);
        var transientType = typeof(ITransientDependency);


        var scopedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetExportedTypes())
            .Where(p => scopedType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
            .ToList();

        var singletonTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetExportedTypes())
            .Where(p => singletonType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
            .ToList();

        var transientTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetExportedTypes())
            .Where(p => transientType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
            .ToList();


        foreach (var type in scopedTypes)
        {
            var iType = type.GetInterface("I" + type.Name);
            services.AddScoped(iType, type);
        }

        foreach (var type in singletonTypes)
        {
            var iType = type.GetInterface("I" + type.Name);
            services.AddSingleton(iType, type);
        }

        foreach (var type in transientTypes)
        {
            var iType = type.GetInterface("I" + type.Name);
            services.AddTransient(iType, type);
        }

        #region SMS

        services.AddTransient<IppanelPanelService>();
        services.AddTransient<KavenegarPanelService>();
        services.AddTransient<Func<SmsPanelType, ISmsPanelService>>(provider => key =>
        {
            return key switch
            {
                SmsPanelType.Ippanel => provider.GetService<IppanelPanelService>(),
                SmsPanelType.Kavenegar => provider.GetService<KavenegarPanelService>(),
                _ => throw new ArgumentOutOfRangeException(nameof(key), key, null)
            };
        });

        #endregion
    }

    public static void AddSwagger(this IServiceCollection services, SwaggerSettings swaggerSettings)
    {
        services.AddSwaggerGen(options =>
        {
            options.OperationFilter<UnauthorizedResponsesOperationFilter>(true, "OAuth2");

            options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "API V1" });

            options.AddSecurityDefinition("OAuth2", new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri(swaggerSettings.LoginUrl),
                    }
                }
            });

            options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Name = "ApiKey",
                Description = "Api Key for using web servisec"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
                    },
                    new string[] { }
                }
            });

        });
    }

    public static IServiceCollection AddLazyResolution(this IServiceCollection services)
    {
        return services.AddTransient(
            typeof(Lazy<>),
            typeof(LazilyResolved<>));
    }

    private class LazilyResolved<T> : Lazy<T>
    {
        public LazilyResolved(IServiceProvider serviceProvider)
            : base(serviceProvider.GetRequiredService<T>)
        {
        }
    }
}

public class UnauthorizedResponsesOperationFilter : IOperationFilter
{
    private readonly bool _includeUnauthorizedAndForbiddenResponses;
    private readonly string _schemeName;

    public UnauthorizedResponsesOperationFilter(bool includeUnauthorizedAndForbiddenResponses, string schemeName = "Bearer")
    {
        _includeUnauthorizedAndForbiddenResponses = includeUnauthorizedAndForbiddenResponses;
        _schemeName = schemeName;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var filters = context.ApiDescription.ActionDescriptor.FilterDescriptors;
        var metadta = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        var hasAnonymous = filters.Any(p => p.Filter is AllowAnonymousFilter) || metadta.Any(p => p is AllowAnonymousAttribute);
        if (hasAnonymous) return;

        var hasAuthorize = filters.Any(p => p.Filter is AuthorizeFilter) || metadta.Any(p => p is AuthorizeAttribute);
        if (!hasAuthorize) return;

        if (_includeUnauthorizedAndForbiddenResponses)
        {
            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });
        }

        operation.Security.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Scheme = _schemeName,
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "OAuth2" }
                },
                Array.Empty<string>() //new[] { "readAccess", "writeAccess" }
            }
        });
    }
}