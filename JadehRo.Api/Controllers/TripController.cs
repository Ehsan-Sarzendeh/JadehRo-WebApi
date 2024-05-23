using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadehRo.Api.Controllers;

[Authorize]
public class TripController : BaseController
{
    private readonly ITripService _tripService;

    public TripController(ITripService tripService)
    {
        _tripService = tripService;
    }

    [HttpGet("CarBrands/{carType}"), AllowAnonymous]
    public ApiResult<IList<CarBrandDto>> GetCarBrands(CarType? carType)
    {
       var res = _tripService.GetCarBrands(carType);
       return Ok(res);
    }

    [HttpPost("Add")]
    public ApiResult AddTrip(AddTripDto dto)
    {
        _tripService.AddTrip(dto);
        return Ok();
    }

    [HttpPut("Edit")]
    public ApiResult EditTrip(EditTripDto dto)
    {
        _tripService.EditTrip(dto);
        return Ok();
    }

    [HttpPut("Delete/{tripId:long}")]
    public ApiResult DeleteTrip(long tripId)
    {

        _tripService.DeleteTrip(UserId!.Value, tripId);
        return Ok();
    }

    [HttpPut("Accept/{tripId:long}")]
    public ApiResult AcceptTrip(long tripId)
    {
        _tripService.AcceptTrip(tripId);
        return Ok();
    }

    [HttpPut("Reject/{tripId:long}")]
    public ApiResult RejectTrip(long tripId)
    {
        _tripService.RejectTrip(tripId);
        return Ok();
    }

    [HttpGet("{tripId:long}"), AllowAnonymous]
    public ApiResult<GetTripDto> GetTrip(long tripId)
    {
        var trip = _tripService.GetTrip(tripId);
        return trip;
    }

    [HttpGet]
    public ApiResult<IList<GetTripListDto>> GetAllTrips([FromQuery] TripPaginate paginate)
    {
        var (userDto, count) = _tripService.GetAllTrips(paginate);
        return new ApiResult<IList<GetTripListDto>>
        {
            TotalRecord = count,
            Data = userDto
        };
    }

    [HttpGet("Accepted"), AllowAnonymous]
    public ApiResult<IList<GetTripListDto>> GetAcceptedTrips([FromQuery] TripPaginate paginate)
    {
        var (userDto, count) = _tripService.GetAcceptedTrips(paginate);
        return new ApiResult<IList<GetTripListDto>>()
        {
            TotalRecord = count,
            Data = userDto
        };
    }

    [HttpGet("Driver")]
    public ApiResult<IList<GetTripListDto>> GetDriverTrips([FromQuery] TripPaginate paginate)
    {
        var (userDto, count) = _tripService.GetDriverTrips(UserId!.Value, paginate);
        return new ApiResult<IList<GetTripListDto>>()
        {
            TotalRecord = count,
            Data = userDto
        };
    }

    [HttpGet("SeenDriverInfo")]
    public ApiResult<DriverInfo> GetTripDriverInfo(long tripId)
    {
        var res = _tripService.GetTripDriverInfo(UserId!.Value, tripId);
        return res;
    }
}