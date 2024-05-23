using JadehRo.Service.TripService.Dto;
using JadehRo.Service.TripService.Paginates;

namespace JadehRo.Service.TripService;

public interface ITripService : IScopedDependency
{
    public IList<CarBrandDto> GetCarBrands(CarType? carType);

    public void AddTrip(AddTripDto dto);
    public void EditTrip(EditTripDto dto);
    public void DeleteTrip(long userId, long tripId);
    public void AcceptTrip(long tripId);
    public void RejectTrip(long tripId);

    public (IList<GetTripListDto>, int) GetAllTrips(TripPaginate paginate);
    public (IList<GetTripListDto>, int) GetAcceptedTrips(TripPaginate paginate);
    public (IList<GetTripListDto>, int) GetDriverTrips(long driverId, TripPaginate paginate);
    public GetTripDto GetTrip(long tripId);

    public DriverInfo GetTripDriverInfo(long userId, long tripId);
}