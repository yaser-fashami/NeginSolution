
using System.ComponentModel;

namespace Negin.Core.Domain.Entities.Basic;

public class AgentShippingLine: BaseEntity<uint>
{
	[DisplayName("Owner")]
	public uint ShippingLineCompanyId { get; set; }
	public ShippingLineCompany ShippingLineCompany { get; set; }
	public uint AgentShippingLineCompanyId { get; set; }
	public ShippingLineCompany AgentShippingLineCompany { get; set; }

}
