using Negin.Core.Domain.KeylessEntities;

namespace Negin.WebUI.Models.ViewModels;

public class DashboardViewModel
{
    public uint ActiveVoyages { get; set; }
    public uint Gone { get; set; }
    public uint InProcess { get; set; }
    public uint WaitForVessel { get; set; }
    public uint VesselStoppageCount { get; set; }
    public uint Invoiced { get; set; }
    public uint Confirmed { get; set; }
    public IEnumerable<int> Years { get; set; }
    public DataForDashboardChart1_Proc[] Chart1Data { get; set; }

}
