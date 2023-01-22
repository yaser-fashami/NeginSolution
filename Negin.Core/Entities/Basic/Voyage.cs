using Negin.Core.Domain.Entities.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Negin.Core.Domain.Entities.Basic;

public class Voyage: BaseEntity
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
