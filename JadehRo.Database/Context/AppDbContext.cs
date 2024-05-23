using JadehRo.Database.Entities.Base;
using JadehRo.Database.Entities.Users;
using JadehRo.Database.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JadehRo.Database.Context;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<User, Role, long, IdentityUserClaim<long>, UserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>(options)
{
    public bool IgnorePreSaveChange { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var entitiesAssembly = typeof(IEntity).Assembly;

        modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly);
        modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly, typeof(AppDbContext).Assembly);
        modelBuilder.AddRestrictDeleteBehaviorConvention();
        modelBuilder.ApplyIsActiveQueryFilters();
    }
}