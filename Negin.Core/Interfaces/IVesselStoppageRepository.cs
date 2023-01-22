
using Microsoft.Extensions.Primitives;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Billing;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Core.Domain.Interfaces;

public interface IVesselStoppageRepository
{
    Task<PagedData<VesselStoppage>> GetPaginationVesselStoppageAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<SqlException> CreateVesselStoppageAsync(VesselStoppage newVesselStoppage, IEnumerable<KeyValuePair<string, StringValues>> formCollection);
    Task<SqlException> UpdateVesselStoppageAsync(VesselStoppage v, IEnumerable<KeyValuePair<string, StringValues>> formCollection);
    void DeleteVesselStoppageById(ulong id);
    Task<IList<Port>> GetAllPorts();

}
