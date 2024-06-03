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
        var trip = _repository.Trip.Table.Single(x => x.Id == dto.Id);
        dto.ToEntity(_mapper, trip);
        _repository.Trip.Update(trip, true);
    }

    public void CancelTrip(long userId, long tripId)
    {
        var trip = _repository.Trip.Table.Single(x => x.Id == tripId);

        if (userId != trip.CreatedUserId)
            throw new BadRequestException("درخواست نامعتبر");

        trip.Status = TripStatus.Cancel;
        _repository.Save();
    }

    public (IList<GetTripListDto>, int) GetAllTrips(TripPaginate paginate)
    {
        var trips = _repository.Trip.TableNoTracking
            .Include(x => x.Source)
            .Include(x => x.Destination)
            .Include(x => x.CarBrand)
            .AsQueryable();

        (trips, var count) = TripPaginate.GetPaginatedList(paginate, trips, _mapper);

        var result = GetTripListDto.FromEntities(_mapper, trips.ToList());
        return (result, count);
    }

    public (IList<GetTripListDto>, int) GetPendingTrips(TripPaginate paginate)
    {
        var trips = _repository.Trip.TableNoTracking
            .Include(x => x.Source)
            .Include(x => x.Destination)
            .Include(x => x.CarBrand)
            .Where(x => x.Status == TripStatus.Pending && x.MoveDateTime.Date >= DateTime.Now.Date);

        (trips, var count) = TripPaginate.GetPaginatedList(paginate, trips, _mapper);

        var result = GetTripListDto.FromEntities(_mapper, trips.ToList());
        return (result, count);
    }

    public (IList<GetTripListDto>, int) GetDriverTrips(long driverId, TripPaginate paginate)
    {
        var trips = _repository.Trip.TableNoTracking
            .Include(x => x.Source)
            .Include(x => x.Destination)
            .Include(x => x.CarBrand)
            .Where(x => x.CreatedUserId == driverId);

        (trips, var count) = TripPaginate.GetPaginatedList(paginate, trips, _mapper);

        var result = GetTripListDto.FromEntities(_mapper, trips.ToList());
        return (result, count);
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

    public DriverInfo SeenInfo(long userId, long tripId)
    {
        var trip = _repository.Trip.Table
            .Include(x => x.CreatedUser)
            .Single(x => x.Id == tripId);

        var driverInfo = new DriverInfo
        {
            PhoneNumber = trip.CreatedUser.PhoneNumber,
            FullName = trip.CreatedUser.FullName,
        };

        var tripReq = new TripReq
        {
            TripId = trip.Id,
            UserId = userId,
            SeenDateTime = DateTime.Now,
            Status = TripReqStatus.Seen,
        };

        _repository.TripReq.Add(tripReq);
        _repository.Save();

        return driverInfo;
    }

    public void SendRequest(long userId, long tripId)
    {
        var trip = _repository.Trip.TableNoTracking
            .Single(x => x.Id == tripId);

        if (trip.Status != TripStatus.Pending)
            throw new BadRequestException();

        if (trip.RemainingCapacity == 0)
            throw new BadRequestException("ظرفیت سفر تکمیل شده است");

        var existReq = _repository.TripReq.Table
            .SingleOrDefault(x => x.UserId == userId && x.TripId == tripId);

        if (existReq is not null)
        {
            switch (existReq.Status)
            {
                case TripReqStatus.Seen:
                    existReq.Status = TripReqStatus.Pending;
                    break;
                case TripReqStatus.Pending:
                case TripReqStatus.Accept:
                case TripReqStatus.Reject:
                    throw new BadRequestException("درخواست سفر قبلا ارسال شده است");
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        else
        {
            var tripReq = new TripReq
            {
                TripId = tripId,
                UserId = userId,
                SeenDateTime = DateTime.Now,
                Status = TripReqStatus.Pending,
            };

            _repository.TripReq.Add(tripReq);
        }

        _repository.Save();
    }
    public void AcceptRequest(long tripReqId)
    {
        var tripReq = _repository.TripReq.Table
            .Include(x => x.Trip)
            .Single(x => x.Id == tripReqId);

        if (tripReq.Status != TripReqStatus.Pending)
            throw new BadRequestException();

        var acceptCount = _repository.TripReq.TableNoTracking
            .Count(x => x.TripId == tripReq.TripId && x.Status == TripReqStatus.Accept);

        if (tripReq.Trip.Capacity == acceptCount)
            throw new AppException("ظرفیت سفر به اتمام رسیده است");

        tripReq.Status = TripReqStatus.Accept;
        tripReq.Trip.FillCapacity++;

        if (tripReq.Trip.RemainingCapacity == 0)
            tripReq.Trip.Status = TripStatus.Finish;

        _repository.Save();
    }
    public void RejectRequest(long tripReqId)
    {
        var tripReq = _repository.TripReq.Table
            .Single(x => x.Id == tripReqId);

        if (tripReq.Status != TripReqStatus.Pending)
            throw new BadRequestException();

        tripReq.Status = TripReqStatus.Reject;
    }
}