using System.ComponentModel;

namespace JadehRo.Service.TripService.Paginates;

public class TripPaginate : Paginate
{
    public string Search { get; set; }
    public MoneyType? MoneyType { get; set; }
    public TripStatus? Status { get; set; }
    public long? CarBrandId { get; set; }

    public long? SourceProvinceId { get; set; }
    public long? DestinationProvinceId { get; set; }

    public long? SourceId { get; set; }
    public long? DestinationId { get; set; }

    public int? Capacity { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }

    public static (IQueryable<Trip>, int) GetPaginatedList(TripPaginate paginate, IQueryable<Trip> models, IMapper mapper)
    {
        int count;
        if (paginate == null)
        {
            count = models.Count();
            models = models.OrderByDescending(x => x.CreatedDateTime).Take(20);
        }
        else
        {
            if (!string.IsNullOrEmpty(paginate.Search))
            {
                models = models.Where(x =>
                    (x.Destination.Name + ", " +
                     x.Source.Name + ", " +
                     x.CarBrand.Name + ", " +
                     x.Description + ", " +
                     x.Id)
                    .Contains(paginate.Search));
            }

            if (paginate.CarBrandId is not (null or 0))
            {
                models = models.Where(x => x.CarBrandId == paginate.CarBrandId);
            }

            if (paginate.DestinationProvinceId is not (null or 0))
            {
                models = models.Where(x => x.DestinationId.ToString().Substring(0, 2) == paginate.DestinationProvinceId.ToString());
            }

            if (paginate.SourceProvinceId is not (null or 0))
            {
                models = models.Where(x => x.SourceId.ToString().Substring(0, 2) == paginate.SourceProvinceId.ToString());
            }

            if (paginate.DestinationId is not (null or 0))
            {
                models = models.Where(x => x.DestinationId == paginate.DestinationId);
            }

            if (paginate.SourceId is not (null or 0))
            {
                models = models.Where(x => x.SourceId == paginate.SourceId);
            }

            if (paginate.MoneyType is not (null or 0))
            {
                models = models.Where(x => x.MoneyType == paginate.MoneyType);
            }

            if (paginate.Status is not (null or 0))
            {
                models = models.Where(x => x.Status == paginate.Status);
            }

            if (paginate.Capacity != null && paginate.Capacity != 0)
            {
                models = models.Where(x => x.Capacity >= paginate.Capacity);
            }

            if (!string.IsNullOrEmpty(paginate.FromDate))
            {
                var fromDate = mapper.Map<DateTime>(paginate.FromDate);
                models = models.Where(x => x.MoveDateTime >= fromDate);
            }

            if (!string.IsNullOrEmpty(paginate.ToDate))
            {
                var toDate = mapper.Map<DateTime>(paginate.ToDate);
                models = models.Where(x => x.MoveDateTime <= toDate);
            }

            count = models.Count();

            if (string.IsNullOrEmpty(paginate.sort))
                models = models.OrderByDescending(x => x.CreatedDateTime);
            else
            {
                var prop = TypeDescriptor.GetProperties(typeof(Trip)).Find(paginate.sort, true);
                models = paginate.order.ToLower() == "asc"
                    ? models.OrderBy(x => EF.Property<Trip>(x, prop.Name))
                    : models.OrderByDescending(x => EF.Property<Trip>(x, prop.Name));
            }

            if (paginate.count != 0)
            {
                models = models
                    .Skip(paginate.count * paginate.index)
                    .Take(paginate.count);
            }
            else
            {
                models = models.Take(20);
            }
        }

        return (models, count);
    }

}