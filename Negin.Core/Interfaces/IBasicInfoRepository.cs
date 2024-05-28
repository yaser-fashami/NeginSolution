using Negin.Core.Domain.Dtos;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Operation;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;

namespace Negin.Core.Domain.Interfaces;

public interface IBasicInfoRepository
{
    Task<IList<Port>> GetAllPorts();

    Task<PagedData<Currency>> GetPaginationCurrenciesAsync(int pageNumber = 1, int pageSize = 10);
    Task<SqlException> CreateCurrencyAsync(Currency newCurrency);

    #region Tariffs
    Task<PagedData<VesselStoppageTariff>> GetPaginationVesselStoppageTariffAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<PagedData<CleaningServiceTariff>> GetPaginationCleaningServiceTariffAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<PagedData<LoadingDischargeTariff>> GetPaginationLoadingDischargeTariffAsync(int pageNumber = 1, int pageSize = 10, string filter = "");

    Task<VesselStoppageTariff> GetAllVesselStoppageTariffDetailAsync(int id);
	Task<CleaningServiceTariff> GetAllCleaningServiceTariffDetailAsync(int id);
	Task<LoadingDischargeTariff> GetLoadingDischargeTariffByIdAsync(int id);
	Task<List<LoadingDischargeTariffDetails>> GetAllLoadingDischargeTariffDetailsAsync();
	Task<SqlException> CreateVesselStoppageTariffAsync(VesselStoppageTariff newVesselStoppageTariff);
	Task<SqlException> CreateCleaningServiceTariffAsync(CleaningServiceTariff newCleaningServiceTariff);
	Task<SqlException> CreateLoadingDischargeTariffAsync(LoadingDischargeTariff newLoadingDischargeTariff);
	Task<SqlException> CreateVesselStoppageTariffDetailsAsync(List<VesselStoppageTariffDetails> newVesselStoppageTariffDetail);
	Task<SqlException> CreateCleaningServiceTariffDetailsAsync(List<CleaningServiceTariffDetails> newCleaningServiceTariffDetail);
	Task<SqlException> CreateLoadingDischargeTariffDetailsAsync(List<LoadingDischargeTariffDetails> newLoadingDischargeTariffDetails);
    #endregion

    #region Vessel
    Task<PagedData<Vessel>> GetPaginationVesselsAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<SqlException> CreateVesselAsync(Vessel newVessel);
    Task<SqlException> UpdateVesselAsync(Vessel v);
    Task<IEnumerable<Country>> GetAllCountries();
    Task<IEnumerable<VesselType>> GetAllVesselTypes();
    Task<Vessel> GetVesselById(ulong id);
    Task<IList<Vessel>> GetAllVessels();
    Task DeleteVesselById(ulong id);

    #endregion

    #region ShippingLineCompany
    Task<PagedData<ShippingLineCompany>> GetPaginationShippingLineCompaniesAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<IList<ShippingLineCompany>> GetAgentsAsync();
    Task<IList<ShippingLineCompany>> GetOwnersAsync();
    Task<IList<ShippingLineCompany>> GetAgentsOfOwnerAsync(uint ownerId);
    Task<SqlException> CreateShippingLineAsync(ShippingLineCompany shippingLine, IList<uint> AgentAssignedIds);
    Task<ShippingLineCompany> GetShippingLineAsync(uint id);
    Task<SqlException> EditShippingLine(ShippingLineCompany shippingLine, IList<uint> AgentAssignedIds);
    void DeleteShippingLine(ulong id);
    Task<IList<ShippingLineCompany>> GetAllPorterageCompanies();
	#endregion

	#region Voyage
	Task<PagedData<Voyage>> GetPaginationVoyagesAsync(int pageNumber = 1, int pageSize = 10, string filter = "");
    Task<Voyage> GetVoyageById(ulong id);
    Task<Voyage> GetVoyageByVesselId(ulong vesselId);
    Task<SqlException> CreateVoyageAsync(Voyage newVoyage);
    Task<SqlException> UpdateVoyageAsync(Voyage v);
    BLMessage ToggleVoyageStatus(ulong id);
    Task<DashboardDto> GetAllVoyageForDashboard();

    #endregion
}

