
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class VatTariffConfig : IEntityTypeConfiguration<VatTariff>
{
    public void Configure(EntityTypeBuilder<VatTariff> builder)
    {
        builder.ToTable("VatTariff", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).IsRequired().HasMaxLength(100);
        builder.Property(x => x.Rate).IsRequired();
    }
}
