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

	public string Address { get; set; }
	public string ReqDescription { get; set; }
	public string AcceptOrRejectDescription { get; set; }

	public string UserFullName { get; set; }

	public GetTripListDto Trip { get; set; }
}

public class AddTripReqDto
{
	public long TripId { get; set; }

	public int PersonCount { get; set; }

	[StringLength(1000)]
	public string Address { get; set; }

	[StringLength(1000)]
	public string ReqDescription { get; set; }
}

public class AcceptOrRejectTripReqDto
{
	public long Id { get; set; }

	[StringLength(1000)]
	public string AcceptOrRejectDescription { get; set; }
}