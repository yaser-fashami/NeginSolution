using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class LoadingDischargeTariffConfig : IEntityTypeConfiguration<LoadingDischargeTariff>
{
    public void Configure(EntityTypeBuilder<LoadingDischargeTariff> builder)
    {
        builder.ToTable("LoadingDischargeTariff", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).HasMaxLength(100);
        builder.Property(x => x.EffectiveDate).IsRequired().HasColumnType("smalldatetime");

        #region Navigation
        builder.HasMany(c => c.LoadingDischargeTariffDetails).WithOne(d => d.LoadingDischargeTariff).HasForeignKey(e => e.LoadingDischargeTariffId);

        #endregion
    }
}

internal class LoadingDischargeTariffDetailsConfig : IEntityTypeConfiguration<LoadingDischargeTariffDetails>
{
    public void Configure(EntityTypeBuilder<LoadingDischargeTariffDetails> builder)
    {
        builder.ToTable("LoadingDischargeTariffDetails", "Basic");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LoadingDischargeTariffId).IsRequired();
        builder.Property(x => x.GroupName).IsRequired().IsUnicode(true).HasMaxLength(50);
        builder.Property(x => x.Goods).IsUnicode(true).HasMaxLength(500);
        builder.Property(x => x.Price).HasColumnType("int");

        #region Navigation
        builder.HasOne(c => c.LoadingDischargeTariff).WithMany(d => d.LoadingDischargeTariffDetails).HasForeignKey(e => e.LoadingDischargeTariffId);
        #endregion
    }
}
