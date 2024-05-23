using System.ComponentModel.DataAnnotations;
using JadehRo.Database.Entities.Base;
using JadehRo.Database.Entities.Common;

namespace JadehRo.Database.Entities.Trip;

public class Trip : BaseAuditableEntity
{
    public TripStatus Status { get; set; }

    public int CarModel { get; set; }

    public int Capacity { get; set; }

    public DateTime MoveDateTime { get; set; }

    public MoneyType MoneyType { get; set; }
    public int? Money { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public required long CarBrandId { get; set; }
    public required CarBrand CarBrand { get; set; }

    public required long SourceId { get; set; }
    public required CountryDivision Source { get; set; }

    public required long DestinationId { get; set; }
    public required CountryDivision Destination { get; set; }
}

public enum MoneyType
{
    Free = 1,
    Agreement = 2,
    SpecificPrice = 3
}

public enum TripStatus
{
    Pending = 1,
    Finish = 2,
    Cancel = 3
}