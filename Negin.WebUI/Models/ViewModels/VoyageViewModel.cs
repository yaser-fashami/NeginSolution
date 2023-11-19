using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.WebUI.Models.ViewModels;

public class VoyageViewModel
{
	public Voyage Voyage { get; set; }
	public IList<Vessel>? VesselList { get; set; }
	public IList<ShippingLineCompany>? OwnerShippinglineList { get; set; }
}
