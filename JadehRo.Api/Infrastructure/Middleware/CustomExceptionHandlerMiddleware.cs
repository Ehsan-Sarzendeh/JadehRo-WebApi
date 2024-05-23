using System.Net;
using JadehRo.Api.Infrastructure.Api;

namespace JadehRo.Api.Infrastructure.Middleware;

public static class CustomExceptionHandlerMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}

public class CustomExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

    public CustomExceptionHandlerMiddleware(RequestDelegate next,
        IWebHostEnvironment env,
        ILogger<CustomExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        string message = null;
        var httpStatusCode = HttpStatusCode.InternalServerError;
        var apiStatusCode = ApiResultStatusCode.ServerError;

        try
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.BadRequest)
            {
                _logger.LogError("خطا در ثبت اطلاعات ورودی");
            }
        }
        catch (BadHttpRequestException exception)
        {
            _logger.LogError(exception, exception.Message);
        }
        catch (AppException exception)
        {
            _logger.LogWarning(exception, exception.Message);
            httpStatusCode = exception.HttpStatusCode;
            apiStatusCode = exception.ApiStatusCode;

            if (_env.IsDevelopment())
            {
                var dic = new Dictionary<string, string>
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace,
                };
                if (exception.InnerException != null)
                {
                    dic.Add("InnerException.Exception", exception.InnerException.Message);
                    dic.Add("InnerException.StackTrace", exception.InnerException.StackTrace);
                }

                if (exception.AdditionalData != null)
                    dic.Add("AdditionalData", JsonConvert.SerializeObject(exception.AdditionalData));

                message = JsonConvert.SerializeObject(dic);
            }
            else
            {
                message = exception.Message;
            }

            await WriteToResponseAsync();
        }
        catch (DbUpdateConcurrencyException exception)
        {
            _logger.LogError(exception, exception.Message);
            message = "لطفا دوباره تلاش کنید";
            await WriteToResponseAsync();
        }
        catch (SecurityTokenExpiredException exception)
        {
            _logger.LogWarning(exception, exception.Message);
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogWarning(exception, exception.Message);
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            if (_env.IsDevelopment())
            {
                var dic = new Dictionary<string, string>
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace,
                };
                message = JsonConvert.SerializeObject(dic);
            }
            await WriteToResponseAsync();
        }

        async Task WriteToResponseAsync()
        {
            if (context.Response.HasStarted)
                throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

            var result = new ApiResult(false, apiStatusCode, message);
            var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            context.Response.StatusCode = (int)httpStatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }

        void SetUnAuthorizeResponse(Exception exception)
        {
            httpStatusCode = HttpStatusCode.Unauthorized;
            apiStatusCode = ApiResultStatusCode.UnAuthorized;

            if (_env.IsDevelopment())
            {
                var dic = new Dictionary<string, string>
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace
                };
                if (exception is SecurityTokenExpiredException tokenException)
                    dic.Add("Expires", tokenException.Expires.ToString());

                message = JsonConvert.SerializeObject(dic);
            }
        }
    }
}