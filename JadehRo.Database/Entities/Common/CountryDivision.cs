using System.ComponentModel.DataAnnotations;
using JadehRo.Database.Entities.Base;

namespace JadehRo.Database.Entities.Common;

public class CountryDivision : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public long? ParentId { get; set; }
    public CountryDivision Parent { get; set; }

    public ICollection<CountryDivision> Children { get; set; }
}