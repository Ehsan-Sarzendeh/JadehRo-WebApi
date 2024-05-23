using System.Reflection;
using AutoMapper;
using JadehRo.Service.CommonService.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace JadehRo.Service.Infrastructure.CustomMapping;

public static class AutoMapperConfiguration
{
    public static void InitializeAutoMapper(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddAutoMapper(config =>
        {
            config.AddCustomMappingProfile();
            config.AddSpecialMapperProfile();

        }, assemblies);
    }

    public static void AddCustomMappingProfile(this IMapperConfigurationExpression config)
    {
        config.AddCustomMappingProfile(typeof(UploadFileDto).Assembly);
    }

    public static void AddCustomMappingProfile(this IMapperConfigurationExpression config, params Assembly[] assemblies)
    {
        var allTypes = assemblies.SelectMany(a => a.ExportedTypes);

        var list = allTypes.Where(type =>
                type is { IsClass: true, IsAbstract: false } &&
                type.GetInterfaces().Contains(typeof(IHaveCustomMapping)))
            .Select(type => (IHaveCustomMapping)Activator.CreateInstance(type)!);

        var profile = new CustomMappingProfile(list);

        config.AddProfile(profile);
    }
}