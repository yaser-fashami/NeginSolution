using Microsoft.Extensions.Primitives;
using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Entities.Basic;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Core.Domain.Interfaces;

public interface IVoyageRepository
{
    Task<PagedData<Voyage>> GetPaginationVoyagesAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<PagedData<Voyage>> GetPaginationVoyagesForBillingAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesAsync(ulong voyageId,int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesForInvoiceAsync(ulong voyageId, int pageNumber = 1, int pageSize = 10);
    Task<Voyage> GetVoyageById(ulong id);
    Task<IList<Voyage>> GetVoyageByVesselId(ulong vesselId);
    Task<SqlException> CreateVoyageAsync(Voyage newVoyage);
    Task<SqlException> UpdateVoyageAsync(Voyage v);
    BLMessage ToggleVoyageStatus(ulong id);
    public Task<IList<Port>> GetAllPorts();
    Task<SqlException> CreateVesselStoppageAsync(VesselStoppage newVesselStoppage);
    Task<VesselStoppage> GetVesselStoppageByVoyageId(ulong id);
    Task<SqlException> UpdateVesselStoppageAsync(VesselStoppage v);

}
