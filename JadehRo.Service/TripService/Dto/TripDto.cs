using System.ComponentModel.DataAnnotations;
using JadehRo.Database.Entities.Trip;
using JadehRo.Service.Infrastructure;
using Newtonsoft.Json;

namespace JadehRo.Service.TripService.Dto;

public class AddTripDto : BaseDto<AddTripDto, Trip>
{
    public int Capacity { get; set; }
    public MoneyType MoneyType { get; set; }
    public int? Money { get; set; }
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
    public int? Money { get; set; }
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
	public TripStatus Status { get; set; }

	public MoneyType MoneyType { get; set; }
	public int? Money { get; set; }
	public string MoveDateTime { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public bool HaveNewReq { get; set; }

	public int Capacity { get; set; }
    public int FillCapacity { get; set; }
    public int RemainingCapacity { get; set; }

    public int CarModel { get; set; }

    public long CarBrandId { get; set; }
    public string CarBrandName { get; set; }

    public long SourceId { get; set; }
    public string SourceName { get; set; }

    public long DestinationId { get; set; }
    public string DestinationName { get; set; }

    public string CreatedDateTime { get; set; }

    [JsonProperty("DriverName")]
    public string CreatedUserFullName { get; set; }

    [JsonProperty("DriverPhoneNumber")]
    public string CreatedUserPhoneNumber { get; set; }
}

public class GetTripDto : BaseDto<GetTripDto, Trip>
{
	public TripStatus Status { get; set; }

	public MoneyType MoneyType { get; set; }
	public int? Money { get; set; }
	public string MoveDateTime { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    public int CarModel { get; set; }

    public bool HaveNewReq { get; set; }

	public int Capacity { get; set; }
    public int FillCapacity { get; set; }
    public int RemainingCapacity { get; set; }

    public long CarBrandId { get; set; }
    public string CarBrandName { get; set; }

    public long SourceId { get; set; }
    public string SourceName { get; set; }

    public long DestinationId { get; set; }
    public string DestinationName { get; set; }

    public string CreatedDateTime { get; set; }

    [JsonProperty("DriverName")]
    public string CreatedUserFullName { get; set; }

    [JsonProperty("DriverPhoneNumber")]
    public string CreatedUserPhoneNumber { get; set; }
}