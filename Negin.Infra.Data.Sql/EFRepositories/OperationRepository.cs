using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Entities.Billing;
using Negin.Core.Domain.Entities.Operation;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Framework.JoinExtentions;
using Negin.Infrastructure;
using Microsoft.AspNetCore.Http;
using Negin.Framework.Exceptions;
using System.Collections.Generic;

namespace Negin.Infra.Data.Sql.EFRepositories;
public class OperationRepository : IOperationRepository
{
    private readonly NeginDbContext _neginDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public OperationRepository(NeginDbContext neginDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _neginDbContext = neginDbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    #region VesselStoppage
    public async Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesAsync(string vesselName, int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var vesselStoppages = _neginDbContext.VesselStoppages
                                .Include(c => c.Voyage)
                                .Include(c => c.VesselStoppageInvoiceDetail)
                                .Include(c => c.OriginPort)
                                .Include(c => c.PreviousPort)
                                .Include(c => c.NextPort).AsNoTracking()
                                        .Where(c => c.Voyage.Vessel.Name == vesselName)
                                        .Where(c => noFilter
                                            || c.OriginPort.PortName.Contains(filter) || c.OriginPort.PortSymbol.Contains(filter)
                                            || c.PreviousPort.PortName.Contains(filter) || c.PreviousPort.PortSymbol.Contains(filter)
                                            || c.NextPort.PortName.Contains(filter) || c.NextPort.PortSymbol.Contains(filter)
                                            || c.VoyageNoIn.Contains(filter));


        PagedData<VesselStoppage> result = new()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await vesselStoppages.CountAsync()
            },
            Data = await vesselStoppages
                                .ToPagination(pageNumber, pageSize)
                                .OrderBy(c => c.ATD)
                                .ToListAsync()
        };
        result.Data.ForEach(x =>
        {
            x.ETADayOfTheWeek = x.ETA?.DayOfWeek; x.ATADayOfTheWeek = x.ATA?.DayOfWeek; x.ETDDayOfTheWeek = x.ETD?.DayOfWeek; x.ATDDayOfTheWeek = x.ATD?.DayOfWeek;
            x.ETA = x.ETA?.MiladiToShamsi(); x.ATA = x.ATA?.MiladiToShamsi(); x.ETD = x.ETD?.MiladiToShamsi(); x.ATD = x.ATD?.MiladiToShamsi();
        });
        SetStatus(result.Data);
        return result;
    }

    public async Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesForInvoiceAsync(ulong voyageId, int pageNumber = 1, int pageSize = 10)
    {
        var fullOuterjoin = _neginDbContext.VesselStoppages
            .Include(c => c.OriginPort)
            .Include(c => c.NextPort)
            .Include(c => c.PreviousPort).AsNoTracking()
            .Where(c => c.VoyageId == voyageId).FullOuterJoin
            (
                await _neginDbContext.VesselStoppageInvoiceDetails.Include(c => c.Invoice).AsNoTracking().Where(c => c.Invoice.VoyageId == voyageId && c.Invoice.Status != Invoice.InvoiceStatus.IsCancel).ToListAsync(),
                v => v.Id,
                i => i.VesselStoppageId,
                (v, i) => new { v, i }
            )
            .Where(vi => vi.v == null || vi.i == null);

        PagedData<VesselStoppage> result = new()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = fullOuterjoin.Count()
            },
            Data = fullOuterjoin.OrderBy(x => x.v.Id).Select(x => x.v)
                                .ToPagination(pageNumber, pageSize)
                                .ToList()
        };
        result.Data.ForEach(x =>
        {
            x.ETADayOfTheWeek = x.ETA?.DayOfWeek; x.ATADayOfTheWeek = x.ATA?.DayOfWeek; x.ETDDayOfTheWeek = x.ETD?.DayOfWeek; x.ATDDayOfTheWeek = x.ATD?.DayOfWeek;
            x.ETA = x.ETA?.MiladiToShamsi(); x.ATA = x.ATA?.MiladiToShamsi(); x.ETD = x.ETD?.MiladiToShamsi(); x.ATD = x.ATD?.MiladiToShamsi();
        });
        SetStatus(result.Data);
        result.Data = result.Data.OrderByDescending(c => c.Status).ToList();
        return result;
    }

    public async Task<SqlException> CreateVesselStoppageAsync(VesselStoppage newVesselStoppage)
    {
        SqlException result = new SqlException();
        VesselStoppage vesselStoppage = new()
        {
            ETA = newVesselStoppage.ETA,
            ATA = newVesselStoppage.ATA,
            ETD = newVesselStoppage.ETD,
            ATD = newVesselStoppage.ATD,
            VoyageId = newVesselStoppage.VoyageId,
            OriginPortId = newVesselStoppage.OriginPortId,
            PreviousPortId = newVesselStoppage.PreviousPortId,
            NextPortId = newVesselStoppage.NextPortId,
            VoyageNoIn = newVesselStoppage.VoyageNoIn,
            StartStorm = newVesselStoppage.StartStorm,
            EndStorm = newVesselStoppage.EndStorm,
            CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId(),
            CreateDate = DateTime.Now
        };
        try
        {
            await _neginDbContext.VesselStoppages.AddAsync(vesselStoppage);
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

    public async Task<SqlException> UpdateVesselStoppageAsync(VesselStoppage v)
    {
        var result = new SqlException() { State = false };

        var vesselStoppage = await _neginDbContext.VesselStoppages.SingleOrDefaultAsync(c => c.Id == v.Id);
        if (vesselStoppage != null)
        {
            if ((vesselStoppage.ATA != null && v.ATA == null) || (vesselStoppage.ATD != null && v.ATD == null))
            {
                result.Message = "You can`t remove a Actual Time!";
                result.State = false;
                return result;
            }
            vesselStoppage.ETA = v.ETA;
            vesselStoppage.ATA = v.ATA;
            vesselStoppage.ETD = v.ETD;
            vesselStoppage.ATD = v.ATD;
            vesselStoppage.OriginPortId = v.OriginPortId;
            vesselStoppage.PreviousPortId = v.PreviousPortId;
            vesselStoppage.NextPortId = v.NextPortId;
            vesselStoppage.VoyageNoIn = v.VoyageNoIn;
            vesselStoppage.StartStorm = v.StartStorm;
            vesselStoppage.EndStorm = v.EndStorm;
            vesselStoppage.ModifiedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
            vesselStoppage.ModifiedDate = DateTime.Now;

            try
            {
                _neginDbContext.VesselStoppages.Update(vesselStoppage);
                _neginDbContext.SaveChanges();
                result.State = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.State = false;
            }

        }
        return result;
    }

    public async Task<VesselStoppage> GetVesselStoppageByVoyageId(ulong id)
    {
        var vesselStoppage = await _neginDbContext.VesselStoppages
            .Include(c => c.OriginPort).Include(c => c.PreviousPort).Include(c => c.NextPort).AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id);

        if (vesselStoppage != null)
        {
            if (vesselStoppage.ETA.HasValue)
            {
                vesselStoppage.ETATime = vesselStoppage.ETA.Value.Hour.ToString("00") + ":" + vesselStoppage.ETA.Value.Minute.ToString("00");
                vesselStoppage.ETA = vesselStoppage.ETA.Value.Date.MiladiToShamsi();
            }
            if (vesselStoppage.ATA.HasValue)
            {
                vesselStoppage.ATATime = vesselStoppage.ATA.Value.Hour.ToString("00") + ":" + vesselStoppage.ATA.Value.Minute.ToString("00");
                vesselStoppage.ATA = vesselStoppage.ATA.Value.Date.MiladiToShamsi();
            }
            if (vesselStoppage.ETD.HasValue)
            {
                vesselStoppage.ETDTime = vesselStoppage.ETD.Value.Hour.ToString("00") + ":" + vesselStoppage.ETD.Value.Minute.ToString("00");
                vesselStoppage.ETD = vesselStoppage.ETD.Value.Date.MiladiToShamsi();
            }
            if (vesselStoppage.ATD.HasValue)
            {
                vesselStoppage.ATDTime = vesselStoppage.ATD.Value.Hour.ToString("00") + ":" + vesselStoppage.ATD.Value.Minute.ToString("00");
                vesselStoppage.ATD = vesselStoppage.ATD.Value.Date.MiladiToShamsi();
            }
            if (vesselStoppage.StartStorm.HasValue)
            {
                vesselStoppage.StartStormTime = vesselStoppage.StartStorm.Value.Hour.ToString("00") + ":" + vesselStoppage.StartStorm.Value.Minute.ToString("00");
                vesselStoppage.StartStorm = vesselStoppage.StartStorm.Value.Date.MiladiToShamsi();
            }
            if (vesselStoppage.EndStorm.HasValue)
            {
                vesselStoppage.EndStormTime = vesselStoppage.EndStorm.Value.Hour.ToString("00") + ":" + vesselStoppage.EndStorm.Value.Minute.ToString("00");
                vesselStoppage.EndStorm = vesselStoppage.EndStorm.Value.Date.MiladiToShamsi();
            }
        }
        return vesselStoppage;
    }

    #endregion

    #region Private Methods
    private static void SetStatus(IEnumerable<VesselStoppage> vesselStoppages)
    {
        foreach (var vesselStoppage in vesselStoppages)
        {
            if (vesselStoppage != null)
            {
                vesselStoppage.SetStatus();
            }
        }
    }

    #endregion

    #region LoadingDischarge
    public async Task<List<VesselStoppage>> GetAllVSForLoadAndDischargeAsync()
    {
        return await _neginDbContext.VesselStoppages.AsNoTracking().OrderByDescending(c => c.ATA).ToListAsync();
    }

    public async Task<SqlException> CreateLoadingDischargeAsync(LoadingDischarge loadingDischarge)
    {
        SqlException result = new SqlException() { State = false };
        loadingDischarge.CreateDate = DateTime.Now;
        loadingDischarge.CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
        try
        {
            await _neginDbContext.LoadingDischarges.AddAsync(loadingDischarge);
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

    public async Task<PagedData<LoadingDischarge>> GetPaginationLoadingDischargeAsync(ulong vesselStoppageId, int pageNumber = 1, int pageCount = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var loadingDischarges = _neginDbContext.LoadingDischarges.AsNoTracking()
                                                        .Include(c => c.VesselStoppage)
                                                        .Include(c => c.LoadingDischargeTariffDetail)
                                                        .Where(c => c.VesselStoppageId == vesselStoppageId)
                                                        .Where(c => c.IsDelete == false)
                                                        .Where(c => noFilter || c.Tonage == Convert.ToDouble(filter));

        PagedData<LoadingDischarge> result = new()
        {
            PageInfo = new PageInfo()
            {
                PageNumber = pageNumber,
                PageSize = pageCount,
                TotalCount = await loadingDischarges.CountAsync()
            },
            Data = await loadingDischarges.ToPagination(pageNumber, pageCount).OrderByDescending(c => c.CreateDate).ToListAsync()
        };

        result.Data.ForEach(x =>
        {
            x.VesselStoppage.ATADayOfTheWeek = x.VesselStoppage.ATA?.DayOfWeek;
            x.VesselStoppage.ATA = x.VesselStoppage.ATA?.MiladiToShamsi();
        });

        return result;
    }

    public async Task<LoadingDischarge> GetLoadingDischargeById(ulong id)
    {
        return await _neginDbContext.LoadingDischarges.Include(c => c.VesselStoppage).Include(c => c.LoadingDischargeTariffDetail).AsNoTracking().SingleAsync(c => c.Id == id);
    }

    public async Task<SqlException> UpdateLoadingDischargeAsync(LoadingDischarge newLoadingDischarge)
    {
        var result = new SqlException() { State = false };

        var loadingDischarge =  await _neginDbContext.LoadingDischarges.SingleAsync(c => c.Id == newLoadingDischarge.Id);
        
        loadingDischarge.Tonage = newLoadingDischarge.Tonage;
        loadingDischarge.LoadingDischargeTariffDetailId = newLoadingDischarge.LoadingDischargeTariffDetailId;
        loadingDischarge.HasCrane = newLoadingDischarge.HasCrane;
        loadingDischarge.HasInventory = newLoadingDischarge.HasInventory;
        loadingDischarge.ModifiedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
        loadingDischarge.ModifiedDate = DateTime.Now;
        try
        {
            _neginDbContext.Update(loadingDischarge);
            await _neginDbContext.SaveChangesAsync();
            result.State = true;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.State = false;
        }

        return result;
    }

    public async Task<SqlException> DeleteLoadingDischargeAsync(ulong id)
    {
        var result = new SqlException() { State = false };
        var loadingDischarge = await _neginDbContext.LoadingDischarges.SingleAsync(c => c.Id == id);
        loadingDischarge.IsDelete = true;
        loadingDischarge.ModifiedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
        loadingDischarge.ModifiedDate = DateTime.Now;

        try
        {
            _neginDbContext.Update(loadingDischarge);
            await _neginDbContext.SaveChangesAsync();
            result.State = true;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            result.State = false;
        }

        return result;

    }

	public async Task<PagedData<LoadingDischarge>> GetPaginationLoadingDischargeForInvoicingAsync(int pageNumber = 1, int pageCount = 10, string filter = "")
	{
		bool noFilter = string.IsNullOrWhiteSpace(filter);

        var loadingDischarges = _neginDbContext.LoadingDischarges
                                            .Include(c => c.VesselStoppage).ThenInclude(c => c.Voyage).ThenInclude(c => c.Vessel)
                                            .Include(c => c.LoadingDischargeTariffDetail).AsNoTracking()
                                            .Where(c => c.IsDelete == false)
                                            .Where(c => noFilter || c.VesselStoppage.VoyageNoIn.Contains(filter)
                                                                 || c.VesselStoppage.Voyage.Vessel.Name.Contains(filter));

        var result = new PagedData<LoadingDischarge>()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageCount,
                TotalCount = await loadingDischarges.CountAsync()
            },
            Data = await loadingDischarges.ToListAsync()
        };

        foreach (var loadingDischarge in result.Data)
        {
            if (_neginDbContext.LoadingDischargeInvoices.Any(c => c.LoadingDischargeId == loadingDischarge.Id))
            {
                loadingDischarge.Status = LoadingDischarge.LoadingDischargeStatus.Invoiced;
            }
            else
            {
                loadingDischarge.Status = LoadingDischarge.LoadingDischargeStatus.notInvoiced;
            }
        }

        return result;
	}


	#endregion
}
