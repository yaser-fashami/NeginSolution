using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Dtos;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Operation;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;
using static Negin.Framework.Exceptions.SqlException;

namespace Negin.Infra.Data.Sql.EFRepositories;

public class BasicInfoRepository : IBasicInfoRepository
{
	private readonly NeginDbContext _neginDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BasicInfoRepository(NeginDbContext neginDbContext, IHttpContextAccessor httpContextAccessor)
	{
		_neginDbContext = neginDbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<IList<Port>> GetAllPorts()
    {
        return await _neginDbContext.Ports.Include(c => c.Country).AsNoTracking().ToListAsync();
    }

    #region Vessel
    public async Task<SqlException> CreateVesselAsync(Vessel newVessel)
    {
        var result = new SqlException();
        newVessel = (Vessel)Util.TrimAllStringFields(newVessel);
        var vessel = new Vessel()
        {
            Name = newVessel.Name,
            VesselTypeId = newVessel.VesselTypeId,
            GrossTonage = newVessel.GrossTonage,
            CallSign = newVessel.CallSign,
            NationalityId = newVessel.NationalityId,
            VesselLength = newVessel.VesselLength,
            VesselWidth = newVessel.VesselWidth,
            BaysQuantity = newVessel.BaysQuantity,
            ActiveCraneQty = newVessel.ActiveCraneQty,
            FlagId = newVessel.FlagId,
            Color = newVessel.Color,
            CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId(),
            CreateDate = DateTime.Now,
            IsDelete = false,
        };
        try
        {
            await _neginDbContext.AddAsync(vessel);
            await _neginDbContext.SaveChangesAsync();
            result.State = true;
        }
        catch (UniqueConstraintException)
        {
            var oldVessel = _neginDbContext.Vessels.Single(c => c.Name == newVessel.Name);
            if (oldVessel.IsDelete)
            {
                _neginDbContext.Vessels.Remove(oldVessel);
                _neginDbContext.SaveChanges();
                result.State = true;
            }
            else
            {
                result.SqlState = SqlExceptionState.DuplicateName;
                result.Message = "This vessel name is exist. please enter another name";
            }
        }
        catch (Exception ex)
        {
            result.State = false;
            result.Message = ex.Message;
        }

        return result;
    }

    public async Task<IEnumerable<Country>> GetAllCountries()
    {
        return await _neginDbContext.Countries.AsNoTracking().OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<IEnumerable<VesselType>> GetAllVesselTypes()
    {
        return await _neginDbContext.VesselTypes.AsNoTracking().ToListAsync();
    }

    public async Task<Vessel> GetVesselById(ulong id)
    {
        return await _neginDbContext.Vessels.AsNoTracking().SingleAsync(c => (c.Id == id && c.IsDelete == false));
    }

    public async Task<PagedData<Vessel>> GetPaginationVesselsAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var vessels = _neginDbContext.Vessels.Include("Type").Include("Nationality").Include("Flag").AsNoTracking()
            .Where(x => x.IsDelete == false)
            .Where(x => noFilter || x.Name.Contains(filter) || x.Type.Name.Contains(filter) || x.Nationality.Name.Contains(filter) || x.Flag.Name.Contains(filter));

        PagedData<Vessel> result = new()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await vessels.CountAsync()
            },
            Data = await vessels.OrderBy(x => x.Name)
                .ToPagination(pageNumber, pageSize)
                .ToListAsync()
        };
        return result;
    }

    public async Task<SqlException> UpdateVesselAsync(Vessel v)
    {
        var result = new SqlException();

        var vessel = await _neginDbContext.Vessels.SingleAsync(c => c.Id == v.Id);
        v = (Vessel)Util.TrimAllStringFields(v);
        if (vessel != null)
        {
            vessel.Name = v.Name;
            vessel.VesselTypeId = v.VesselTypeId;
            vessel.GrossTonage = v.GrossTonage;
            vessel.CallSign = v.CallSign;
            vessel.NationalityId = v.NationalityId;
            vessel.VesselLength = v.VesselLength;
            vessel.VesselWidth = v.VesselWidth;
            vessel.BaysQuantity = v.BaysQuantity;
            vessel.ActiveCraneQty = v.ActiveCraneQty;
            vessel.FlagId = v.FlagId;
            vessel.Color = v.Color;
            vessel.ModifiedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
            vessel.ModifiedDate = DateTime.Now;
            vessel.IsDelete = v.IsDelete;
        }

        try
        {
            _neginDbContext.Vessels.Update(vessel);
            await _neginDbContext.SaveChangesAsync();
            result.State = true;
        }
        catch (Exception ex)
        {
            result.State = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task DeleteVesselById(ulong id)
    {
        try
        {
            var vessel = await _neginDbContext.Vessels.AsNoTracking().SingleAsync(c => c.Id == id);
            vessel.IsDelete = true;
            _neginDbContext.Vessels.Update(vessel);
            await _neginDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public async Task<IList<Vessel>> GetAllVessels()
    {
        return await _neginDbContext.Vessels.AsNoTracking().ToListAsync();
    }

    #endregion

    #region ShippingLineCompany
    public async Task<SqlException> CreateShippingLineAsync(ShippingLineCompany shippingLine, IList<uint> AgentAssignedIds)
    {
        var result = new SqlException();
        if (shippingLine.IsOwner && !shippingLine.IsAgent && AgentAssignedIds == null)
        {
            result.SqlState = SqlExceptionState.AgentIsEmpty;
            result.Message = "this company must has at least one agent! because it`s type is owner.";
        }
        else
        {
            try
            {
                shippingLine = (ShippingLineCompany)Util.TrimAllStringFields(shippingLine);
                shippingLine.CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
                shippingLine.CreateDate = DateTime.Now;
                await _neginDbContext.ShippingLines.AddAsync(shippingLine);
                _neginDbContext.SaveChanges();
                if (shippingLine.IsOwner && shippingLine.IsAgent)
                {
                    _neginDbContext.AgentsShippingLine.Add(new AgentShippingLine()
                    {
                        ShippingLineCompanyId = shippingLine.Id,
                        AgentShippingLineCompanyId = shippingLine.Id
                    });
                    await _neginDbContext.SaveChangesAsync();
                }
                else if (shippingLine.IsOwner && !shippingLine.IsAgent && AgentAssignedIds != null)
                {
                    foreach (var agentId in AgentAssignedIds)
                    {
                        _neginDbContext.AgentsShippingLine.Add(new AgentShippingLine()
                        {
                            ShippingLineCompanyId = shippingLine.Id,
                            AgentShippingLineCompanyId = agentId
                        });
                    }
                    await _neginDbContext.SaveChangesAsync();
                }
                result.State = true;
            }
            catch (UniqueConstraintException ex)
            {
                var oldShippingline = _neginDbContext.ShippingLines.Single(c => c.ShippingLineName == shippingLine.ShippingLineName);

                if (oldShippingline.IsDelete)
                {
                    _neginDbContext.ShippingLines.Remove(oldShippingline);
                    _neginDbContext.SaveChanges();
                    result.State = true;
                }
                else
                {
                    result.SqlState = SqlExceptionState.DuplicateName;
                    result.Message = "This shipping line name is exist. please enter another name";
                }
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = ex.Message;
            }
        }

        return result;
    }

    public async Task<ShippingLineCompany> GetShippingLineAsync(uint id)
    {
        return await _neginDbContext.ShippingLines.Include(c => c.Agents).ThenInclude(c => c.AgentShippingLineCompany).AsNoTracking().SingleAsync(p => p.Id == id);
    }

    public async Task<PagedData<ShippingLineCompany>> GetPaginationShippingLineCompaniesAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var shippingLines = _neginDbContext.ShippingLines.Include(c => c.Agents).ThenInclude(c => c.AgentShippingLineCompany).AsNoTracking()
            .Where(x => x.IsDelete == false)
            .Where(c => noFilter
                || c.ShippingLineName.Contains(filter)
                || c.City.Contains(filter)
                || c.Email.Contains(filter)
                || c.NationalCode.Contains(filter));

        PagedData<ShippingLineCompany> result = new PagedData<ShippingLineCompany>()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await shippingLines.CountAsync()
            },
            Data = await shippingLines.OrderBy(c => c.ShippingLineName).ToPagination(pageNumber, pageSize).ToListAsync()
        };

        return result;
    }

    public async Task<SqlException> EditShippingLine(ShippingLineCompany newShippingLine, IList<uint> AgentAssignedIds)
    {
        var result = new SqlException() { State = false };
        ShippingLineCompany shippingline;
        if (newShippingLine.IsOwner && !newShippingLine.IsAgent && AgentAssignedIds == null)
        {
            result.SqlState = SqlExceptionState.AgentIsEmpty;
            result.Message = "this company must has at least one agent! because it`s type is owner.";
        }
        else
        {
            shippingline = _neginDbContext.ShippingLines.Single(p => p.Id == newShippingLine.Id);
            newShippingLine = (ShippingLineCompany)Util.TrimAllStringFields(newShippingLine);
            if (shippingline != null)
            {
                shippingline.ShippingLineName = newShippingLine.ShippingLineName;
                shippingline.EconomicCode = newShippingLine.EconomicCode;
                shippingline.Tel = newShippingLine.Tel;
                shippingline.Email = newShippingLine.Email;
                shippingline.Address = newShippingLine.Address;
                shippingline.City = newShippingLine.City;
                shippingline.NationalCode = newShippingLine.NationalCode;
                shippingline.Description = newShippingLine.Description;
                shippingline.ModifiedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
                shippingline.ModifiedDate = DateTime.Now;
                shippingline.IsAgent = newShippingLine.IsAgent;
                shippingline.IsOwner = newShippingLine.IsOwner;
                shippingline.IsPorterage = newShippingLine.IsPorterage;

                try
                {
                    _neginDbContext.ShippingLines.Update(shippingline);
                    await UpdateAgentsAsync(AgentAssignedIds, shippingline);
                    _neginDbContext.SaveChanges();
                    result.State = true;
                }
                catch (UniqueConstraintException ex)
                {
                    var oldShippingline = _neginDbContext.ShippingLines.Single(c => c.ShippingLineName == shippingline.ShippingLineName);

                    if (oldShippingline.IsDelete)
                    {
                        _neginDbContext.ShippingLines.Remove(oldShippingline);
                        _neginDbContext.SaveChanges();
                        result.State = true;
                    }
                    else
                    {
                        result.SqlState = SqlExceptionState.DuplicateName;
                        result.Message = "This shipping line name is exist. please enter another name";
                    }
                }
                catch (Exception ex)
                {
                    result.State = false;
                    result.Message = ex.Message;
                }
            }
        }

        return result;
    }

    private async Task UpdateAgentsAsync(IList<uint> AgentAssignedIds, ShippingLineCompany shippingline)
    {
        if (shippingline.IsOwner && shippingline.IsAgent)
        {
            var agentShippingline = await _neginDbContext.AgentsShippingLine.Where(c => c.ShippingLineCompanyId == shippingline.Id).ToListAsync();
            _neginDbContext.RemoveRange(agentShippingline);
            _neginDbContext.AgentsShippingLine.Add(new AgentShippingLine()
            {
                ShippingLineCompanyId = shippingline.Id,
                AgentShippingLineCompanyId = shippingline.Id
            });
            await _neginDbContext.SaveChangesAsync();
        }
        else if (shippingline.IsOwner && !shippingline.IsAgent && AgentAssignedIds != null)
        {
            var agentShippingline = await _neginDbContext.AgentsShippingLine.Where(c => c.ShippingLineCompanyId == shippingline.Id).ToListAsync();
            _neginDbContext.RemoveRange(agentShippingline);

            foreach (var agentId in AgentAssignedIds)
            {
                _neginDbContext.AgentsShippingLine.Add(new AgentShippingLine()
                {
                    ShippingLineCompanyId = shippingline.Id,
                    AgentShippingLineCompanyId = agentId
                });
            }
            await _neginDbContext.SaveChangesAsync();
        }
        else if (!shippingline.IsOwner && shippingline.IsAgent)
        {
            var agentShippingline = await _neginDbContext.AgentsShippingLine.Where(c => c.ShippingLineCompanyId == shippingline.Id).ToListAsync();
            _neginDbContext.RemoveRange(agentShippingline);

            await _neginDbContext.SaveChangesAsync();
        }
    }

    public void DeleteShippingLine(ulong id)
    {
        try
        {
            var shippingline = _neginDbContext.ShippingLines.Single(c => c.Id == id);
            shippingline.IsDelete = true;
            _neginDbContext.ShippingLines.Update(shippingline);
            _neginDbContext.SaveChanges();
        }
        catch (Exception ex)
        {

            throw ex;
        }
    }

    public async Task<IList<ShippingLineCompany>> GetAgentsAsync()
    {
        return await _neginDbContext.ShippingLines.AsNoTracking().Where(c => c.IsAgent == true && c.IsOwner == false).ToListAsync();
    }

    public async Task<IList<ShippingLineCompany>> GetOwnersAsync()
    {
        return await _neginDbContext.ShippingLines.AsNoTracking().Where(c => c.IsOwner == true).ToListAsync();
    }

    public async Task<IList<ShippingLineCompany>> GetAgentsOfOwnerAsync(uint ownerId)
    {
        var agentIds = await _neginDbContext.AgentsShippingLine.Where(c => c.ShippingLineCompanyId == ownerId).Select(c => c.AgentShippingLineCompanyId).ToListAsync();
        return await _neginDbContext.ShippingLines.Include(c => c.Agents).AsNoTracking().Where(c => agentIds.Contains(c.Id)).ToListAsync();
    }

	public async Task<IList<ShippingLineCompany>> GetAllPorterageCompanies()
	{
        return await _neginDbContext.ShippingLines.AsNoTracking().Where(c => c.IsPorterage).ToListAsync();
	}

	#endregion

	#region Voyage
	public async Task<SqlException> CreateVoyageAsync(Voyage newVoyage)
    {
        SqlException result = new SqlException();
        newVoyage = (Voyage)Util.TrimAllStringFields(newVoyage);
        newVoyage.CreateDate = DateTime.Now;
        newVoyage.CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();

        try
        {
            await _neginDbContext.Voyages.AddAsync(newVoyage);
            await _neginDbContext.SaveChangesAsync();
            result.State = true;
        }
        catch (UniqueConstraintException)
        {
            try
            {
                var existVoyage = await _neginDbContext.Voyages.Where(c => c.VesselId == newVoyage.VesselId && c.IsActive == true).SingleAsync();
                existVoyage.IsActive = false;
                _neginDbContext.Update(existVoyage);

                await _neginDbContext.Voyages.AddAsync(newVoyage);
                await _neginDbContext.SaveChangesAsync();
                result.State = true;
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = ex.Message;
            }
        }
        catch (Exception ex)
        {
            result.State = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<PagedData<Voyage>> GetPaginationVoyagesAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var voyages = _neginDbContext.Voyages
            .Include(v => v.Vessel)
            .Include(o => o.Owner)
            .Include(a => a.Agent)
            .Include(s => s.VesselStoppages).AsNoTracking()
                            .Where(c => c.IsActive)
                            .Where(c => noFilter || c.Owner.ShippingLineName.Contains(filter)
                                                 || c.Agent.ShippingLineName.Contains(filter)
                                                 || c.Vessel.Name.Contains(filter));

        PagedData<Voyage> result = new()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await voyages.CountAsync()
            },
            Data = await voyages.OrderBy(x => x.Vessel.Name)
        .ToPagination(pageNumber, pageSize)
        .ToListAsync()
        };

        return result;
    }


    public async Task<SqlException> UpdateVoyageAsync(Voyage v)
    {
        var result = new SqlException() { State = false };

        var voyage = await _neginDbContext.Voyages.SingleAsync(c => c.Id == v.Id);
        if (voyage != null)
        {
            v = (Voyage)Util.TrimAllStringFields(v);

            voyage.VesselId = v.VesselId;
            voyage.OwnerId = v.OwnerId;
            voyage.AgentId = v.AgentId;
            voyage.ModifiedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
            voyage.ModifiedDate = DateTime.Now;

            try
            {
                _neginDbContext.Voyages.Update(voyage);
                _neginDbContext.SaveChanges();
                result.State = true;
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = ex.Message;
            }
        }

        return result;
    }

    public BLMessage ToggleVoyageStatus(ulong id)
    {
        var result = new BLMessage() { State = false };
        try
        {
            var voyage = _neginDbContext.Voyages.Include(c => c.VesselStoppages).ThenInclude(c => c.VesselStoppageInvoiceDetail)
                .Include(c => c.Vessel).SingleOrDefault(c => c.Id == id);
            if (voyage != null)
            {
                foreach (VesselStoppage vesselStoppage in voyage.VesselStoppages)
                {
                    vesselStoppage.SetStatus();
                }
                if (!voyage.VesselStoppages.Any(c => c.Status == VesselStoppage.VesselStoppageStatus.Gone || c.Status == VesselStoppage.VesselStoppageStatus.InProcess))
                {
                    if (voyage.VesselStoppages != null && !voyage.VesselStoppages.Any(c => c.ATA != null && c.ATD == null))
                    {
                        voyage.IsDelete = !voyage.IsDelete;
                        _neginDbContext.Voyages.Update(voyage);
                        _neginDbContext.SaveChanges();
                        result.State = true;
                        result.Message = "Vessel: " + voyage?.Vessel?.Name;
                    }
                }
                else
                {
                    result.State = false;
                    result.Message = "This voyage can`t change status! because there is atleast a vesselStoppage with Gone or InProcess status in it";
                }
            }
        }
        catch (Exception ex)
        {
            result.State = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<Voyage> GetVoyageById(ulong id)
    {
        return await _neginDbContext.Voyages.Include(c => c.Vessel).Include(c => c.Owner).AsNoTracking().SingleAsync(c => c.Id == id);
    }

    public async Task<Voyage> GetVoyageByVesselId(ulong vesselId)
    {
        return await _neginDbContext.Voyages.AsNoTracking().Where(c => c.IsDelete == false && c.IsActive == true).FirstOrDefaultAsync(c => c.VesselId == vesselId);
    }

    public async Task<DashboardDto> GetAllVoyageForDashboard()
    {
        var vesselStoppages = await _neginDbContext.VesselStoppages
                                        .Include(c => c.Voyage)
                                        .Include(c => c.VesselStoppageInvoiceDetail)
                                        .ToListAsync();

        vesselStoppages.ForEach(v => v.SetStatus());

        DashboardDto result = new()
        {
            ActiveVoyages = (uint)(await _neginDbContext.Voyages.Where(c => c.IsDelete == false && c.IsActive == true).CountAsync()),
            Gone = (uint)vesselStoppages.Where(c => c.Status == VesselStoppage.VesselStoppageStatus.Gone ||
                                                                                c.Status == VesselStoppage.VesselStoppageStatus.Invoiced).Count(),
            InProcess = (uint)vesselStoppages.Where(c => c.Status == VesselStoppage.VesselStoppageStatus.InProcess).Count(),
            WaitForVessel = (uint)vesselStoppages.Where(c => c.Status == VesselStoppage.VesselStoppageStatus.WaitForVessel).Count(),
            VesselStoppageCount = (uint)vesselStoppages.Where(c => c.Voyage.IsDelete == false && c.Voyage.IsActive == true && c.Status != VesselStoppage.VesselStoppageStatus.Gone ||
                                                                                c.Status == VesselStoppage.VesselStoppageStatus.Invoiced).Count(),
            Invoiced = (uint)vesselStoppages
                .Where(c => c.Status == VesselStoppage.VesselStoppageStatus.Invoiced).Count(),
            NotInvoiced = (uint)vesselStoppages
                .Where(c => c.Voyage.IsDelete == false && c.Voyage.IsActive == true && c.Status != VesselStoppage.VesselStoppageStatus.Invoiced).Count(),
        };

        return result;
    }

    #endregion

    #region	Currency
    public async Task<SqlException> CreateCurrencyAsync(Currency newCurrency)
	{
		SqlException result = new SqlException() { State = false };
		Currency currency = new()
		{
			ForeignDollerRate = newCurrency.ForeignDollerRate,
			PersianDollerRate = newCurrency.PersianDollerRate,
			Date = newCurrency.Date.ShamsiToMiladi(),
            CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId(),
            CreateDate = DateTime.Now

        };
		try
		{
			await _neginDbContext.AddAsync(currency);
			await _neginDbContext.SaveChangesAsync();
			result.State = true;
		}
		catch (Exception ex)
		{
			result.State = false;
			result.Message = ex.Message;
		}

		return result;
	}

	public async Task<PagedData<Currency>> GetPaginationCurrenciesAsync(int pageNumber = 1, int pageSize = 10)
	{
		var currnecies = _neginDbContext.Currencies.OrderByDescending(c => c.Date).AsNoTracking();

		PagedData<Currency> result = new()
		{
			PageInfo = new()
			{
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = await currnecies.CountAsync()
			},
			Data = await currnecies.ToPagination(pageNumber, pageSize)
								.ToListAsync()
		};

		return result;
	}

	#endregion

	#region	VesselStoppage Tariff
	public async Task<SqlException> CreateVesselStoppageTariffAsync(VesselStoppageTariff newVesselStoppageTariff)
    {
		SqlException result = new SqlException() { State = false };
		VesselStoppageTariff vesselStoppageTariff = new()
		{
			Description = newVesselStoppageTariff.Description.Trim(),
			EffectiveDate = newVesselStoppageTariff.EffectiveDate.ShamsiToMiladi(),
			CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId(),
			CreateDate = DateTime.Now

		};
		try
		{
			await _neginDbContext.AddAsync(vesselStoppageTariff);
			await _neginDbContext.SaveChangesAsync();
			result.State = true;
			result.sqlResult = vesselStoppageTariff.Id;
		}
		catch (Exception ex)
		{
			result.State = false;
			result.Message = ex.Message;
		}

		return result;
	}

	public async Task<PagedData<VesselStoppageTariff>> GetPaginationVesselStoppageTariffAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

		var tarrifs = _neginDbContext.VesselStoppageTariffs.Include(c=>c.VesselStoppageTariffDetails).AsNoTracking()
						  .Where(x => noFilter || x.Description.Contains(filter));

		PagedData<VesselStoppageTariff> result = new()
		{
			PageInfo = new()
			{
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = await tarrifs.Where(x => x.VesselStoppageTariffDetails.Count > 0).CountAsync()
			},
			Data = await tarrifs.Where(x => x.VesselStoppageTariffDetails.Count > 0).OrderByDescending(x => x.EffectiveDate)
									.ToPagination(pageNumber, pageSize)
									.ToListAsync()
		};

        return result;
    }

	public async Task<SqlException> CreateVesselStoppageTariffDetailsAsync(List<VesselStoppageTariffDetails> newVesselStoppageTariffDetails)
	{
		SqlException result = new SqlException() { State = false };

		try
		{
			await _neginDbContext.AddRangeAsync(newVesselStoppageTariffDetails);
			await _neginDbContext.SaveChangesAsync();
			result.State = true;
		}
		catch (Exception ex)
		{
			result.State = false;
			result.Message = ex.Message;
		}

		return result;
	}

    public async Task<VesselStoppageTariff> GetAllVesselStoppageTariffDetailAsync(int id)
    {
		return await _neginDbContext.VesselStoppageTariffs.Include(c => c.VesselStoppageTariffDetails).ThenInclude(c => c.VesselType).AsNoTracking()
								.SingleAsync(c=>c.Id == id);			
    }

	#endregion

	#region CleaningService Tariff
	public async Task<PagedData<CleaningServiceTariff>> GetPaginationCleaningServiceTariffAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
	{
		bool noFilter = string.IsNullOrWhiteSpace(filter);

		var tarrifs = _neginDbContext.CleaningServiceTariffs.Include(c => c.CleaningServiceTariffDetails).AsNoTracking()
						  .Where(x => noFilter || x.Description.Contains(filter));

		PagedData<CleaningServiceTariff> result = new()
		{
			PageInfo = new()
			{
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = await tarrifs.Where(x => x.CleaningServiceTariffDetails.Count > 0).CountAsync()
			},
			Data = await tarrifs.Where(x => x.CleaningServiceTariffDetails.Count > 0).OrderByDescending(x => x.EffectiveDate)
									.ToPagination(pageNumber, pageSize)
									.ToListAsync()
		};

		return result;

	}

	public async Task<CleaningServiceTariff> GetAllCleaningServiceTariffDetailAsync(int id)
	{
		return await _neginDbContext.CleaningServiceTariffs.Include(c => c.CleaningServiceTariffDetails).AsNoTracking()
						.SingleAsync(c => c.Id == id);

	}

	public async Task<SqlException> CreateCleaningServiceTariffAsync(CleaningServiceTariff newCleaningServiceTariff)
	{
		SqlException result = new SqlException() { State = false };
		CleaningServiceTariff cleaningServiceTariff = new()
		{
			Description = newCleaningServiceTariff.Description.Trim(),
			EffectiveDate = newCleaningServiceTariff.EffectiveDate.ShamsiToMiladi(),
			CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId(),
			CreateDate = DateTime.Now

		};
		try
		{
			await _neginDbContext.AddAsync(cleaningServiceTariff);
			await _neginDbContext.SaveChangesAsync();
			result.State = true;
			result.sqlResult = cleaningServiceTariff.Id;
		}
		catch (Exception ex)
		{
			result.State = false;
			result.Message = ex.Message;
		}

		return result;

	}

	public async Task<SqlException> CreateCleaningServiceTariffDetailsAsync(List<CleaningServiceTariffDetails> newCleaningServiceTariffDetail)
	{
		SqlException result = new SqlException() { State = false };

		try
		{
			await _neginDbContext.AddRangeAsync(newCleaningServiceTariffDetail);
			await _neginDbContext.SaveChangesAsync();
			result.State = true;
		}
		catch (Exception ex)
		{
			result.State = false;
			result.Message = ex.Message;
		}

		return result;

	}
    #endregion

    #region LoadingDischargeTariff
    public async Task<SqlException> CreateLoadingDischargeTariffAsync(LoadingDischargeTariff newLoadingDischargeTariff)
    {
        SqlException result = new SqlException() { State = false };
		LoadingDischargeTariff loadingDischargeTariff = new()
		{
			Description = newLoadingDischargeTariff.Description?.Trim(),
			EffectiveDate = newLoadingDischargeTariff.EffectiveDate.ShamsiToMiladi(),
			CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId(),
			CreateDate = DateTime.Now
		};
		try
		{
			await _neginDbContext.AddAsync(loadingDischargeTariff);
			await _neginDbContext.SaveChangesAsync();
			result.State = true;
			result.sqlResult = loadingDischargeTariff.Id;
		}
		catch (Exception ex)
		{
            result.State = false;
            result.Message = ex.Message;
        }

		return result;
    }

    public async Task<SqlException> CreateLoadingDischargeTariffDetailsAsync(List<LoadingDischargeTariffDetails> newLoadingDischargeTariffDetails)
    {
        SqlException result = new SqlException() { State = false };
        try
        {
            await _neginDbContext.AddRangeAsync(newLoadingDischargeTariffDetails);
            await _neginDbContext.SaveChangesAsync();
            result.State = true;
        }
        catch (Exception ex)
        {
            result.State = false;
            result.Message = ex.Message;
        }

        return result;
    }

    public async Task<PagedData<LoadingDischargeTariff>> GetPaginationLoadingDischargeTariffAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

		var tariffs = _neginDbContext.LoadingDischargeTariffs.Include(c=>c.LoadingDischargeTariffDetails).AsNoTracking()
                            .Where(x => noFilter || x.Description.Contains(filter));

        PagedData<LoadingDischargeTariff> result = new()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await tariffs.Where(x => x.LoadingDischargeTariffDetails.Count > 0).CountAsync()
            },
            Data = await tariffs.Where(x => x.LoadingDischargeTariffDetails.Count > 0).OrderByDescending(x => x.EffectiveDate)
                            .ToPagination(pageNumber, pageSize)
                            .ToListAsync()
        };

        return result;

    }

	public async Task<LoadingDischargeTariff> GetLoadingDischargeTariffByIdAsync(int id)
	{
		return await _neginDbContext.LoadingDischargeTariffs.Include(c => c.LoadingDischargeTariffDetails).AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
	}

    public async Task<List<LoadingDischargeTariffDetails>> GetAllLoadingDischargeTariffDetailsAsync()
    {
        return await _neginDbContext.LoadingDischargeTariffs
            .Include(c => c.LoadingDischargeTariffDetails)
            .AsNoTracking()
            .OrderByDescending(c => c.EffectiveDate)
            .Take(1)
            .SelectMany(c=>c.LoadingDischargeTariffDetails)
            .Where(c => c.Goods != null)
            .ToListAsync();
    }


	#endregion
}

