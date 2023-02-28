
using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Entities.Basic;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Core.Domain.Interfaces;

public interface IBasicInfoRepository
{
	Task<PagedData<Currency>> GetPaginationCurrenciesAsync(int pageNumber = 1, int pageSize = 10);
	Task<PagedData<VesselStoppageTariff>> GetPaginationVesselStoppageTariffAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
	Task<PagedData<CleaningServiceTariff>> GetPaginationCleaningServiceTariffAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
	Task<VesselStoppageTariff> GetAllVesselStoppageTariffDetailAsync(int id);
	Task<CleaningServiceTariff> GetAllCleaningServiceTariffDetailAsync(int id);
	Task<SqlException> CreateCurrencyAsync(Currency newCurrency);
	Task<SqlException> CreateVesselStoppageTariffAsync(VesselStoppageTariff newVesselStoppageTariff);
	Task<SqlException> CreateCleaningServiceTariffAsync(CleaningServiceTariff newCleaningServiceTariff);
	Task<SqlException> CreateVesselStoppageTariffDetailsAsync(List<VesselStoppageTariffDetails> newVesselStoppageTariffDetail);
	Task<SqlException> CreateCleaningServiceTariffDetailsAsync(List<CleaningServiceTariffDetails> newCleaningServiceTariffDetail);
}
