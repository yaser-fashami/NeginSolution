using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class DiscountTariffConfig : IEntityTypeConfiguration<DiscountTariff>
{
    public void Configure(EntityTypeBuilder<DiscountTariff> builder)
    {
        builder.ToTable("DiscountTariff", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(100);
        builder.Property(x => x.DiscountPercent).IsRequired();

    }
}
