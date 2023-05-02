using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Entities.Basic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Negin.Core.Domain.Aggregates.Billing;

public class Invoice : BaseAuditableEntity<ulong>
{
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public ulong VoyageId { get; set; }
    public Voyage? Voyage { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.IsIssued;
    public bool IsPaied { get; set; } = false;
    public uint TotalDwellingHour { get; set; }
    public ulong SumPriceR { get; set; }
    public decimal SumPriceD { get; set; }
    public ulong SumPriceRVat { get; set; }

    public virtual ICollection<VesselStoppageInvoiceDetail> VesselStoppageInvoiceDetails { get; set; }
    public virtual ICollection<CleaningServiceInvoiceDetail> CleaningServiceInvoiceDetails { get; set; }

    public enum InvoiceStatus
    {
        IsIssued = 0, IsCancel = 1, IsConfirm = 2, SentToFinance = 3
    }
}

public class VesselStoppageInvoiceDetail : BaseAudit_SoftDeleteable_Entity<ulong>
{
    public ulong InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public ulong VesselStoppageId { get; set; }
    public VesselStoppage? VesselStoppage { get; set; }
    public ushort VesselStoppageTariffId { get; set; }
    public VesselStoppageTariff? VesselStoppageTariff { get; set; }
    public uint CurrencyId { get; set; }
    public Currency? Currency { get; set; }
    public uint DwellingHour { get; set; }
    public ulong PriceR { get; set; }
    public decimal PriceD { get; set; }
    public ulong PriceRVat { get; set; }

    public double ApplyNormalPrice { get; set; }
    public double ApplyExtraPrice { get; set; }
    public uint ApplyCurrencyRate { get; set; }

}

public class CleaningServiceInvoiceDetail : BaseAudit_SoftDeleteable_Entity<ulong>
{
    public ulong InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public ulong VesselStoppageId { get; set; }
    public VesselStoppage? VesselStoppage { get; set; }
    public ushort CleaningServiceTariffId { get; set; }
    public CleaningServiceTariff? CleaningServiceTariff { get; set; }
    public uint CurrencyId { get; set; }
    public Currency? Currency { get; set; }

    public uint DwellingHour { get; set; }
    public ulong PriceR { get; set; }
    public decimal PriceD { get; set; }
    public ulong PriceRVat { get; set; }

    public double ApplyPrice { get; set; }
    public uint ApplyCurrencyRate { get; set; }

}