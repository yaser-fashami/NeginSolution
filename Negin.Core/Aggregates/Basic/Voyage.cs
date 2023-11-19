using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Entities;
using Negin.Core.Domain.Entities.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negin.Core.Domain.Aggregates.Basic;

public class Voyage : BaseAudit_SoftDeleteable_Entity<ulong>
{
    public ulong VesselId { get; set; }
    public Vessel? Vessel { get; set; }
    public uint OwnerId { get; set; }
    public ShippingLineCompany? Owner { get; set; }
    public uint AgentId { get; set; }
    public ShippingLineCompany? Agent { get; set; }

    public virtual ICollection<VesselStoppage>? VesselStoppages { get; set; }
}
