using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.Core.Domain.Aggregates.Basic;

public class Voyage : BaseAudit_SoftDeleteable_Entity<ulong>
{
    public string VoyageNoIn { get; set; }
    public string VoyageNoOut { get; set; }
    public ulong VesselId { get; set; }
    public Vessel? Vessel { get; set; }
    public uint OwnerId { get; set; }
    public ShippingLineCompany? Owner { get; set; }
    public uint AgentId { get; set; }
    public ShippingLineCompany? Agent { get; set; }

    public virtual ICollection<VesselStoppage>? VesselStoppages { get; set; }
}
