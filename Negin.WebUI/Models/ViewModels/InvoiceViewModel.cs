using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Operation;
using Negin.Framework.Pagination;

namespace Negin.WebUI.Models.ViewModels;

public class InvoiceViewModel
{
    public PagedData<VesselStoppage> Items { get; set; }
}
