using JadehRo.Service.Infrastructure;

namespace JadehRo.Service.TripService.Dto;

public class CarBrandDto : BaseDto<CarBrandDto, CarBrand>
{
    public CarType Type { get; set; }
    public string TypeName => Type.ToDisplay();
    public string Name { get; set; }
}