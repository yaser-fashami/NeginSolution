using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Entities.Billing;

namespace Negin.Infra.Data.Sql.Configurations;
internal class LoadingDischargeInvoiceConfig : IEntityTypeConfiguration<LoadingDischargeInvoice>
{
	public void Configure(EntityTypeBuilder<LoadingDischargeInvoice> builder)
	{
		builder.ToTable(nameof(LoadingDischargeInvoice), "Billing");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.InvoiceNo).IsRequired().IsUnicode(false).HasMaxLength(15);
        builder.Property(x => x.Status).IsRequired().HasColumnType("tinyint");
        builder.Property(x => x.DiscountPercent).IsRequired().HasDefaultValue(0);
	}
}
