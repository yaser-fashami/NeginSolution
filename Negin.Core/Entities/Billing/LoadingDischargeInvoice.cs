using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Operation;
using static Negin.Core.Domain.Entities.Billing.Invoice;

namespace Negin.Core.Domain.Entities.Billing;
public class LoadingDischargeInvoice : BaseAudit_SoftDeleteable_Entity<ulong>
{
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.IsIssued;
    public ulong LoadingDischargeId { get; set; }
    public LoadingDischarge? LoadingDischarge { get; set; }
    public int LoadingDischargeTariffId { get; set; }
    public LoadingDischargeTariff? LoadingDischargeTariff { get; set; }
    public uint ShippingLineCompanyId { get; set; }
    public ShippingLineCompany? ShippingLineCompany { get; set; }
    public byte DiscountPercent { get; set; }
    public double Tonage { get; set; }
    public ulong LDCostR { get; set; }
    public ulong? CraneCostR { get; set; }
    public ulong? InventoryCostR { get; set; }
    public byte? VatPercent { get; set; }
    public ulong? VatCostR { get; set; }
    public ulong TotalPriceR { get; set; }

}
