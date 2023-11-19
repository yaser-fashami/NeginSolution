using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;

namespace Negin.Services.Operation;

public class VesselStoppageService : IVesselStoppageService
{
    private readonly NeginDbContext _neginDbContext;
    private readonly IVoyageRepository _voyageRepository;

    public VesselStoppageService(NeginDbContext neginDbContext, IVoyageRepository voyageRepository)
    {
        _neginDbContext = neginDbContext;
        _voyageRepository = voyageRepository;
    }

    public BLMessage AddVesselStoppage(VesselStoppage v, IFormCollection formCollection)
    {
        SetTimes(ref v, formCollection);

        var result = ValidateVesselStoppageAsync(v).Result;
        if (result.State)
        {
            v = DatesShamsiToMiladi(v);
            result = _voyageRepository.CreateVesselStoppageAsync(v).Result;
        }

        return result;
    }

    public BLMessage UpdateVesselStoppage(VesselStoppage v, IFormCollection formCollection)
    {
        SetTimes(ref v, formCollection);
        var result = ValidateVesselStoppageAsync(v).Result;
        if (result.State)
        {
            v = DatesShamsiToMiladi(v);
            result = _voyageRepository.UpdateVesselStoppageAsync(v).Result;
        }

        return result;
    }


    private VesselStoppage DatesShamsiToMiladi(VesselStoppage vesselStoppage)
    {
        VesselStoppage v = new VesselStoppage
        {
            Id = vesselStoppage.Id,
            VoyageId = vesselStoppage.VoyageId,
            ATA = vesselStoppage.ATA,
            ATD = vesselStoppage.ATD,
            ETA = vesselStoppage.ETA,
            ETD = vesselStoppage.ETD,
            OriginPortId = vesselStoppage.OriginPortId,
            PreviousPortId = vesselStoppage.PreviousPortId,
            NextPortId = vesselStoppage.NextPortId,
            VoyageNoIn = vesselStoppage.VoyageNoIn
        };
        v.ETA = v.ETA?.ShamsiToMiladi();
        v.ATA = v.ATA?.ShamsiToMiladi();
        v.ETD = v.ETD?.ShamsiToMiladi();
        v.ATD = v.ATD?.ShamsiToMiladi();
        return v;
    }

    private async Task<BLMessage> ValidateVesselStoppageAsync(VesselStoppage v)
    {
        BLMessage result = new BLMessage() { State = true };
        v = DatesShamsiToMiladi(v);
        if (v.ETA == null && v.ATA == null && v.ETD == null && v.ATD == null && v.OriginPortId == null && v.PreviousPortId == null && v.NextPortId == null)
        {
            result.State = false;
            result.Message = "All items cann`t null";
        }
        else if (v.ATA == null && v.ATD != null)
        {
            result.Message = "ATA must be filled at first!";
            result.State = false;
        }

        #region Update
        if (v.Id != 0)
        {
            if (v.ATA != null)
            {
                if (await _neginDbContext.VesselStoppages.Where(c => c.VoyageId == v.VoyageId && c.Id != v.Id).AnyAsync(c => c.ATA > v.ATA))
                {
                    result.Message = "There is at least a ATA after this ATA!";
                    result.State = false;
                    return result;
                }
                else if (await _neginDbContext.VesselStoppages.Where(c => c.VoyageId == v.VoyageId && c.Id != v.Id).AnyAsync(c => c.ATA <= v.ATA && c.ATD == null))
                {
                    result.Message = "There is a InProcess stoppage before this ATA!";
                    result.State = false;
                    return result;
                }
                else if (v.ETA > v.ETD || v.ATA >= v.ATD || await _neginDbContext.VesselStoppages.Where(c => c.VoyageId == v.VoyageId && c.Id != v.Id).AnyAsync(c => c.ATD >= v.ATD))
                {
                    result.Message = "Your Dates is not valid!";
                    result.State = false;
                    return result;
                }
                else if (v.ATA > DateTime.Now)
                {
                    result.Message = "ATA must be past!";
                    result.State = false;
                    return result;
                }
            }
            if (v.ATD != null)
            {
                if (await _neginDbContext.VesselStoppages.Where(c => c.VoyageId == v.VoyageId && c.Id != v.Id).AnyAsync(c => c.ATD > v.ATD))
                {
                    result.Message = "There is at least a ATD after this date!";
                    result.State = false;
                    return result;
                }
                else if (await _neginDbContext.VesselStoppages.Where(c => c.VoyageId == v.VoyageId && c.Id != v.Id).AnyAsync(c => c.ATA >= v.ATD))
                {
                    result.Message = "Your Dates is not valid!";
                    result.State = false;
                    return result;
                }
                else if (v.ATD > DateTime.Now)
                {
                    result.Message = "ATD must be past!";
                    result.State = false;
                    return result;
                }
            }
        }
        #endregion

        #region Add
        if (v.Id == 0)
        {
            if (await _neginDbContext.VesselStoppages.Where(c => c.VoyageId == v.VoyageId).AnyAsync(c => c.ETA >= v.ETA || c.ATA >= v.ATA || c.ETD >= v.ETD || c.ATD >= v.ATD))
            {
                result.Message = "There is at least a vessel stoppage before this!";
                result.State = false;
            }
            else if (v.ETA >= v.ETD || v.ATA >= v.ATD)
            {
                result.Message = "Your Dates is not valid!";
                result.State = false;
            }
            else if (await _neginDbContext.VesselStoppages.Where(c => c.VoyageId == v.VoyageId).AnyAsync(c => c.ATA <= v.ATA && c.ATD == null))
            {
                result.Message = "There is an InProcess stoppage before!";
                result.State = false;
            }
            else if (v.ATA > DateTime.Now || v.ATD > DateTime.Now)
            {
                result.Message = "Your Actual Dates must be past!";
                result.State = false;
            }
        }

        #endregion  

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
        return newVesselStoppageDate + TimeSpan.Parse(item.Value);
    }

}
