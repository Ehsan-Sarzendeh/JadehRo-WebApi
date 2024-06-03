using JadehRo.Api.Infrastructure.Api;
using JadehRo.Database.Entities.Trip;
using JadehRo.Service.TripService;
using JadehRo.Service.TripService.Dto;
using JadehRo.Service.TripService.Paginates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JadehRo.Api.Controllers;

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

    [HttpPut("Cancel/{tripId}")]
    public ApiResult CancelTrip(long tripId)
    {
        _tripService.CancelTrip(UserId!.Value, tripId);
        return Ok();
    }

    [HttpGet("{tripId}"), AllowAnonymous]
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

    [HttpGet("Pending"), AllowAnonymous]
    public ApiResult<IList<GetTripListDto>> GetPendingTrips([FromQuery] TripPaginate paginate)
    {
        var (userDto, count) = _tripService.GetPendingTrips(paginate);
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

    [HttpGet("SeenTrip")]
    public ApiResult<DriverInfo> SeenTrip(long tripId)
    {
        var res = _tripService.SeenInfo(UserId!.Value, tripId);
        return Ok(res);
    }

    [HttpPost("SendRequest")]
    public ApiResult SendRequest(long tripId)
    {
       _tripService.SendRequest(UserId!.Value, tripId);
       return Ok();
    }

    [HttpPut("AcceptRequest")]
    public ApiResult AcceptRequest(long tripReqId)
    {
        _tripService.AcceptRequest(tripReqId);
        return Ok();
    }

    [HttpPut("RejectRequest")]
    public ApiResult RejectRequest(long tripReqId)
    {
        _tripService.RejectRequest(tripReqId);
        return Ok();
    }
}