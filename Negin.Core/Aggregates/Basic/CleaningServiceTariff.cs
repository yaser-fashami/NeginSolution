using Negin.Core.Domain.Aggregates.Billing;
using Negin.Core.Domain.Entities;

namespace Negin.Core.Domain.Aggregates.Basic;

public class CleaningServiceTariff : BaseAuditableEntity<ushort>
{
    public string Description { get; set; }
    public DateTime EffectiveDate { get; set; }

    public virtual ICollection<CleaningServiceTariffDetails> CleaningServiceTariffDetails { get; set; }
    public virtual ICollection<CleaningServiceInvoiceDetail> CleaningServiceInvoiceDetail { get; set; }

}

public class CleaningServiceTariffDetails : BaseEntity<uint>
{
    public ushort CleaningServiceTariffId { get; set; }
    public CleaningServiceTariff? CleaningServiceTariff { get; set; }
    public uint GrossWeight { get; set; }
    public double Price { get; set; }
    public double? Vat { get; set; }
}
