using JadehRo.Database.Entities.Trip;
using System.ComponentModel.DataAnnotations;
using JadehRo.Service.Infrastructure;

namespace JadehRo.Service.TripService.Dto;

public class GetTripReqDto : BaseDto<GetTripReqDto, TripReq>
{
	public TripReqStatus Status { get; set; }

	public string ReqDateTime { get; set; }
	public string AcceptOrRejectDateTime { get; set; }

	public int PersonCount { get; set; }

    public string SourcePath { get; set; }
    public double SourceLatitude { get; set; }
    public double SourceLongitude { get; set; }

    public string DestinationPath { get; set; }
    public double DestinationLatitude { get; set; }
    public double DestinationLongitude { get; set; }

    public string ReqDescription { get; set; }
	public string AcceptOrRejectDescription { get; set; }

	public string UserFullName { get; set; }
    public string UserPhoneNumber { get; set; }

    public GetTripListDto Trip { get; set; }
}

public class AddTripReqDto
{
	public long TripId { get; set; }

	public int PersonCount { get; set; }

    [StringLength(1000)]
    public string SourcePath { get; set; }
    public double SourceLatitude { get; set; }
    public double SourceLongitude { get; set; }

    [StringLength(1000)]
    public string DestinationPath { get; set; }
    public double DestinationLatitude { get; set; }
    public double DestinationLongitude { get; set; }

    [StringLength(1000)]
	public string ReqDescription { get; set; }
}

public class AcceptOrRejectTripReqDto
{
	public long Id { get; set; }

	[StringLength(1000)]
	public string AcceptOrRejectDescription { get; set; }
}