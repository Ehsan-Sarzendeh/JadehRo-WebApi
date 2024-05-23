using AutoMapper;
using AutoMapper.Internal;
using JadehRo.Database.Entities.Base;
using Newtonsoft.Json;

namespace JadehRo.Service.Infrastructure.CustomMapping.SpecificConvertors;

public class OptionToDtoConvertor<TModel, TDto> : ITypeConverter<IEnumerable<TModel>, TDto> where TModel : BaseOptionEntity
{
    public TDto Convert(IEnumerable<TModel> source, TDto destination, ResolutionContext context)
    {
        var models = source;
        var dic = models.ToDictionary(model => model.Key, model => model.Value);

        return DeserializeDicObject(dic!);
    }

    private static TDto DeserializeDicObject(Dictionary<string, string> dic)
    {
        var serializedDic = JsonConvert.SerializeObject(dic);

        serializedDic = CorrectSerializedObject(serializedDic);

        var result = JsonConvert.DeserializeObject<TDto>(serializedDic);

        return result!;
    }

    private static string CorrectSerializedObject(string str)
    {
        str = CorrectSerializedObject("\"[", "]\"", str, 1);
        str = CorrectSerializedObject("\"{", "}\"", str, 1);

        return str;
    }

    private static string CorrectSerializedObject(string leftOldValue, string rightOldValue, string str, int index)
    {
        if (str[index..].IndexOf(leftOldValue, StringComparison.Ordinal) > -1
            && str[index..].IndexOf(rightOldValue, StringComparison.Ordinal) > -1)
        {
            var start = str[index..].IndexOf(leftOldValue, StringComparison.Ordinal) + index;
            var end = str[index..].IndexOf(rightOldValue, StringComparison.Ordinal) + index;

            if (end < start)
                return CorrectSerializedObject(leftOldValue, rightOldValue, str, start);

            var oldSub = str.Substring(start, end - start + leftOldValue.Length);
            var newSub = JsonConvert.DeserializeObject(oldSub);

            str = str.Replace(oldSub, newSub?.ToString());

            return CorrectSerializedObject(leftOldValue, rightOldValue, str, start + 1);
        }

        return str;
    }
}

public class DtoToOptionConvertor<TDto, TModel> : ITypeConverter<TDto, IEnumerable<TModel>> where TModel : BaseOptionEntity, new()
{
    public IEnumerable<TModel> Convert(TDto source, IEnumerable<TModel> destination, ResolutionContext context)
    {
        return source!.GetType()
            .GetProperties().Where(wh => wh.CanWrite)
            .Select(x => new TModel
            {
                Key = x.Name,
                Value = ((x.PropertyType.IsClass && x.PropertyType != typeof(string)) || x.PropertyType.IsCollection())
                    ? JsonConvert.SerializeObject(x.GetValue(source, null))
                    : x.GetValue(source, null)?.ToString(),
            })
            .ToList();
    }
}