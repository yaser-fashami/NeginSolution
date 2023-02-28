using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Framework.Pagination;

namespace Negin.WebUI.Models.ViewModels;

public class CreateInvoiceViewModel
{
    public Voyage Voyage { get; set; }
    public PagedData<VesselStoppage> VesselStoppages { get; set; }
}
