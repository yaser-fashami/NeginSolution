using Negin.Core.Domain.Aggregates.Basic;
using Negin.Framework.Pagination;

namespace Negin.WebUI.Models.ViewModels;

public class InvoiceViewModel
{
    public PagedData<Voyage> Voyages { get; set; }
}
