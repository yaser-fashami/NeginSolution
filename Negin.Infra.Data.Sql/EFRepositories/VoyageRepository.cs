using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;
using static Negin.Framework.Exceptions.SqlException;
using Microsoft.Extensions.Primitives;
using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using Negin.Core.Domain.Entities.Billing;

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
            VoyageNoIn = newVoyage.VoyageNoIn,
            VoyageNoOut = newVoyage.VoyageNoOut,
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
			result.State = SqlExceptionMessages.Success;
		}
		catch (UniqueConstraintException ex)
        {
            result.State = SqlExceptionMessages.DuplicateName;
            result.Message = "(Vessel Number In + Vessel) must be unique!";
        }
		catch (Exception ex)
        {
			result.State = SqlExceptionMessages.Fail;
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
            .Include(a => a.Agent).AsNoTracking()
                            .Where(c => noFilter || c.VoyageNoIn.Contains(filter)
                                                 || c.VoyageNoOut.Contains(filter)
                                                 || c.Owner.ShippingLineName.Contains(filter)
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
        var result = new SqlException() { State = SqlExceptionMessages.Fail };

        var voyage = await _neginDbContext.Voyages.SingleAsync(c => c.Id == v.Id);
        if (voyage != null)
        {
			v = (Voyage)Util.TrimAllStringFields(v);

            voyage.VoyageNoIn = v.VoyageNoIn;
            voyage.VoyageNoOut = v.VoyageNoOut;
            voyage.VesselId = v.VesselId;
            voyage.OwnerId = v.OwnerId;
            voyage.AgentId = v.AgentId;
            voyage.ModifiedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
            voyage.ModifiedDate = DateTime.Now;

            try
            {
                _neginDbContext.Voyages.Update(voyage);
                _neginDbContext.SaveChanges();
				result.State = SqlExceptionMessages.Success;
			}
            catch (Exception ex)
            {
				result.State = SqlExceptionMessages.Fail;
				result.Message = ex.Message;
			}
		}

		return result;
    }

    public void ToggleVoyageStatus(ulong id)
    {
        try
        {
            var voyage = _neginDbContext.Voyages.SingleOrDefault(c => c.Id == id);
            if (voyage != null)
            {
                voyage.IsDelete = !voyage.IsDelete;
                _neginDbContext.Voyages.Update(voyage);
                _neginDbContext.SaveChanges();
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<Voyage> GetVoyageById(ulong id)
    {
        return await _neginDbContext.Voyages.SingleAsync(c => c.Id == id);
    }

    public async Task<IList<Voyage>> GetVoyageByVesselId(ulong vesselId)
    {
        return await _neginDbContext.Voyages.AsNoTracking().Where(c => c.IsDelete == false).Where(c => c.VesselId == vesselId).ToListAsync();
    }

    public async Task<IList<Port>> GetAllPorts()
    {
        return await _neginDbContext.Ports.Include(c => c.Country).AsNoTracking().ToListAsync();
    }

    public async Task<PagedData<VesselStoppage>> GetPaginationVesselStoppagesAsync(ulong voyageId, int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var vesselStoppages = _neginDbContext.VesselStoppages.Include(c => c.OriginPort).Include(c => c.PreviousPort).Include(c => c.NextPort).AsNoTracking()
                                                                .Where(c => c.IsDelete == false)
                                                                .Where(c => c.VoyageId == voyageId)
                                                                .Where(c => noFilter
                                                                    || c.OriginPort.PortName.Contains(filter)
                                                                    || c.PreviousPort.PortName.Contains(filter)
                                                                    || c.NextPort.PortName.Contains(filter));

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
        result.Data.ForEach(x => { x.ETADayOfTheWeek = x.ETA?.DayOfWeek; x.ATADayOfTheWeek = x.ATA?.DayOfWeek; x.ETDDayOfTheWeek = x.ETD?.DayOfWeek; x.ATDDayOfTheWeek = x.ATD?.DayOfWeek; });
        result.Data.ForEach(x => { x.ETA = x.ETA?.MiladiToShamsi(); x.ATA = x.ATA?.MiladiToShamsi(); x.ETD = x.ETD?.MiladiToShamsi(); x.ATD = x.ATD?.MiladiToShamsi(); });
        return result;
    }

    public async Task<SqlException> CreateVesselStoppageAsync(VesselStoppage newVesselStoppage, IEnumerable<KeyValuePair<string, StringValues>> formCollection)
    {

        SqlException result = new SqlException();
        SetTimes(ref newVesselStoppage, formCollection);
        newVesselStoppage.ETA = newVesselStoppage.ETA?.ShamsiToMiladi();
        newVesselStoppage.ATA = newVesselStoppage.ATA?.ShamsiToMiladi();
        newVesselStoppage.ETD = newVesselStoppage.ETD?.ShamsiToMiladi();
        newVesselStoppage.ATD = newVesselStoppage.ATD?.ShamsiToMiladi();
        if (_neginDbContext.VesselStoppages.Any(c=>c.ATA < newVesselStoppage.ATA) || _neginDbContext.VesselStoppages.Any(c => c.ATD < newVesselStoppage.ATD) || _neginDbContext.VesselStoppages.Any(c => c.ATA > newVesselStoppage.ATD))
        {
            result.Message = "There is at least a vessel stoppage after this!";
            result.State = SqlExceptionMessages.Fail;
            return result;
        }
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
            CreatedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId(),
            CreateDate = DateTime.Now
        };
        try
        {
            await _neginDbContext.VesselStoppages.AddAsync(vesselStoppage);
            await _neginDbContext.SaveChangesAsync();
            result.State = SqlExceptionMessages.Success;
        }
        catch (Exception ex)
        {
            result.State = SqlExceptionMessages.Fail;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<VesselStoppage> GetVesselStoppageByVoyageId(ulong id)
    {
        var vesselStoppage = await _neginDbContext.VesselStoppages.Include(c => c.OriginPort).Include(c => c.PreviousPort).Include(c => c.NextPort).AsNoTracking().SingleOrDefaultAsync(c => c.Id == id);
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

    public async Task<SqlException> UpdateVesselStoppageAsync(VesselStoppage v, IEnumerable<KeyValuePair<string, StringValues>> formCollection)
    {
        var result = new SqlException() { State = SqlExceptionMessages.Fail };
        var vesselStoppage = await _neginDbContext.VesselStoppages.SingleOrDefaultAsync(c => c.Id == v.Id);
        if (vesselStoppage != null)
        {
            vesselStoppage.ETA = v.ETA?.ShamsiToMiladi();
            vesselStoppage.ATA = v.ATA?.ShamsiToMiladi();
            vesselStoppage.ETD = v.ETD?.ShamsiToMiladi();
            vesselStoppage.ATD = v.ATD?.ShamsiToMiladi();
            SetTimes(ref vesselStoppage, formCollection);
            if (vesselStoppage.ATA > DateTime.Now || vesselStoppage.ATD > DateTime.Now)
            {
                result.Message = "Actual Time must be past!";
                result.State = SqlExceptionMessages.Fail;
                return result;
            }
            vesselStoppage.OriginPortId = v.OriginPortId;
            vesselStoppage.PreviousPortId = v.PreviousPortId;
            vesselStoppage.NextPort = v.NextPort;
            vesselStoppage.ModifiedById = _httpContextAccessor.HttpContext.User.Identity?.GetCurrentUserId();
            vesselStoppage.ModifiedDate = DateTime.Now;

            try
            {
                _neginDbContext.VesselStoppages.Update(vesselStoppage);
                _neginDbContext.SaveChanges();
                result.State = SqlExceptionMessages.Success;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.State = SqlExceptionMessages.Fail;
            }

        }
        return result;
    }

    private void SetTimes(ref VesselStoppage newVesselStoppage, IEnumerable<KeyValuePair<string, StringValues>> formCollection)
    {
        foreach (var item in formCollection)
        {
            if (item.Key.Contains("ETATime") && newVesselStoppage.ETA != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                newVesselStoppage.ETA = AddTimeToDate(newVesselStoppage.ETA.Value, item);
            }
            if (item.Key.Contains("ATATime") && newVesselStoppage.ATA != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                newVesselStoppage.ATA = AddTimeToDate(newVesselStoppage.ATA.Value, item);
            }
            if (item.Key.Contains("ETDTime") && newVesselStoppage.ETD != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                newVesselStoppage.ETD = AddTimeToDate(newVesselStoppage.ETD.Value, item);
            }
            if (item.Key.Contains("ATDTime") && newVesselStoppage.ATD != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                newVesselStoppage.ATD = AddTimeToDate(newVesselStoppage.ATD.Value, item);
            }
        }
    }

    private DateTime? AddTimeToDate(DateTime newVesselStoppageDate, KeyValuePair<string, StringValues> item)
    {
        //TimeSpan time = new TimeSpan();
        //foreach (var eta in item.Value)
        //{
        //    if (TimeSpan.TryParse(eta.Substring(0, eta.Length > 2 ? eta.Length - 2 : eta.Length), out time) && eta.Contains("AM"))
        //    {
        //        time = time.Add(new TimeSpan(-12, 0, 0));
        //    }
        //    else if (eta.Contains("PM"))
        //    {
        //        time = time.Add(new TimeSpan(12, 0, 0));
        //    }
        //}
        return newVesselStoppageDate = newVesselStoppageDate + TimeSpan.Parse(item.Value);
    }

}
