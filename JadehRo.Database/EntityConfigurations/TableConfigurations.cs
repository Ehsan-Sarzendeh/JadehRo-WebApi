using JadehRo.Database.Entities.Common;
using JadehRo.Database.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadehRo.Database.EntityConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasMany(e => e.UserRoles)
            .WithOne(x => x.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Property(p => p.UserName).IsRequired().HasMaxLength(50);
    }
}

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder
            .HasMany(e => e.UserRoles)
            .WithOne(x => x.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(50);
    }
}

public class CountryDivisionConfiguration : IEntityTypeConfiguration<CountryDivision>
{
    public void Configure(EntityTypeBuilder<CountryDivision> builder)
    {
        builder.HasKey(key => key.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
    }
}