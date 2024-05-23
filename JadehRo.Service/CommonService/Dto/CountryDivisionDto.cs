using JadehRo.Database.Entities.Common;
using JadehRo.Service.Infrastructure;

namespace JadehRo.Service.CommonService.Dto;

public class CountryDivisionDto : BaseDto<CountryDivisionDto , CountryDivision>
{
    public string Name { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}