using Negin.Core.Domain.Aggregates.Basic;
using Negin.Framework.Pagination;

namespace Negin.WebUI.Models.ViewModels;

public class InvoiceViewModel
{
    public PagedData<Voyage> Voyages { get; set; }
    public int ActiveVoyages { get; set; }
    public int Gone { get; set; }
    public int InProcess { get; set; }
    public int WaitForVessel { get; set; }
    public int VesselStoppageCount { get; set; }
    public int Invoiced { get; set; }
    public int Confirmed { get; set; }
}
