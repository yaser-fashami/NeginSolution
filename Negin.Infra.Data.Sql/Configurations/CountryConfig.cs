using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negin.Infra.Data.Sql.Configurations;

internal class CountryConfig : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable("Countries", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Symbol).IsRequired().HasMaxLength(3);
        builder.Property(x => x.FlagName).HasMaxLength(50);

        #region Navigation
        builder.HasMany(c => c.Nationalities).WithOne(d => d.Nationality).HasForeignKey(s => s.NationalityId);
        builder.HasMany(c => c.Flags).WithOne(d => d.Flag).HasForeignKey(s => s.FlagId);
        builder.HasMany(c => c.Ports).WithOne(d => d.Country).HasForeignKey(s => s.CountryId);

        #endregion

    }
}
