using Negin.Core.Domain.Entities.Basic;

namespace Negin.WebUI.Models.ViewModels;

public class ShippingLineViewModel
{
	public ShippingLineCompany ShippingLineCompany { get; set; }

	public IList<ShippingLineCompany>? AgentList { get; set; }
	public IList<uint>? AgentAssigned { get; set; }
}
