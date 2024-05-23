using JadehRo.Service.OptionServices.Dto;

namespace JadehRo.Service.OptionServices;

public interface IOptionService : IScopedDependency
{
    void EditOption(OptionDto dto);
    OptionDto GetOption();
}