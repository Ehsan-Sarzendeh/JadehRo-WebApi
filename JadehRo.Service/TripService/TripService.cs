using AutoMapper;
using JadehRo.Common.Exceptions;
using JadehRo.Database.Entities.Trip;
using JadehRo.Database.Repositories.RepositoryWrapper;
using JadehRo.Service.TripService.Dto;
using JadehRo.Service.TripService.Paginates;
using Microsoft.EntityFrameworkCore;

namespace JadehRo.Service.TripService;

public class TripService : ITripService
{
    private readonly IRepositoryWrapper _repository;
    private readonly IMapper _mapper;

    public TripService(IRepositoryWrapper repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public IList<CarBrandDto> GetCarBrands(CarType? carType)
    {
        var brands = carType is null or 0 
            ? _repository.CarBrand.TableNoTracking.ToList() 
            : _repository.CarBrand.TableNoTracking.Where(x => x.Type == carType).ToList();

        return CarBrandDto.FromEntities(_mapper, brands);
    }

    public void AddTrip(AddTripDto dto)
    {
        var moveDate = _mapper.Map<DateTime>(dto.MoveDateTime);

        if (moveDate.Date < DateTime.Now.Date)
            throw new BadRequestException("تاریخ حرکت صحیح نمی باشد");

        var trip = dto.ToEntity(_mapper);

        trip.Status = TripStatus.Pending;
        _repository.Trip.Add(trip, true);
    }
    public void EditTrip(EditTripDto dto)
    {
        var trip = _repository.Trip.Table
            .Include(x => x.Requests)
            .Single(x => x.Id == dto.Id);

        if (trip.Requests.Any())
            throw new BadRequestException("امکان ویرایش سفر به علت داشتن درخواست وجود ندارد");

        dto.ToEntity(_mapper, trip);
        _repository.Trip.Update(trip, true);
    }
    public void FinishTrip(long userId, long tripId)
    {
	    var trip = _repository.Trip.Table.Single(x => x.Id == tripId);

	    if (userId != trip.CreatedUserId)
		    throw new BadRequestException("درخواست نامعتبر");

	    trip.Status = TripStatus.Finish;
	    _repository.Save();
    }
	public void CancelTrip(long userId, long tripId)
    {
        var trip = _repository.Trip.Table.Single(x => x.Id == tripId);

        if (userId != trip.CreatedUserId)
            throw new BadRequestException("درخواست نامعتبر");

        trip.Status = TripStatus.Cancel;
        _repository.Save();
    }

	public GetTripDto GetTrip(long tripId)
	{
		var trip = _repository.Trip.TableNoTracking
			.Include(x => x.Source)
			.Include(x => x.Destination)
			.Include(x => x.CarBrand)
			.Single(x => x.Id == tripId);

		var result = GetTripDto.FromEntity(_mapper, trip);
		return result;
	}
	public (IList<GetTripListDto>, int) GetAllTrips(TripPaginate paginate)
    {
        var trips = _repository.Trip.TableNoTracking
            .AsQueryable();

        (trips, var count) = TripPaginate.GetPaginatedList(paginate, trips, _mapper);

        var result = GetTripListDto.FromEntities(_mapper, trips);
        return (result, count);
    }
    public (IList<GetTripListDto>, int) GetPendingTrips(TripPaginate paginate)
    {
        var trips = _repository.Trip.TableNoTracking
            .Where(x => x.Status == TripStatus.Pending && x.MoveDateTime.Date >= DateTime.Now.Date);

        (trips, var count) = TripPaginate.GetPaginatedList(paginate, trips, _mapper);

        var result = GetTripListDto.FromEntities(_mapper, trips);
        return (result, count);
    }
    public (IList<GetTripListDto>, int) GetDriverTrips(long driverId, TripPaginate paginate)
    {
        var trips = _repository.Trip.TableNoTracking
            .Where(x => x.CreatedUserId == driverId);

        (trips, var count) = TripPaginate.GetPaginatedList(paginate, trips, _mapper);

        var result = GetTripListDto.FromEntities(_mapper, trips);
        return (result, count);
    }

	public (IList<GetTripReqDto>, int) GetTripRequests(long tripId, TripReqPaginate paginate)
	{
		var trip = _repository.Trip.Table
			.Single(x => x.Id == tripId);

		trip.HaveNewReq = false;
        _repository.Save();

	    var trips = _repository.TripReq.TableNoTracking
		    .Where(x => x.TripId == tripId);

        (trips, var count) = TripReqPaginate.GetPaginatedList(paginate, trips);

	    var result = GetTripReqDto.FromEntities(_mapper, trips);
	    return (result, count);
    }
	public (IList<GetTripReqDto>, int) GetPassengerRequests(long passengerId, TripReqPaginate paginate)
	{
		var trips = _repository.TripReq.TableNoTracking
			.Where(x => x.UserId == passengerId);

		(trips, var count) = TripReqPaginate.GetPaginatedList(paginate, trips);

		var result = GetTripReqDto.FromEntities(_mapper, trips);
		return (result, count);
	}
	public void SendRequest(long userId, AddTripReqDto dto)
    {
        var trip = _repository.Trip.Table
			.Single(x => x.Id == dto.TripId);

        if (trip.Status != TripStatus.Pending)
            throw new BadRequestException("ظرفیت سفر تکمیل شده است");

        if (trip.RemainingCapacity < dto.PersonCount)
	        throw new BadRequestException("ظرفیت سفر برای تعداد درخواستی کافی نمی باشد");

		var existReq = _repository.TripReq.Table
            .SingleOrDefault(x => x.UserId == userId && x.TripId == dto.TripId);

        if (existReq != null)
	        throw new BadRequestException("درخواست سفر قبلا ارسال شده است");

        var tripReq = new TripReq
        {
	        TripId = dto.TripId,
	        UserId = userId,
	        ReqDateTime = DateTime.Now,
	        Status = TripReqStatus.Pending,
            SourcePath = dto.SourcePath,
            SourceLatitude = dto.SourceLatitude,
            SourceLongitude = dto.SourceLongitude,
            DestinationPath = dto.DestinationPath,
            DestinationLatitude = dto.DestinationLatitude,
            DestinationLongitude = dto.DestinationLongitude,
            ReqDescription = dto.ReqDescription,
            PersonCount = dto.PersonCount
        };

        trip.HaveNewReq = true;

        _repository.TripReq.Add(tripReq);

		_repository.Save();
    }
    public void AcceptRequest(AcceptOrRejectTripReqDto dto)
    {
        var tripReq = _repository.TripReq.Table
            .Include(x => x.Trip)
            .Single(x => x.Id == dto.Id);

        if (tripReq.Status != TripReqStatus.Pending)
            throw new BadRequestException();

        if (tripReq.Trip.Status != TripStatus.Pending)
	        throw new BadRequestException();

		if (tripReq.Trip.RemainingCapacity < tripReq.PersonCount)
            throw new BadRequestException("ظرفیت سفر برای تایید این درخواست کافی نمی باشد");

        tripReq.Status = TripReqStatus.Accept;
        tripReq.AcceptOrRejectDateTime = DateTime.Now;
        tripReq.AcceptOrRejectDescription = dto.AcceptOrRejectDescription;

		tripReq.Trip.FillCapacity += tripReq.PersonCount;

		if (tripReq.Trip.RemainingCapacity == 0)
		{
			tripReq.Trip.Status = TripStatus.Finish;

			var otherTripRequests = _repository.TripReq.Table
				.Where(x => x.TripId == tripReq.TripId && x.Status == TripReqStatus.Pending && x.Id != tripReq.Id)
				.ToList();

			foreach (var otherTripRequest in otherTripRequests)
			{
				otherTripRequest.Status = TripReqStatus.Reject;
				otherTripRequest.AcceptOrRejectDescription = "اتمام ظرفیت";
			}
		}

		_repository.Save();
    }
    public void RejectRequest(AcceptOrRejectTripReqDto dto)
    {
        var tripReq = _repository.TripReq.Table
            .Single(x => x.Id == dto.Id);

        if (tripReq.Status != TripReqStatus.Pending)
            throw new BadRequestException();

        tripReq.AcceptOrRejectDateTime = DateTime.Now;
        tripReq.Status = TripReqStatus.Reject;
        tripReq.AcceptOrRejectDescription = dto.AcceptOrRejectDescription;

        _repository.Save();
    }
    public void CancelRequest(long tripReqId)
    {
	    var tripReq = _repository.TripReq.Table
		    .Single(x => x.Id == tripReqId);

	    if (tripReq.Status != TripReqStatus.Pending)
		    throw new BadRequestException();

	    tripReq.Status = TripReqStatus.Cancel;

	    _repository.Save();
    }
}