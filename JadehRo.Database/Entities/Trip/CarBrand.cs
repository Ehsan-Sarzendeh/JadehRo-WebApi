using System.ComponentModel.DataAnnotations;
using JadehRo.Database.Entities.Base;

namespace JadehRo.Database.Entities.Trip;

public class CarBrand : BaseEntity
{
    public CarType Type { get; set; }
    public required string Name { get; set; }
}

public enum CarType
{
    [Display(Name = "سواری")]
    Car = 1,
    [Display(Name = "ون")]
    Van = 2
}