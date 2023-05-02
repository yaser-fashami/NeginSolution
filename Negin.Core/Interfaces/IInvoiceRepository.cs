using Negin.Core.Domain.Aggregates.Billing;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Core.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<PagedData<Invoice>> GetPaginationInvoiceAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<Invoice> GetInvoiceDetailsById(ulong id);
    Task<BLMessage> CancelInvoice(ulong id);
    Task<BLMessage> ConfirmInvoice(ulong id);
    Task<int> InvoiceConfirmedCount();
}
