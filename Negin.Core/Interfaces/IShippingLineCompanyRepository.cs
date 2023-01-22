
using Negin.Core.Domain.Entities.Basic;
using Negin.Framework.Pagination;
using Negin.Framework.Exceptions;

namespace Negin.Core.Domain.Interfaces;

public interface IShippingLineCompanyRepository
{
	Task<PagedData<ShippingLineCompany>> GetPaginationShippingLineCompaniesAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
	Task<IList<ShippingLineCompany>> GetAgentsAsync();
	Task<IList<ShippingLineCompany>> GetOwnersAsync();
	Task<IList<ShippingLineCompany>> GetAgentsOfOwnerAsync(uint ownerId);
	Task<SqlException> CreateShippingLineAsync(ShippingLineCompany shippingLine, IList<uint> AgentAssignedIds);
	Task<ShippingLineCompany> GetShippingLineAsync(uint id);
	Task<SqlException> EditShippingLine(ShippingLineCompany shippingLine, IList<uint> AgentAssignedIds);
	void DeleteShippingLine(ulong id);
}
