using Negin.Core.Domain.Entities.Basic;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Core.Domain.Interfaces;

public interface IVesselRepository
{
    Task<PagedData<Vessel>> GetPaginationVesselsAsync(int pageNumber = 1, int pageSize = 10, string filter = "");

    Task<SqlException> CreateVesselAsync(Vessel newVessel);
	Task<SqlException> UpdateVesselAsync(Vessel v);
    Task<IEnumerable<Country>> GetAllCountries();
    Task<IEnumerable<VesselType>> GetAllVesselTypes();
    Task<Vessel> GetVesselById(ulong id);
    Task<IList<Vessel>> GetVesselsAssignedVoyage();
    Task<IList<Vessel>> GetVesselsNotAssignedVoyage();
    Task DeleteVesselById(ulong id);
}
