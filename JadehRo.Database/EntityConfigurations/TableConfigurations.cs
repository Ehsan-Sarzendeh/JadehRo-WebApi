using JadehRo.Database.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JadehRo.Database.EntityConfigurations;

public class CountryDivisionConfiguration : IEntityTypeConfiguration<CountryDivision>
{
    public void Configure(EntityTypeBuilder<CountryDivision> builder)
    {
        builder.HasKey(key => key.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();
    }
}