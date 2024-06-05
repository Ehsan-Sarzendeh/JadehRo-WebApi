using System.ComponentModel;
using JadehRo.Common.Utilities;
using JadehRo.Database.Entities.Trip;
using Microsoft.EntityFrameworkCore;

namespace JadehRo.Service.TripService.Paginates;

public class TripReqPaginate : Paginate
{
    public string Search { get; set; }
    public TripReqStatus? Status { get; set; }

	public static (IQueryable<TripReq>, int) GetPaginatedList(TripReqPaginate paginate, IQueryable<TripReq> models)
    {
        int count;
        if (paginate == null)
        {
            count = models.Count();
            models = models.OrderByDescending(x => x.ReqDateTime).Take(20);
        }
        else
        {
            if (!string.IsNullOrEmpty(paginate.Search))
            {
                models = models.Where(x =>
                    (x.ReqDescription + ", " +
                     x.Id)
                    .Contains(paginate.Search));
            }

            if (paginate.Status is not (null or 0))
            {
                models = models.Where(x => x.Status == paginate.Status);
            }

            count = models.Count();

            if (string.IsNullOrEmpty(paginate.Sort))
                models = models.OrderByDescending(x => x.ReqDateTime);
            else
            {
                var prop = TypeDescriptor.GetProperties(typeof(TripReq)).Find(paginate.Sort, true);
                models = paginate.Order.ToLower() == "asc"
                    ? models.OrderBy(x => EF.Property<TripReq>(x, prop.Name))
                    : models.OrderByDescending(x => EF.Property<TripReq>(x, prop.Name));
            }

            if (paginate.Count != 0)
            {
                models = models
                    .Skip(paginate.Count * paginate.Index)
                    .Take(paginate.Count);
            }
            else
            {
                models = models.Take(20);
            }
        }

        return (models, count);
    }

}