using JadehRo.Database.Entities.Common;
using JadehRo.Database.Entities.Trip;
using JadehRo.Database.Entities.Users;

namespace JadehRo.Database.Repositories.RepositoryWrapper;

public interface IRepositoryWrapper
{
    void Save();

    IRepository<User> User { get; }
    IRepository<Role> Role { get; }
    IRepository<UserRole> UserRole { get; }
    IRepository<Option> Option { get; }
    IRepository<CountryDivision> CountryDivision { get; }
    IRepository<VerifyCode> VerifyCode { get; }
    IRepository<CarBrand> CarBrand { get; }
    IRepository<Trip> Trip { get; }
    IRepository<TripReq> TripReq { get; }
}