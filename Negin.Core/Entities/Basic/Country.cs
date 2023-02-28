
namespace Negin.Core.Domain.Entities.Basic;

public class Country : BaseEntity<ushort>
{
    public string Name { get; set; }
    public string Symbol { get; set; }
    public string? FlagName { get; set; }
    public virtual ICollection<Vessel>? Nationalities { get; set; }
    public virtual ICollection<Vessel>? Flags { get; set; }
    public virtual ICollection<Port>? Ports { get; set; }
}
