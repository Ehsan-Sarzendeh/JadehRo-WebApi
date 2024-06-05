using JadehRo.Common.Utilities;
using JadehRo.Database.Entities.Trip;
using JadehRo.Service.TripService.Dto;
using JadehRo.Service.TripService.Paginates;

namespace JadehRo.Service.TripService;

public interface ITripService : IScopedDependency
{
    public IList<CarBrandDto> GetCarBrands(CarType? carType);

    public void AddTrip(AddTripDto dto);
    public void EditTrip(EditTripDto dto);
    public void FinishTrip(long userId, long tripId);
	public void CancelTrip(long userId, long tripId);

    public (IList<GetTripListDto>, int) GetAllTrips(TripPaginate paginate);
    public (IList<GetTripListDto>, int) GetPendingTrips(TripPaginate paginate);
    public (IList<GetTripListDto>, int) GetDriverTrips(long driverId, TripPaginate paginate);
    public GetTripDto GetTrip(long tripId);

    public (IList<GetTripReqDto>, int) GetTripRequests(long tripId, TripReqPaginate paginate);
    public (IList<GetTripReqDto>, int) GetPassengerRequests(long passengerId, TripReqPaginate paginate);
	public void SendRequest(long userId, AddTripReqDto dto);
    public void AcceptRequest(AcceptOrRejectTripReqDto dto);
    public void RejectRequest(AcceptOrRejectTripReqDto dto);
    public void CancelRequest(long tripReqId);
}