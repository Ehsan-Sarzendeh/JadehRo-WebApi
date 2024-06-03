using System.ComponentModel;
using JadehRo.Common.Utilities;
using JadehRo.Database.Entities.Users;
using Microsoft.EntityFrameworkCore;

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

            if (string.IsNullOrEmpty(paginate.Sort))
                models = models.OrderByDescending(x => x.FullName);
            else
            {
                var prop = TypeDescriptor.GetProperties(typeof(User)).Find(paginate.Sort, true);
                models = paginate.Order.ToLower() == "asc"
                    ? models.OrderBy(x => EF.Property<User>(x, prop.Name))
                    : models.OrderByDescending(x => EF.Property<User>(x, prop.Name));
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