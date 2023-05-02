using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Negin.Core.Domain.Aggregates.Billing;
using Negin.Core.Domain.Aggregates.Operation;

namespace Negin.Infra.Data.Sql.Configurations;

internal class InvoiceConfig : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices", "Billing");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.InvoiceNo).IsRequired().IsUnicode(false).HasMaxLength(15);
        builder.Property(x => x.VoyageId).IsRequired();
        builder.Property(x => x.Status).IsRequired().HasColumnType("tinyint");
        builder.Property(x => x.IsPaied).IsRequired().HasDefaultValue(false);
        builder.Property(x => x.TotalDwellingHour).IsRequired().HasColumnType("int");

        #region Navigation
        builder.HasMany(c => c.VesselStoppageInvoiceDetails).WithOne(d => d.Invoice).HasForeignKey(s => s.InvoiceId);
        builder.HasMany(c => c.CleaningServiceInvoiceDetails).WithOne(d => d.Invoice).HasForeignKey(s => s.InvoiceId);
        #endregion
    }
}

internal class VesselStoppageInvoiceDetailConfig : IEntityTypeConfiguration<VesselStoppageInvoiceDetail>
{
    public void Configure(EntityTypeBuilder<VesselStoppageInvoiceDetail> builder)
    {
        builder.ToTable("VesselStoppageInvoiceDetails", "Billing");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.VesselStoppageTariffId).IsRequired();
        builder.Property(x => x.CurrencyId).IsRequired();
        builder.Property(x => x.ApplyCurrencyRate).IsRequired().HasColumnType("int");
        builder.Property(x => x.DwellingHour).IsRequired();
        builder.Property(x => x.PriceR).IsRequired();
        builder.Property(x => x.PriceD).IsRequired();
        builder.Property(x => x.PriceRVat).IsRequired();
        builder.Property(x => x.ApplyNormalPrice).IsRequired();
        builder.Property(x => x.ApplyExtraPrice).IsRequired();

        #region Navigation
        builder.HasOne(c => c.Invoice).WithMany(d => d.VesselStoppageInvoiceDetails).HasForeignKey(s => s.InvoiceId);
        builder.HasOne(c => c.VesselStoppageTariff).WithMany(d => d.VesselStoppageInvoiceDetail).HasForeignKey(s => s.VesselStoppageTariffId);
        #endregion
    }
}

internal class CleaningServiceInvoiceDetailConfig : IEntityTypeConfiguration<CleaningServiceInvoiceDetail>
{
    public void Configure(EntityTypeBuilder<CleaningServiceInvoiceDetail> builder)
    {
        builder.ToTable("CleaningServiceInvoiceDetails", "Billing");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CleaningServiceTariffId).IsRequired();
        builder.Property(x => x.CurrencyId).IsRequired();
        builder.Property(x => x.ApplyCurrencyRate).IsRequired().HasColumnType("int");
        builder.Property(x => x.DwellingHour).IsRequired();
        builder.Property(x => x.PriceR).IsRequired();
        builder.Property(x => x.PriceD).IsRequired();
        builder.Property(x => x.PriceRVat).IsRequired();
        builder.Property(x => x.ApplyPrice).IsRequired();

        #region Navigation
        builder.HasOne(c => c.Invoice).WithMany(d => d.CleaningServiceInvoiceDetails).HasForeignKey(s => s.InvoiceId);
        builder.HasOne(c => c.CleaningServiceTariff).WithMany(d => d.CleaningServiceInvoiceDetail).HasForeignKey(s => s.CleaningServiceTariffId);

        #endregion
    }
}


