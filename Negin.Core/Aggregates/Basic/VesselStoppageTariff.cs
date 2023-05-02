using Negin.Core.Domain.Aggregates.Billing;
using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Core.Domain.Aggregates.Basic;

public class VesselStoppageTariff : BaseAuditableEntity<ushort>
{
    public string Description { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }

    public virtual ICollection<VesselStoppageTariffDetails> VesselStoppageTariffDetails { get; set; }
    public virtual ICollection<VesselStoppageInvoiceDetail> VesselStoppageInvoiceDetail { get; set; }
}

public class VesselStoppageTariffDetails : BaseEntity<uint>
{
    public ushort VesselStoppageTarrifId { get; set; }
    public VesselStoppageTariff? VesselStoppageTarriff { get; set; }
    public byte VesselTypeId { get; set; }
    public VesselType? VesselType { get; set; }
    public int NormalHour { get; set; }
    public double NormalPrice { get; set; }
    public double ExtraPrice { get; set; }
}
