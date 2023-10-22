using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;

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
}
