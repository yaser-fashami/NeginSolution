
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Aggregates.Billing;
using Negin.Core.Domain.Interfaces;
using Negin.Core.Domain.KeylessEntities;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;

namespace Negin.Infra.Data.Sql.EFRepositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly NeginDbContext _neginDBcontext;

    public InvoiceRepository(NeginDbContext neginDBcontext)
    {
        _neginDBcontext = neginDBcontext;
    }

    public async Task<BLMessage> CancelInvoice(ulong id)
    {
        var result = new BLMessage() { State = false };
        try
        {
            var invoice = await _neginDBcontext.Invoices.Include(x=>x.VesselStoppageInvoiceDetails).Include(x=>x.CleaningServiceInvoiceDetails).SingleAsync(x => x.Id == id);
            invoice.Status = Invoice.InvoiceStatus.IsCancel;
            foreach (var vesselStoppageInvoiceDetail in invoice.VesselStoppageInvoiceDetails)
            {
                _neginDBcontext.VesselStoppageInvoiceDetails.Remove(vesselStoppageInvoiceDetail);
            }
            foreach (var cleaningServiceInvoiceDetail in invoice.CleaningServiceInvoiceDetails)
            {
                _neginDBcontext.CleaningServiceInvoiceDetails.Remove(cleaningServiceInvoiceDetail);
            }
            _neginDBcontext.Update(invoice);
            await _neginDBcontext.SaveChangesAsync();
            result.State = true;
        }
        catch (Exception ex)
        {
            result.State = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<BLMessage> ConfirmInvoice(ulong id)
    {
        var result = new BLMessage() { State = false };
        try
        {
            var invoice = await _neginDBcontext.Invoices.SingleAsync(x => x.Id == id);
            invoice.Status = Invoice.InvoiceStatus.IsConfirm;
            _neginDBcontext.Update(invoice);
            await _neginDBcontext.SaveChangesAsync();
            result.State = true;
        }
        catch (Exception ex)
        {
            result.State = false;
            result.Message = ex.Message;
        }
        return result;
    }

    public async Task<IList<int>> GetExistYear()
    {
        IList<PersianDate> persianDates = new List<PersianDate>();
        var miladi = await _neginDBcontext.Invoices.Select(c => c.InvoiceDate).ToListAsync();
        foreach (var date in miladi)
        {
            persianDates.Add(date.MiladiToPersianDate());
        }
        return persianDates.Select(c => c.year).Distinct().ToList();
    }

    public async Task<Invoice> GetInvoiceDetailsById(ulong id)
    {
        return await _neginDBcontext.Invoices.Include(c => c.CreatedBy)
                            .Include(c => c.Voyage).ThenInclude(c => c.Vessel).ThenInclude(c => c.Flag)
                            .Include(c => c.Voyage).ThenInclude(c => c.Agent)
                            .Include(c => c.Voyage).ThenInclude(c => c.Owner)
                            .Include(c => c.VesselStoppageInvoiceDetails).ThenInclude(c => c.VesselStoppage)
                            .Include(c => c.CleaningServiceInvoiceDetails).ThenInclude(c => c.VesselStoppage).AsNoTracking()
                                .SingleAsync(c => c.Id == id);
    }

    public async Task<PagedData<Invoice>> GetPaginationInvoiceAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);

        var invoices = _neginDBcontext.Invoices.Include(c => c.Voyage).ThenInclude(c=>c.Vessel)
                            .Include(c=>c.Voyage).ThenInclude(c=>c.Agent)
                            .Include(c => c.Voyage).ThenInclude(c => c.Owner).AsNoTracking()
                                        .Where(c => noFilter
                                                || c.InvoiceNo.Contains(filter)
                                                || c.Voyage.Vessel.Name.Contains(filter)
                                                || c.Voyage.Agent.ShippingLineName.Contains(filter)
                                                || c.Voyage.Owner.ShippingLineName.Contains(filter));


        PagedData<Invoice> result = new PagedData<Invoice>()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await invoices.CountAsync()
            },
            Data = await invoices.OrderByDescending(c => c.Id).ToPagination(pageNumber, pageSize).ToListAsync()
        };

        return result;
    }

    public DataForDashboardChart1_Proc[] GetSumPriceProc(string year)
    {
        return _neginDBcontext.Set<DataForDashboardChart1_Proc>()
            .FromSqlInterpolated($"EXEC GetDataForDashboardChart1 {year}")
            .ToArray();
    }

    public async Task<int> InvoiceConfirmedCount()
    {
        return await _neginDBcontext.Invoices.Where(c=>c.Status == Invoice.InvoiceStatus.IsConfirm).CountAsync();
    }
}
