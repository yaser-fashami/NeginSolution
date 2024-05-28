using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Entities.Billing;
using Negin.Core.Domain.Entities.Operation;
using Negin.Core.Domain.Interfaces;
using Negin.Core.Domain.KeylessEntities;
using Negin.Framework.Exceptions;
using Negin.Framework.Pagination;
using Negin.Framework.Utilities;
using Negin.Infrastructure;

namespace Negin.Infra.Data.Sql.EFRepositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly NeginDbContext _neginDbContext;

    public InvoiceRepository(NeginDbContext neginDBcontext)
    {
        _neginDbContext = neginDBcontext;
    }

    public async Task<BLMessage> CancelInvoice(ulong id)
    {
        var result = new BLMessage() { State = false };
        try
        {
            var invoice = await _neginDbContext.Invoices.Include(x=>x.VesselStoppageInvoiceDetails).Include(x=>x.CleaningServiceInvoiceDetails).SingleAsync(x => x.Id == id);
            invoice.Status = Invoice.InvoiceStatus.IsCancel;
            foreach (var vesselStoppageInvoiceDetail in invoice.VesselStoppageInvoiceDetails)
            {
                _neginDbContext.VesselStoppageInvoiceDetails.Remove(vesselStoppageInvoiceDetail);
            }
            foreach (var cleaningServiceInvoiceDetail in invoice.CleaningServiceInvoiceDetails)
            {
                _neginDbContext.CleaningServiceInvoiceDetails.Remove(cleaningServiceInvoiceDetail);
            }
            _neginDbContext.Update(invoice);
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

    public async Task<BLMessage> ConfirmInvoice(ulong id)
    {
        var result = new BLMessage() { State = false };
        try
        {
            var invoice = await _neginDbContext.Invoices.SingleAsync(x => x.Id == id);
            invoice.Status = Invoice.InvoiceStatus.IsConfirm;
            _neginDbContext.Update(invoice);
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

    public async Task<IList<int>> GetExistYear()
    {
        IList<PersianDate> persianDates = new List<PersianDate>();
        var miladi = await _neginDbContext.Invoices.Select(c => c.InvoiceDate).ToListAsync();
        foreach (var date in miladi)
        {
            persianDates.Add(date.MiladiToPersianDate());
        }
        return persianDates.Select(c => c.year).Distinct().ToList();
    }

    public async Task<Invoice> GetInvoiceDetailsById(ulong id)
    {
        return await _neginDbContext.Invoices.Include(c => c.CreatedBy)
                            .Include(c => c.Voyage).ThenInclude(c => c.Vessel).ThenInclude(c => c.Flag)
                            .Include(c => c.Voyage).ThenInclude(c => c.Agent)
                            .Include(c => c.Voyage).ThenInclude(c => c.Owner)
                            .Include(c => c.VesselStoppageInvoiceDetails).ThenInclude(c => c.VesselStoppage)
                            .Include(c => c.CleaningServiceInvoiceDetails).ThenInclude(c => c.VesselStoppage).AsNoTracking()
                                .SingleAsync(c => c.Id == id);
    }

    public async Task<PagedData<VesselStoppage>> GetPaginationDataForBillingAsync(int pageNumber = 1, int pageSize = 10, string filter = "", bool? invoiced = null)
    {
        bool noFilter = string.IsNullOrWhiteSpace(filter);
        IQueryable<VesselStoppage> vesselStoppages = null;
        if (invoiced == null)
        {
            vesselStoppages = _neginDbContext.VesselStoppages
                .Include(c => c.Voyage).ThenInclude(c => c.Vessel)
                .Include(c => c.Voyage).ThenInclude(c => c.Owner)
                .Include(c => c.Voyage).ThenInclude(c => c.Agent)
                .Include(c => c.VesselStoppageInvoiceDetail).AsNoTracking()
                    .Where(c => noFilter || c.Voyage.Vessel.Name.Contains(filter)
                                            || c.VoyageNoIn.Contains(filter)
                                            || c.Voyage.Owner.ShippingLineName.Contains(filter)
                                            || c.Voyage.Agent.ShippingLineName.Contains(filter))
                    .OrderBy(c => c.VesselStoppageInvoiceDetail);
        }
        else if (invoiced == true)
        {
            vesselStoppages = _neginDbContext.VesselStoppages
                .Include(c => c.Voyage).ThenInclude(c => c.Vessel)
                .Include(c => c.Voyage).ThenInclude(c => c.Owner)
                .Include(c => c.Voyage).ThenInclude(c => c.Agent)
                .Include(c => c.VesselStoppageInvoiceDetail).AsNoTracking()
                    .Where(c => c.VesselStoppageInvoiceDetail != null)
                    .Where(c => noFilter || c.Voyage.Vessel.Name.Contains(filter)
                                            || c.VoyageNoIn.Contains(filter)
                                            || c.Voyage.Owner.ShippingLineName.Contains(filter)
                                            || c.Voyage.Agent.ShippingLineName.Contains(filter))
                    .OrderByDescending(c => c.VesselStoppageInvoiceDetail.CreateDate);
        }
        else if (invoiced == false)
        {
            vesselStoppages = _neginDbContext.VesselStoppages
                .Include(c => c.Voyage).ThenInclude(c => c.Vessel)
                .Include(c => c.Voyage).ThenInclude(c => c.Owner)
                .Include(c => c.Voyage).ThenInclude(c => c.Agent)
                .Include(c => c.VesselStoppageInvoiceDetail).AsNoTracking()
                    .Where(c => c.VesselStoppageInvoiceDetail == null)
                    .Where(c => noFilter || c.Voyage.Vessel.Name.Contains(filter)
                                            || c.VoyageNoIn.Contains(filter)
                                            || c.Voyage.Owner.ShippingLineName.Contains(filter)
                                            || c.Voyage.Agent.ShippingLineName.Contains(filter))
                    .OrderByDescending(c => c.ATA);

        }


        PagedData<VesselStoppage> result = new()
        {
            PageInfo = new()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = await vesselStoppages.CountAsync()
            },
            Data = await vesselStoppages.ToPagination(pageNumber, pageSize).ToListAsync()
        };

        result.Data.ForEach(c => c.SetStatus());
        for (int i = 0; i < result.Data.Count; i++)
        {
            if (result.Data[i].Status != VesselStoppage.VesselStoppageStatus.Invoiced && (result.Data[i].Voyage.IsDelete == true || result.Data[i].Voyage.IsActive == false))
            {
                result.Data.RemoveAt(i);
                result.PageInfo.TotalCount--;
            }
        }

        return result;
    }


    //public async Task<PagedData<Invoice>> GetPaginationInvoiceAsync(int pageNumber = 1, int pageSize = 10, string filter = "")
    //{
    //    bool noFilter = string.IsNullOrWhiteSpace(filter);

    //    var invoices = _neginDbContext.Invoices.Include(c => c.Voyage).ThenInclude(c=>c.Vessel)
    //                        .Include(c=>c.Voyage).ThenInclude(c=>c.Agent)
    //                        .Include(c => c.Voyage).ThenInclude(c => c.Owner).AsNoTracking()
    //                                    .Where(c => noFilter
    //                                            || c.InvoiceNo.Contains(filter)
    //                                            || c.Voyage.Vessel.Name.Contains(filter)
    //                                            || c.Voyage.Agent.ShippingLineName.Contains(filter)
    //                                            || c.Voyage.Owner.ShippingLineName.Contains(filter));


    //    PagedData<Invoice> result = new PagedData<Invoice>()
    //    {
    //        PageInfo = new()
    //        {
    //            PageNumber = pageNumber,
    //            PageSize = pageSize,
    //            TotalCount = await invoices.CountAsync()
    //        },
    //        Data = await invoices.OrderByDescending(c => c.Id).ToPagination(pageNumber, pageSize).ToListAsync()
    //    };

    //    return result;
    //}

    public DataForDashboardChart1_Proc[] GetSumPriceProc(string year)
    {
        return _neginDbContext.Set<DataForDashboardChart1_Proc>()
            .FromSqlInterpolated($"EXEC GetDataForDashboardChart1 {year}")
            .ToArray();
    }

    public async Task<int> InvoiceConfirmedCount()
    {
        return await _neginDbContext.Invoices.Where(c=>c.Status == Invoice.InvoiceStatus.IsConfirm).CountAsync();
    }

    public async Task<LoadingDischargeInvoice> GetLoadingDischargeInvoice(ulong id)
    {
        var result = await _neginDbContext.LoadingDischargeInvoices
                            .Include(c=>c.ShippingLineCompany)
                            .Include(c=>c.LoadingDischarge)
                            .ThenInclude(c=>c.VesselStoppage)
                            .ThenInclude(c=>c.Voyage)
                            .ThenInclude(c=>c.Vessel)
                            .ThenInclude(c=>c.Flag)
                            .AsNoTracking()
                            .SingleAsync(c => c.LoadingDischargeId == id);

        result.CreatedBy = await _neginDbContext.Users.AsNoTracking().SingleOrDefaultAsync(c => c.Id == result.CreatedById);
        return result;
    }

    public async Task<BLMessage> CancelLoadinDischargeInvoice(ulong id)
    {
        var result = new BLMessage() { State = false };
        try
        {
            var invoice = await _neginDbContext.LoadingDischargeInvoices.SingleAsync(x => x.Id == id);
            invoice.Status = Invoice.InvoiceStatus.IsCancel;
            _neginDbContext.Update(invoice);
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

    public async Task<BLMessage> ConfirmLoadingDischargeInvoice(ulong id)
    {
        var result = new BLMessage() { State = false };
        try
        {
            var invoice = await _neginDbContext.LoadingDischargeInvoices.SingleAsync(x => x.Id == id);
            invoice.Status = Invoice.InvoiceStatus.IsConfirm;
            _neginDbContext.Update(invoice);
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
}
