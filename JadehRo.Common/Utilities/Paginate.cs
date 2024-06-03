using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace JadehRo.Common.Utilities;

public class Paginate
{
    public int Index { get; set; }
    public int Count { get; set; }
    public string Order { get; set; }
    public string Sort { get; set; }

    public static (IQueryable<T>, int) GetPaginatedList<T>(Paginate? paginate, IQueryable<T> models)
    {
        if (paginate == null)
            return (models.Take(20), models.Count());

        if (string.IsNullOrEmpty(paginate.Sort))
        {
            var prop = TypeDescriptor.GetProperties(typeof(T))
                .Find("CreatedDateTime", true);

            models = models.OrderByDescending(x => EF.Property<T>(x!, prop!.Name));
        }
        else
        {
            var prop = TypeDescriptor.GetProperties(typeof(T)).Find(paginate.Sort, true);
            models = paginate.Order!.ToLower() == "asc"
                ? models.OrderBy(x => EF.Property<T>(x!, prop!.Name))
                : models.OrderByDescending(x => EF.Property<T>(x!, prop!.Name));
        }

        var count = models.Count();

        if (paginate.Count is > 0 and <= 50 && paginate.Index >= 0)
        {
            models = models
                .Skip(paginate.Count * paginate.Index)
                .Take(paginate.Count);
        }
        else
        {
            models = models.Take(20);
        }

        return (models, count);
    }
}