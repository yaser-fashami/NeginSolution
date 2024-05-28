using Negin.Core.Domain.Entities.Basic;

namespace Negin.WebUI.Models.ViewModels;

public class CreateLoadingDischargeViewModel
{
    public string Method { get; set; }
    public int VesselStoppageId { get; set; }
    public List<LoadingDischargeTariffDetails> LoadingDischargeTariffDetails { get; set; }
}
