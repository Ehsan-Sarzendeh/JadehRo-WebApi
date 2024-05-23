using JadehRo.Database.Context;
using JadehRo.Database.Entities.Common;
using JadehRo.Database.Entities.Trip;
using JadehRo.Database.Entities.Users;
using Microsoft.Extensions.DependencyInjection;

namespace JadehRo.Database.Repositories.RepositoryWrapper;

public class RepositoryWrapper(
    AppDbContext dbContext,
    IServiceProvider services,
    IRepository<User> user,
    IRepository<Role> role,
    IRepository<UserRole> userRole,
    IRepository<Option> option,
    IRepository<CountryDivision> countryDivisions,
    IRepository<VerifyCode> verifyCode,
    IRepository<CarBrand> carBrand,
    IRepository<Trip> trip,
    IRepository<TripReq> tripReq)
    : IRepositoryWrapper
{
    public void Save()
    {
        dbContext.SaveChanges();
    }

    public IRepository<User> User => user ??= services.GetRequiredService<IRepository<User>>();
    public IRepository<Role> Role => role ??= services.GetRequiredService<IRepository<Role>>();
    public IRepository<UserRole> UserRole => userRole ??= services.GetRequiredService<IRepository<UserRole>>();
    public IRepository<Option> Option => option ??= services.GetRequiredService<IRepository<Option>>();
    public IRepository<CountryDivision> CountryDivision => countryDivisions ??= services.GetRequiredService<IRepository<CountryDivision>>();
    public IRepository<VerifyCode> VerifyCode => verifyCode ??= services.GetRequiredService<IRepository<VerifyCode>>();
    public IRepository<CarBrand> CarBrand => carBrand ??= services.GetRequiredService<IRepository<CarBrand>>();
    public IRepository<Trip> Trip => trip ??= services.GetRequiredService<IRepository<Trip>>();
    public IRepository<TripReq> TripReq => tripReq ??= services.GetRequiredService<IRepository<TripReq>>();
}