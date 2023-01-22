using Microsoft.Extensions.Primitives;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Billing;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Core.Domain.Interfaces;

public interface IVoyageRepository
{
    Task<PagedData<Voyage>> GetPaginationVoyagesAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesAsync(ulong voyageId,int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<Voyage> GetVoyageById(ulong id);
    Task<IList<Voyage>> GetVoyageByVesselId(ulong vesselId);
    Task<SqlException> CreateVoyageAsync(Voyage newVoyage);
    Task<SqlException> UpdateVoyageAsync(Voyage v);
    void ToggleVoyageStatus(ulong id);
    public Task<IList<Port>> GetAllPorts();
    Task<SqlException> CreateVesselStoppageAsync(VesselStoppage newVesselStoppage, IEnumerable<KeyValuePair<string, StringValues>> formCollection);
    Task<VesselStoppage> GetVesselStoppageByVoyageId(ulong id);
    Task<SqlException> UpdateVesselStoppageAsync(VesselStoppage v, IEnumerable<KeyValuePair<string, StringValues>> formCollection);

}
