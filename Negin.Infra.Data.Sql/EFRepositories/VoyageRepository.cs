using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;
using static Negin.Framework.Exceptions.SqlException;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Entities.Basic;
using Negin.Framework.JoinExtentions;
using Negin.Core.Domain.Dtos;
using System.Linq;
using Microsoft.IdentityModel.Tokens;
using Negin.Core.Domain.Aggregates.Billing;

namespace Negin.Infra.Data.Sql.EFRepositories;

public class VoyageRepository : IVoyageRepository
{
    private readonly NeginDbContext _neginDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VoyageRepository(NeginDbContext neginDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _neginDbContext = neginDbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<SqlException> CreateVoyageAsync(Voyage newVoyage)
    {
        SqlException result = new SqlException();
        newVoyage = (Voyage)Util.TrimAllStringFields(newVoyage);

		var voyage = new Voyage()
        {
            VesselId = newVoyage.VesselId,
            OwnerId = newVoyage.OwnerId,
            AgentId = newVoyage.AgentId,
            CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId(),
            CreateDate = DateTime.Now
        };
        try
        {
            await _neginDbContext.Voyages.AddAsync(voyage);
            await _neginDbContext.SaveChangesAsync();
			result.State = true;
		}
		catch (UniqueConstraintException ex)
        {
            result.SqlState = SqlExceptionState.DuplicateName;
            result.Message = "(Vessel Number In + Vessel) must be unique!";
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

    public async Task<PagedData<Tuple<VesselStoppage,Voyage>>> GetPaginationDataForBillingAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var vesselStoppages = _neginDbContext.VesselStoppages
                .Include(c => c.Voyage).ThenInclude(c => c.Vessel)
                .Include(c => c.Voyage).ThenInclude(c => c.Owner)
                .Include(c => c.Voyage).ThenInclude(c => c.Agent)
                .Include(c => c.VesselStoppageInvoiceDetail).AsNoTracking()
                    .Where(c => noFilter || c.Voyage.Vessel.Name.Contains(filter)
                                            || c.VoyageNoIn.Contains(filter)
                                            || c.Voyage.Owner.ShippingLineName.Contains(filter)
                                            || c.Voyage.Agent.ShippingLineName.Contains(filter));


        List<Tuple<VesselStoppage, Voyage>> list = new List<Tuple<VesselStoppage, Voyage>>();
        
        foreach (VesselStoppage vesselStoppage in await vesselStoppages.OrderByDescending(c => c.CreateDate).ToListAsync())
        {
            vesselStoppage.SetStatus();
            if(vesselStoppage.Status != VesselStoppage.VesselStoppageStatus.Invoiced)
            {
                list.Add(new Tuple<VesselStoppage, Voyage>(vesselStoppage, vesselStoppage.Voyage));
            }
        }
        PagedData<Tuple<VesselStoppage, Voyage>> result = new()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = list.Count
            },
            Data = list
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
            var voyage = _neginDbContext.Voyages.Include(c => c.VesselStoppages).ThenInclude(c=>c.VesselStoppageInvoiceDetail)
                .Include(c => c.Vessel).SingleOrDefault(c => c.Id == id);
            if (voyage != null)
            {
                foreach (VesselStoppage vesselStoppage in voyage.VesselStoppages)
                {
                    vesselStoppage.SetStatus();
                }
                if (!voyage.VesselStoppages.Any(c=>c.Status == VesselStoppage.VesselStoppageStatus.Gone || c.Status == VesselStoppage.VesselStoppageStatus.InProcess))
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
        return await _neginDbContext.Voyages.Include(c => c.Vessel).AsNoTracking().SingleAsync(c => c.Id == id);
    }

    public async Task<Voyage> GetVoyageByVesselId(ulong vesselId)
    {
        return await _neginDbContext.Voyages.AsNoTracking().Where(c => c.IsDelete == false).FirstOrDefaultAsync(c => c.VesselId == vesselId);
    }

    public async Task<IList<Port>> GetAllPorts()
    {
        return await _neginDbContext.Ports.Include(c => c.Country).AsNoTracking().ToListAsync();
    }

    public async Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesAsync(ulong voyageId, int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var vesselStoppages = _neginDbContext.VesselStoppages.Include(c => c.VesselStoppageInvoiceDetail).Include(c => c.OriginPort).Include(c => c.PreviousPort).Include(c => c.NextPort).AsNoTracking()
                                        .Where(c => c.VoyageId == voyageId)
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
            Data = await vesselStoppages.OrderBy(x => x.Id)
                                .ToPagination(pageNumber, pageSize)
                                .ToListAsync()
        };
        result.Data.ForEach(x => 
        {
            x.ETADayOfTheWeek = x.ETA?.DayOfWeek; x.ATADayOfTheWeek = x.ATA?.DayOfWeek; x.ETDDayOfTheWeek = x.ETD?.DayOfWeek; x.ATDDayOfTheWeek = x.ATD?.DayOfWeek;
            x.ETA = x.ETA?.MiladiToShamsi(); x.ATA = x.ATA?.MiladiToShamsi(); x.ETD = x.ETD?.MiladiToShamsi(); x.ATD = x.ATD?.MiladiToShamsi();
        });
        SetStatus(result.Data);
        result.Data = result.Data.OrderByDescending(c=>c.Status).ToList();
        return result;
    }

    public async Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesForInvoiceAsync(ulong voyageId, int pageNumber = 1, int pageSize = 10)
    {
        var fullOuterjoin = _neginDbContext.VesselStoppages.Include(c => c.OriginPort).Include(c => c.NextPort).Include(c => c.PreviousPort).AsNoTracking()
            .Where(c => c.VoyageId == voyageId).FullOuterJoin
            (
                await _neginDbContext.VesselStoppageInvoiceDetails.Include(c => c.Invoice).AsNoTracking().Where(c => c.Invoice.VoyageId == voyageId && c.Invoice.Status != Core.Domain.Aggregates.Billing.Invoice.InvoiceStatus.IsCancel).ToListAsync(),
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
            Data =  fullOuterjoin.OrderBy(x => x.v.Id).Select(x => x.v)
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

    public async Task<VesselStoppage> GetVesselStoppageByVoyageId(ulong id)
    {
        var vesselStoppage = await _neginDbContext.VesselStoppages
            .Include(c => c.OriginPort).Include(c => c.PreviousPort).Include(c => c.NextPort).AsNoTracking()
            .SingleOrDefaultAsync(c => c.Id == id);

        if (vesselStoppage != null)
        {
            if (vesselStoppage.ETA.HasValue)
            {
                vesselStoppage.ETATime = vesselStoppage.ETA.Value.Hour.ToString() + ":" + vesselStoppage.ETA.Value.Minute.ToString();
                vesselStoppage.ETA = vesselStoppage.ETA.Value.Date.MiladiToShamsi();
            }
            if (vesselStoppage.ATA.HasValue)
            {
                vesselStoppage.ATATime = vesselStoppage.ATA.Value.Hour.ToString() + ":" + vesselStoppage.ATA.Value.Minute.ToString();
                vesselStoppage.ATA = vesselStoppage.ATA.Value.Date.MiladiToShamsi();
            }
            if (vesselStoppage.ETD.HasValue)
            {
                vesselStoppage.ETDTime = vesselStoppage.ETD.Value.Hour.ToString() + ":" + vesselStoppage.ETD.Value.Minute.ToString();
                vesselStoppage.ETD = vesselStoppage.ETD.Value.Date.MiladiToShamsi();
            }
            if (vesselStoppage.ATD.HasValue)
            {
                vesselStoppage.ATDTime = vesselStoppage.ATD.Value.Hour.ToString() + ":" + vesselStoppage.ATD.Value.Minute.ToString();
                vesselStoppage.ATD = vesselStoppage.ATD.Value.Date.MiladiToShamsi();
            }
        }
        return vesselStoppage;
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

    public async Task<DashboardDto> GetAllVoyageForDashboard()
    {
        var voyages = await _neginDbContext.Voyages
                        .Include(v => v.Vessel)
                        .Include(s => s.VesselStoppages).ThenInclude(i => i.VesselStoppageInvoiceDetail).ThenInclude(i => i.Invoice)
                        .Include(o => o.Owner).Include(a => a.Agent).AsNoTracking()
                            .Where(c => c.IsDelete == false)
                            .ToListAsync();

        voyages.ForEach(v => { SetStatus(v.VesselStoppages); });

        DashboardDto result = new()
        {
            ActiveVoyages = (uint)voyages.Count,
            Gone = (uint)voyages.SelectMany(c => c.VesselStoppages).Where(c => c.Status == VesselStoppage.VesselStoppageStatus.Gone ||
                                                                                c.Status == VesselStoppage.VesselStoppageStatus.Invoiced).Count(),
            InProcess = (uint)voyages.SelectMany(c => c.VesselStoppages).Where(c => c.Status == VesselStoppage.VesselStoppageStatus.InProcess).Count(),
            WaitForVessel = (uint)voyages.SelectMany(c => c.VesselStoppages).Where(c => c.Status == VesselStoppage.VesselStoppageStatus.WaitForVessel).Count(),
            VesselStoppageCount = (uint)voyages.SelectMany(c => c.VesselStoppages).Where(c => c.Status == VesselStoppage.VesselStoppageStatus.Gone ||
                                                                                                c.Status == VesselStoppage.VesselStoppageStatus.Invoiced).Count(),
            Invoiced = (uint)voyages.SelectMany(c => c.VesselStoppages).Where(c => c.Status == VesselStoppage.VesselStoppageStatus.Invoiced).Count()
        };

        return result;
    }
}
