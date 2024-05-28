using Negin.Core.Domain.Entities.Operation;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Core.Domain.Interfaces;
public interface IOperationRepository
{
    Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesAsync(string vesselName, int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesForInvoiceAsync(ulong voyageId, int pageNumber = 1, int pageSize = 10);
    Task<SqlException> CreateVesselStoppageAsync(VesselStoppage newVesselStoppage);
    Task<SqlException> UpdateVesselStoppageAsync(VesselStoppage v);
    Task<VesselStoppage> GetVesselStoppageByVoyageId(ulong id);


    Task<List<VesselStoppage>> GetAllVSForLoadAndDischargeAsync();
    Task<SqlException> CreateLoadingDischargeAsync(LoadingDischarge loadingDischarge);
    Task<PagedData<LoadingDischarge>> GetPaginationLoadingDischargeAsync(ulong vesselStoppageId, int pageNumber = 1, int pageCount = 10, string filter = "");
    Task<LoadingDischarge> GetLoadingDischargeById(ulong id);
    Task<SqlException> UpdateLoadingDischargeAsync(LoadingDischarge loadingDischarge);
    Task<SqlException> DeleteLoadingDischargeAsync(ulong id);
	Task<PagedData<LoadingDischarge>> GetPaginationLoadingDischargeForInvoicingAsync(int pageNumber = 1, int pageCount = 10, string filter = "");

}
