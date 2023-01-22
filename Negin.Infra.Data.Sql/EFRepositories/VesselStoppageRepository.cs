
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Billing;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;
using static Negin.Framework.Exceptions.SqlException;

namespace Negin.Infra.Data.Sql.EFRepositories;

public class VesselStoppageRepository : IVesselStoppageRepository
{
    private readonly NeginDbContext _neginDbContext;

    public VesselStoppageRepository(NeginDbContext neginDbContext)
    {
        _neginDbContext = neginDbContext;
    }

    public Task<SqlException> CreateVesselStoppageAsync(VesselStoppage newVesselStoppage, IEnumerable<KeyValuePair<string, StringValues>> formCollection)
    {
        throw new NotImplementedException();
    }

    public void DeleteVesselStoppageById(ulong id)
    {
        throw new NotImplementedException();
    }

    public async Task<IList<Port>> GetAllPorts()
    {
        return await _neginDbContext.Ports.Include(c => c.Country).AsNoTracking().ToListAsync();
    }

    public async Task<PagedData<VesselStoppage>> GetPaginationVesselStoppageAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var vesselStoppages = _neginDbContext.VesselStoppages.Include(c => c.Voyage).AsNoTracking()
                                                                        .Where(c => c.IsDelete == false)
                                                                        .Where(c => noFilter || c.Voyage.VoyageNoIn.Contains(filter) || c.Voyage.Vessel.Name.Contains(filter));

        PagedData<VesselStoppage> result = new()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await vesselStoppages.CountAsync()
            },
            Data = await vesselStoppages.OrderBy(x => x.Voyage.Vessel.Name)
                                .ToPagination(pageNumber, pageSize)
                                .ToListAsync()
        };
        result.Data.ForEach(x => { x.ETA = x.ETA?.MiladiToShamsi(); x.ATA = x.ATA?.MiladiToShamsi(); x.ETD = x.ETD?.MiladiToShamsi(); x.ATD = x.ATD?.MiladiToShamsi(); });
        return result;

    }

    public Task<SqlException> UpdateVesselStoppageAsync(VesselStoppage v, IEnumerable<KeyValuePair<string, StringValues>> formCollection)
    {
        throw new NotImplementedException();
    }

    private void SetTimes(ref VesselStoppage newVesselStoppage, IEnumerable<KeyValuePair<string, StringValues>> formCollection)
    {
        foreach (var item in formCollection)
        {
            if (item.Key.Contains("ETA") && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                newVesselStoppage.ETA = AddTimeToDate(newVesselStoppage.ETA.Value, item);
            }
            if (item.Key.Contains("ATA") && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                newVesselStoppage.ATA = AddTimeToDate(newVesselStoppage.ATA.Value, item);
            }
            if (item.Key.Contains("ETD") && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                newVesselStoppage.ETD = AddTimeToDate(newVesselStoppage.ETD.Value, item);
            }
            if (item.Key.Contains("ATD") && string.IsNullOrEmpty(item.Value.First()) == false)
            {
                newVesselStoppage.ATD = AddTimeToDate(newVesselStoppage.ATD.Value, item);
            }
        }
    }

    private DateTime AddTimeToDate(DateTime newVesselStoppageDate, KeyValuePair<string, StringValues> item)
    {
        TimeSpan time = new TimeSpan();
        foreach (var eta in item.Value)
        {
            if (TimeSpan.TryParse(eta.Substring(0, eta.Length > 2 ? eta.Length - 2 : eta.Length), out time) && eta.Contains("AM"))
            {
                time = time.Add(new TimeSpan(-12, 0, 0));
            }
            else if (eta.Contains("PM"))
            {
                time = time.Add(new TimeSpan(12, 0, 0));
            }
        }

        return newVesselStoppageDate.AddTicks(time.Ticks);
    }
}
