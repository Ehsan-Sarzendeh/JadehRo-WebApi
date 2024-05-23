using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace JadehRo.Api.Infrastructure.ModelBinders;

/// <summary>
/// Ye Ke Model Binder Extensions
/// </summary>
public static class YeKeModelBinderExtensions
{
    /// <summary>
    /// Inserts YeKeModelBinderProvider at the top of the MvcOptions.ModelBinderProviders list.
    /// </summary>
    public static MvcOptions UseYeKeModelBinder(this MvcOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        options.ModelBinderProviders.Insert(0, new YeKeModelBinderProvider());
        return options;
    }
}

/// <summary>
/// Persian Ye Ke Model Binder Provider
/// </summary>
public class YeKeModelBinderProvider : IModelBinderProvider
{
    /// <summary>
    /// Creates an IModelBinder based on ModelBinderProviderContext.
    /// </summary>
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.IsComplexType)
        {
            return null;
        }

        if (context.Metadata.ModelType == typeof(string))
        {
            return new YeKeModelBinder();
        }

#if !NETSTANDARD1_6
        var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
        return new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory);
#else
            return new SimpleTypeModelBinder(context.Metadata.ModelType);
#endif
    }
}

/// <summary>
/// Persian Ye Ke Model Binder
/// </summary>
public class YeKeModelBinder : IModelBinder
{

    /// <summary>
    /// Attempts to bind a model.
    /// </summary>
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

#if !NETSTANDARD1_6
        var logger = bindingContext.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>();
        var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType, logger);
#else
            var fallbackBinder = new SimpleTypeModelBinder(bindingContext.ModelType);
#endif

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
             
            return fallbackBinder.BindModelAsync(bindingContext);
        }
        bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

        var valueAsString = valueProviderResult.FirstValue;
        if (string.IsNullOrWhiteSpace(valueAsString))
        {
            return fallbackBinder.BindModelAsync(bindingContext);
        }

        // var model = valueAsString
        //     .Replace((char)1609, (char)1610)
        //     .Replace((char)1740, (char)1610)
        //     .Replace((char)1706, (char)1603)
        //     .Replace((char)1705, (char)1603)
        //     .Replace((char)1749, (char)1607)
        //     .Replace((char)1729, (char)1607)
        //     .Replace((char)1731, (char)1577)
        //     .Replace((char)1730, (char)1728); 

        var model = valueAsString.CleanString();

        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }
}