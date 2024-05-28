using Negin.Core.Domain.Entities.Billing;
using Negin.Core.Domain.Entities.Operation;
using Negin.Core.Domain.KeylessEntities;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Core.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<PagedData<VesselStoppage>> GetPaginationDataForBillingAsync(int pageNumber = 1, int pageSize = 10, string filter = "", bool? invoiced = null);

    Task<Invoice> GetInvoiceDetailsById(ulong id);
    Task<BLMessage> CancelInvoice(ulong id); 
    Task<BLMessage> ConfirmInvoice(ulong id);
    Task<int> InvoiceConfirmedCount();
    DataForDashboardChart1_Proc[] GetSumPriceProc(string year);
    Task<IList<int>> GetExistYear();

    Task<LoadingDischargeInvoice> GetLoadingDischargeInvoice(ulong id);
    Task<BLMessage> CancelLoadinDischargeInvoice(ulong id);
    Task<BLMessage> ConfirmLoadingDischargeInvoice(ulong id);
}
