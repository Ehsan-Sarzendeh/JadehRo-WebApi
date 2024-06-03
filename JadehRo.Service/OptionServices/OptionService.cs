using AutoMapper;
using JadehRo.Database.Repositories.RepositoryWrapper;
using JadehRo.Service.OptionServices.Dto;

namespace JadehRo.Service.OptionServices;

public class OptionService : IOptionService
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;

    public OptionService(IRepositoryWrapper repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public void EditOption(OptionDto dto)
    {
            
    }

    public OptionDto GetOption()
    {
        var options = _repository.Option.TableNoTracking
            .ToList();

        var optionDto = OptionDto.FromListEntity(_mapper, options);
        return optionDto;
    }
}