using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class VesselStoppageTariffConfig : IEntityTypeConfiguration<VesselStoppageTariff>
{
	public void Configure(EntityTypeBuilder<VesselStoppageTariff> builder)
	{
		builder.ToTable("VesselStoppageTariff", "Basic");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Description).IsRequired().HasMaxLength(50);
		builder.Property(x => x.EffectiveDate).IsRequired().HasColumnType("smalldatetime");

		#region Navigation
		builder.HasMany(c => c.VesselStoppageTariffDetails).WithOne(d => d.VesselStoppageTarriff).HasForeignKey("VesselStoppageTarrifId");
        builder.HasMany(c => c.VesselStoppageInvoiceDetail).WithOne(d => d.VesselStoppageTariff).HasForeignKey(s => s.VesselStoppageTariffId);

        #endregion
    }
}

internal class VesselStoppageTariffDetailsConfig : IEntityTypeConfiguration<VesselStoppageTariffDetails>
{
	public void Configure(EntityTypeBuilder<VesselStoppageTariffDetails> builder)
	{
		builder.ToTable("VesselStoppageTariffDetails", "Basic");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.VesselStoppageTarrifId).IsRequired();
		builder.Property(x => x.VesselTypeId).IsRequired();
		builder.Property(x => x.NormalHour).IsRequired();
		builder.Property(x => x.NormalPrice).IsRequired();
		builder.Property(x => x.ExtraPrice).IsRequired();

		#region Navigation
		builder.HasOne(c => c.VesselStoppageTarriff).WithMany(d => d.VesselStoppageTariffDetails).HasForeignKey("VesselStoppageTarrifId");
		builder.HasOne(c => c.VesselType).WithMany(d => d.VesselStoppageTariffDetails).HasForeignKey("VesselTypeId");
		#endregion
	}
}
