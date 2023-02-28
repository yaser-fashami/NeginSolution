using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Aggregates.Basic;

namespace Negin.Infra.Data.Sql.Configurations;

internal class CleaningServiceTariffConfig : IEntityTypeConfiguration<CleaningServiceTariff>
{
	public void Configure(EntityTypeBuilder<CleaningServiceTariff> builder)
	{
		builder.ToTable("CleaningServiceTariff", "Basic");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Description).IsRequired().HasMaxLength(50);
		builder.Property(x => x.EffectiveDate).IsRequired().HasColumnType("smalldatetime");

		#region Navigation
		builder.HasMany(c => c.CleaningServiceTariffDetails).WithOne(d => d.CleaningServiceTariff).HasForeignKey("CleaningServiceTariffId");
		#endregion
	}
}

internal class CleaningServiceTariffDetailsConfig : IEntityTypeConfiguration<CleaningServiceTariffDetails>
{
	public void Configure(EntityTypeBuilder<CleaningServiceTariffDetails> builder)
	{
		builder.ToTable("CleaningServiceTariffDetails", "Basic");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.CleaningServiceTariffId).IsRequired();
		builder.Property(x => x.GrossWeight).IsRequired();
		builder.Property(x => x.Price).IsRequired();
		builder.Property(x => x.Vat).IsRequired(false);

		#region Navigation
		builder.HasOne(c => c.CleaningServiceTariff).WithMany(d => d.CleaningServiceTariffDetails).HasForeignKey("CleaningServiceTariffId");
		#endregion
	}
}
