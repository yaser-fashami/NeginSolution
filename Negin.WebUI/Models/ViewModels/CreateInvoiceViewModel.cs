using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Operation;
using Negin.Framework.Pagination;

namespace Negin.WebUI.Models.ViewModels;

public class CreateInvoiceViewModel
{
    public Voyage Voyage { get; set; }
    public PagedData<VesselStoppage> VesselStoppages { get; set; }
}
