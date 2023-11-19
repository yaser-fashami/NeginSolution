using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;
using System.ComponentModel.DataAnnotations;
using static Negin.Framework.Exceptions.SqlException;

namespace Negin.Infra.Data.Sql.EFRepositories;

public class VesselRepository : IVesselRepository
{
    private readonly NeginDbContext _neginDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public VesselRepository(NeginDbContext neginDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _neginDbContext = neginDbContext;
        _httpContextAccessor = httpContextAccessor;
    }


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
        return await _neginDbContext.Countries.AsNoTracking().OrderBy(c=>c.Name).ToListAsync();
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
            .Where(x=>x.IsDelete == false)
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

	public async Task<IList<Vessel>> GetVesselsAssignedVoyage()
	{
        return await _neginDbContext.Voyages.Include(v => v.Vessel).AsNoTracking().Select(v => v.Vessel).Distinct().ToListAsync();
	}

    public async Task<IList<Vessel>> GetVesselsNotAssignedVoyage()
    {
		var assignedVesselsIds = await _neginDbContext.Voyages.Include(v => v.Vessel).AsNoTracking().Select(v => v.Vessel.Id).Distinct().ToListAsync();

        var result = await _neginDbContext.Vessels.AsNoTracking().ToListAsync();
        result.RemoveAll(i => assignedVesselsIds.Any(c => c == i.Id));

        return result;
    }
}
