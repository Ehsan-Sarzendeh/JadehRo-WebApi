using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using JadehRo.Database.Entities.Base;
using JadehRo.Database.Entities.Common;

namespace JadehRo.Database.Entities.Trip;

public class Trip : BaseAuditableEntity
{
    public TripStatus Status { get; set; }

    public int CarModel { get; set; }

    public int Capacity { get; set; }

    public int FillCapacity { get; set; }

    [NotMapped] public int RemainingCapacity => Capacity - FillCapacity;

    public DateTime MoveDateTime { get; set; }

    public MoneyType MoneyType { get; set; }
    public int? Money { get; set; }

    [StringLength(1000)]
    public string Description { get; set; }

    public long CarBrandId { get; set; }
    public CarBrand CarBrand { get; set; }

    public long SourceId { get; set; }
    public CountryDivision Source { get; set; }

    public long DestinationId { get; set; }
    public CountryDivision Destination { get; set; }
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