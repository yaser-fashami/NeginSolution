using Negin.Core.Domain.Entities.Basic;

namespace Negin.WebUI.Models.ViewModels;

public class VesselViewModel
{
	public Vessel Vessel { get; set; }
	public IList<Country>? Countries { get; set; }
	public IList<VesselType>? VesselTypes { get; set; }
}
