using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Negin.Core.Domain.Entities.Operation;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;

namespace Negin.Services.Operation;

public class VesselStoppageService : IVesselStoppageService
{
    private readonly NeginDbContext _neginDbContext;
    private readonly IOperationRepository _operationRepository;

    public VesselStoppageService(NeginDbContext neginDbContext, IOperationRepository operationRepository)
    {
        _neginDbContext = neginDbContext;
        _operationRepository = operationRepository;
    }

    public BLMessage AddVesselStoppage(VesselStoppage v, IFormCollection formCollection)
    {
        BLMessage result = new();
        if (SetTimes(ref v, formCollection))
        {
            result = ValidateVesselStoppageAsync(v).Result;
            ValidateStormDateTime(v, result);

            if (result.State)
            {
                v = DatesShamsiToMiladi(v);
                result = _operationRepository.CreateVesselStoppageAsync(v).Result;
            }
        }
        else
        {
            result.State = false;
            result.Message = "your time is invalid!";
        }

        return result;
    }

    public BLMessage UpdateVesselStoppage(VesselStoppage v, IFormCollection formCollection)
    {
        BLMessage result = new();
        if(SetTimes(ref v, formCollection))
        {
            result = ValidateVesselStoppageAsync(v).Result;
            ValidateStormDateTime(v, result);

            if (result.State)
            {
                v = DatesShamsiToMiladi(v);
                result = _operationRepository.UpdateVesselStoppageAsync(v).Result;
            }
        }
        else
        {
            result.State = false;
            result.Message = "your time is invalid";
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
            VoyageNoIn = vesselStoppage.VoyageNoIn,
            StartStorm = vesselStoppage.StartStorm,
            EndStorm = vesselStoppage.EndStorm,
        };
        v.ETA = v.ETA?.ShamsiToMiladi();
        v.ATA = v.ATA?.ShamsiToMiladi();
        v.ETD = v.ETD?.ShamsiToMiladi();
        v.ATD = v.ATD?.ShamsiToMiladi();
        v.StartStorm = v.StartStorm?.ShamsiToMiladi();
        v.EndStorm = v.EndStorm?.ShamsiToMiladi();
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

    private static void ValidateStormDateTime(VesselStoppage v, in BLMessage result)
    {
        if (v.StartStorm != null && v.EndStorm == null)
        {
            result.State = false;
            result.Message = "EndStorm must be filled!";
        }
        if (v.StartStorm == null && v.EndStorm != null)
        {
            result.State = false;
            result.Message = "StartStorm must be filled!";
        }
        if (v.StartStorm != null && v.EndStorm != null)
        {
            if (v.StartStorm < v.ATA)
            {
                result.State = false;
                result.Message = "StartStorm cann`t be before ATA!";
            }
            if (v.StartStorm > v.ATD)
            {
                result.State = false;
                result.Message = "StartStorm cann`t be after ATD!";
            }
            if (v.EndStorm > v.ATD)
            {
                result.State = false;
                result.Message = "EndtStorm cann`t be after ATD!";
            }
            if (v.StartStorm > v.EndStorm)
            {
                result.State = false;
                result.Message = "StartStorm cann`t be after EndStorm!";
            }
        }
    }

    private bool SetTimes(ref VesselStoppage newVesselStoppage, IEnumerable<KeyValuePair<string, StringValues>> formCollection)
    {
        foreach (var item in formCollection)
        {
            if (item.Key.Contains("ETATime") && newVesselStoppage.ETA != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                if (TimeSpan.TryParse(item.Value, out TimeSpan validate))
                {
                    newVesselStoppage.ETA = AddTimeToDate(newVesselStoppage.ETA.Value, item);
                }
                else
                {
                    return false;
                }
            }
            if (item.Key.Contains("ATATime") && newVesselStoppage.ATA != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                if (TimeSpan.TryParse(item.Value, out TimeSpan validate))
                {
                    newVesselStoppage.ATA = AddTimeToDate(newVesselStoppage.ATA.Value, item); 
                }
                else
                {
                    return false;
                }

            }
            if (item.Key.Contains("ETDTime") && newVesselStoppage.ETD != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                if (TimeSpan.TryParse(item.Value, out TimeSpan validate))
                {
                    newVesselStoppage.ETD = AddTimeToDate(newVesselStoppage.ETD.Value, item); 
                }
                else
                {
                    return false;
                }

            }
            if (item.Key.Contains("ATDTime") && newVesselStoppage.ATD != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                if (TimeSpan.TryParse(item.Value, out TimeSpan validate))
                {
                    newVesselStoppage.ATD = AddTimeToDate(newVesselStoppage.ATD.Value, item); 
                }
                else
                {
                    return false;
                }
            }
            if (item.Key.Contains("StartStormTime") && newVesselStoppage.StartStorm != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                if (TimeSpan.TryParse(item.Value, out TimeSpan validate))
                {
                    newVesselStoppage.StartStorm = AddTimeToDate(newVesselStoppage.StartStorm.Value, item); 
                }
                else
                {
                    return false;
                }
            }
            if (item.Key.Contains("EndStormTime") && newVesselStoppage.EndStorm != null && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                if (TimeSpan.TryParse(item.Value, out TimeSpan validate))
                {
                    newVesselStoppage.EndStorm = AddTimeToDate(newVesselStoppage.EndStorm.Value, item); 
                }
                else
                {
                    return false;
                }
            }
        }
        return true;
    }

    private DateTime? AddTimeToDate(DateTime newVesselStoppageDate, KeyValuePair<string, StringValues> item)
    {
        return newVesselStoppageDate + TimeSpan.Parse(item.Value);
    }

}
