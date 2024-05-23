using System.ComponentModel.DataAnnotations;
using JadehRo.Service.Infrastructure;

namespace JadehRo.Service.TripService.Dto;

public class AddTripDto : BaseDto<AddTripDto, Trip>
{
    public int Capacity { get; set; }
    public MoneyType MoneyType { get; set; }
    public string MoveDateTime { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public int CarModel { get; set; }

    public long CarBrandId { get; set; }

    public long SourceId { get; set; }

    public long DestinationId { get; set; }
}

public class EditTripDto : BaseDto<EditTripDto, Trip>
{
    public int Capacity { get; set; }
    public MoneyType MoneyType { get; set; }
    public string MoveDateTime { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public int CarModel { get; set; }

    public long CarBrandId { get; set; }

    public long SourceId { get; set; }

    public long DestinationId { get; set; }
}

public class GetTripListDto : BaseDto<GetTripListDto, Trip>
{
    public int Capacity { get; set; }
    public MoneyType MoneyType { get; set; }
    public string MoveDateTime { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public int CarModel { get; set; }

    public long CarBrandId { get; set; }
    public string CarBrandName { get; set; }

    public long SourceId { get; set; }
    public string SourceName { get; set; }

    public long DestinationId { get; set; }
    public string DestinationName { get; set; }

    public string CreatedDateTime { get; set; }
}

public class GetTripDto : BaseDto<GetTripDto, Trip>
{
    public int Capacity { get; set; }
    public MoneyType MoneyType { get; set; }
    public string MoveDateTime { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public int CarModel { get; set; }

    public long CarBrandId { get; set; }
    public string CarBrandName { get; set; }

    public long SourceId { get; set; }
    public string SourceName { get; set; }

    public long DestinationId { get; set; }
    public string DestinationName { get; set; }

    public string CreatedDateTime { get; set; }
}