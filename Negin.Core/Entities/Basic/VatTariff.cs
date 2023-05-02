
namespace Negin.Core.Domain.Entities.Basic;

public class VatTariff : BaseAuditableEntity<ushort>
{
    public string Description { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public double Rate { get; set; }
    public double? TollRate { get; set; }
}
