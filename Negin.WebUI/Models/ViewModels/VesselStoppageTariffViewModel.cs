using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Entities.Basic;

namespace Negin.WebUI.Models.ViewModels;

public class VesselStoppageTariffViewModel
{
	public IList<VesselType>? VesselTypes { get; set; }
	public VesselStoppageTariffDetails VesselStoppageTariffDetals { get; set; }

}
