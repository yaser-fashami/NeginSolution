namespace Negin.Core.Domain.Entities.Basic;

public class DiscountTariff : BaseAuditableEntity<ushort>
{
    public string Description { get; set; } = string.Empty;
    public DateTime EffectiveDate { get; set; }
    public ushort? FlagId { get; set; }
    public Country? Flag { get; set; }
    public uint? ToGrossTonage { get; set; }
    public double DiscountPercent { get; set; }

}
