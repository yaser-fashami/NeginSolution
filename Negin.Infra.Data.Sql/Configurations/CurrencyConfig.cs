
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class CurrencyConfig : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.ToTable("Currencies", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.ForeignDollerRate).IsRequired().HasColumnType("int");
        builder.Property(x=>x.PersianDollerRate).IsRequired().HasColumnType("int");
        builder.Property(x=>x.Date).IsRequired();
    }
}
