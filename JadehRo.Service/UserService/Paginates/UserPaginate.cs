using System.ComponentModel;

namespace JadehRo.Service.UserService.Paginates;

public class UserPaginate : Paginate
{
    public string Search { get; set; }

    public static (IQueryable<User>, int) GetPaginatedList(UserPaginate paginate, IQueryable<User> models)
    {
        int count;
       
        if (paginate == null)
        {
            count = models.Count();
            models =
                models.OrderByDescending(x => x.FullName)
                    .Take(20);
        }
        else
        {
               
            if (!string.IsNullOrEmpty(paginate.Search))
            {
                models = models.Where(x =>
                    (x.FullName + ", "
                                + x.UserName + ", "
                                + x.PhoneNumber + ", " + x.FullName)
                    .Contains(paginate.Search));
            }

            count = models.Count();

            if (string.IsNullOrEmpty(paginate.sort))
                models = models.OrderByDescending(x => x.FullName);
            else
            {
                var prop = TypeDescriptor.GetProperties(typeof(User)).Find(paginate.sort, true);
                models = paginate.order.ToLower() == "asc"
                    ? models.OrderBy(x => EF.Property<User>(x, prop.Name))
                    : models.OrderByDescending(x => EF.Property<User>(x, prop.Name));
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