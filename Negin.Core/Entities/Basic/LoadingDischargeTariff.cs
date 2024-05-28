using Negin.Core.Domain.Entities;

namespace Negin.Core.Domain.Entities.Basic;
public class LoadingDischargeTariff : BaseAuditableEntity<int>
{
    public string? Description { get; set; }
    public DateTime EffectiveDate { get; set; }

    public virtual ICollection<LoadingDischargeTariffDetails> LoadingDischargeTariffDetails { get; set; }

}

public class LoadingDischargeTariffDetails : BaseEntity<int>
{
    public int LoadingDischargeTariffId { get; set; }
    public LoadingDischargeTariff? LoadingDischargeTariff { get; set; }
    public string GroupName { get; set; }
    public string? Goods { get; set; }
    public uint Price { get; set; }
}
