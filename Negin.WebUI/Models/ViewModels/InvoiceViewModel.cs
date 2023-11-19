using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Framework.Pagination;

namespace Negin.WebUI.Models.ViewModels;

public class InvoiceViewModel
{
    public PagedData<Tuple<VesselStoppage, Voyage>> Items { get; set; }
}
