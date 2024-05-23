using JadehRo.Service.TripService.Dto;
using JadehRo.Service.TripService.Paginates;

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
        trip.Status = TripStatus.Accept;
        _repository.Trip.Add(trip, true);
    }

    public void EditTrip(EditTripDto dto)
    {
        var trip = _repository.Trip.Table.Single(x => x.Id == dto.Id);
        dto.ToEntity(_mapper, trip);
        _repository.Trip.Update(trip, true);
    }

    public void DeleteTrip(long userId, long tripId)
    {
        var trip = _repository.Trip.Table.Single(x => x.Id == tripId);

        if (userId != trip.CreatedUserId)
            throw new BadRequestException("درخواست نامعتبر");

        trip.Status = TripStatus.Delete;
        _repository.Save();
    }

    public void AcceptTrip(long tripId)
    {
        var trip = _repository.Trip.Table.Single(x => x.Id == tripId);
        trip.Status = TripStatus.Accept;
        _repository.Save();
    }

    public void RejectTrip(long tripId)
    {
        var trip = _repository.Trip.Table.Single(x => x.Id == tripId);
        trip.Status = TripStatus.Reject;
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

    public (IList<GetTripListDto>, int) GetAcceptedTrips(TripPaginate paginate)
    {
        var trips = _repository.Trip.TableNoTracking
            .Include(x => x.Source)
            .Include(x => x.Destination)
            .Include(x => x.CarBrand)
            .Where(x => x.Status == TripStatus.Accept && x.MoveDateTime.Date >= DateTime.Now.Date);

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
            .Where(x => x.CreatedUserId == driverId && x.Status != TripStatus.Delete);

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

    public DriverInfo GetTripDriverInfo(long userId, long tripId)
    {
        var trip = _repository.Trip.Table
            .Include(x => x.CreatedUser)
            .Single(x => x.Id == tripId);

        var driverInfo = new DriverInfo
        {
            PhoneNumber = trip.CreatedUser.PhoneNumber,
            FullName = trip.CreatedUser.FullName,
        };

        trip.ReadCount++;

        var tripReq = new TripReq()
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
}