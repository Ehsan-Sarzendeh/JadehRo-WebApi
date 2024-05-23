using AutoMapper;
using JadehRo.Common.Utilities;
using JadehRo.Database.Entities.Common;
using JadehRo.Service.Infrastructure.CustomMapping.SpecificConvertors;
using JadehRo.Service.OptionServices.Dto;

namespace JadehRo.Service.Infrastructure.CustomMapping;

public static class SpecialMapperConfiguration
{
    public static void AddSpecialMapperProfile(this IMapperConfigurationExpression config)
    {
        #region Convert with Convertor


        config.CreateMap<Dictionary<string, string>, string>().ConvertUsing(new AutoMapperTypeConvertors.DictionaryToStringTypeConvertor());
        config.CreateMap<string, Dictionary<string, string>>().ConvertUsing(new AutoMapperTypeConvertors.StringToDictionaryTypeConvertor());

        config.CreateMap<IEnumerable<Option>, OptionDto>().ConvertUsing(new OptionToDtoConvertor<Option, OptionDto>());
        config.CreateMap<OptionDto, IEnumerable<Option>>().ConvertUsing(new DtoToOptionConvertor<OptionDto, Option>());

        #endregion

        #region Convert with Expression

        config.CreateMap<DateTime?, string?>().ConvertUsing(src => src == null ? null : src.Value.ToStringDateTime());
        config.CreateMap<DateTime, string>().ConvertUsing(src => src.ToStringDateTime());
        config.CreateMap<string, DateTime>().ConvertUsing(src => src.ToDateTime());

        config.CreateMap<TimeSpan, string>().ConvertUsing(src => src.ToStringTime());
        config.CreateMap<string, TimeSpan>().ConvertUsing(src => src.ToTimeSpan());

        config.CreateMap<DateOnly, string>().ConvertUsing(src => src.ToStringDate());

        config.CreateMap<IList<int>, int>().ConvertUsing(src => src.ToInteger());
        config.CreateMap<int, IList<int>>().ConvertUsing(src => src.ToBinaries());

        #endregion
    }
}