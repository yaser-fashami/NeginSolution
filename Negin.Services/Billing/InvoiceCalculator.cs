using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Aggregates.Billing;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Dtos;
using Negin.Framework.Exceptions;
using Negin.Framework.Utilities;
using Negin.Infrastructure;
using System.Text;
using static Negin.Core.Domain.Aggregates.Billing.Invoice;

namespace Negin.Services.Billing;

public class InvoiceCalculator : IInvoiceCalculator
{
    private readonly NeginDbContext _neginDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InvoiceCalculator(NeginDbContext neginDbContext, IHttpContextAccessor httpContextAccessor)
    {
        _neginDbContext = neginDbContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PreInvoiceDto> CalculateAsync(ulong voyageId, IEnumerable<string> vesselStoppagesIdStr)
    {
        var date = DateTime.Now;
        var shamsiDate = date.MiladiToShamsi();
        var invoiceNo = new StringBuilder();
        invoiceNo.Append(shamsiDate?.Year);
        invoiceNo.Append(shamsiDate?.Month.ToString().Length == 1 ? "0" + shamsiDate?.Month : shamsiDate?.Month);
        invoiceNo.Append('-');
        var lastInvoice = await _neginDbContext.Invoices.Where(c=>c.InvoiceNo.StartsWith(shamsiDate.Value.Year.ToString())).OrderBy(c => c.Id).LastOrDefaultAsync();
        var lastinvoiceNo = lastInvoice != null ? ((int.Parse(lastInvoice.InvoiceNo.Substring(7, lastInvoice.InvoiceNo.Length - 7))) + 1).ToString() : "1";
        invoiceNo.Append(lastinvoiceNo);

        List<ulong> vesselStoppagesId = vesselStoppagesIdStr.Select(item => Convert.ToUInt64(item)).ToList();

        IList<VesselStoppage> vesselStoppages = await _neginDbContext.VesselStoppages
            .Include(c=>c.OriginPort)
            .Include(c=>c.PreviousPort)
            .Include(c=>c.NextPort)
            .Include(c=>c.CreatedBy).AsNoTracking()
            .Where(c => vesselStoppagesId.Contains(c.Id)).ToListAsync();

        var voyage = await _neginDbContext.Voyages.Include(c => c.Vessel).ThenInclude(c=>c.Type)
            .Include(c=>c.Vessel).ThenInclude(c=>c.Flag)
            .Include(c => c.Vessel).ThenInclude(c => c.Nationality)
            .Include(c => c.Owner).Include(c => c.Agent)
            .AsNoTracking().SingleAsync(c => c.Id == voyageId);

        int totalDwellingHour = 0, totalDwellingDay = 0; 
        ulong sumPriceR = 0, sumPriceRVat = 0;
        decimal sumPriceD = 0;
        List<PreInvoiceDetailDto> preInvoiceDetails = new();

        foreach (var vesselStoppage in vesselStoppages)
        {
            vesselStoppage.ATADayOfTheWeek = vesselStoppage.ATA?.DayOfWeek;
            vesselStoppage.ATDDayOfTheWeek = vesselStoppage.ATD?.DayOfWeek;
            var diffHour = vesselStoppage.ATD?.Hour - vesselStoppage.ATA?.Hour;
            var diffDays = (vesselStoppage.ATD - vesselStoppage.ATA)?.Days;
            var dwellingHour = (diffDays * 24) + diffHour ?? 0;
            var dwellingDay = dwellingHour % 24 == 0 ? dwellingHour / 24 : (dwellingHour / 24) + 1;
            totalDwellingHour += dwellingHour;
            var vesselStoppageTariff = await _neginDbContext.VesselStoppageTariffs.Include(c => c.VesselStoppageTariffDetails).AsNoTracking().Where(c => c.EffectiveDate <= vesselStoppage.ATA).OrderBy(c => c.EffectiveDate).LastOrDefaultAsync();
            var cleaningServiceTariff = await _neginDbContext.CleaningServiceTariffs.Include(c => c.CleaningServiceTariffDetails).AsNoTracking().Where(c => c.EffectiveDate <= vesselStoppage.ATA).OrderBy(c => c.EffectiveDate).LastOrDefaultAsync();
            var vesselStoppageTariffDetail = vesselStoppageTariff?.VesselStoppageTariffDetails.Where(c => c.VesselTypeId == voyage.Vessel?.VesselTypeId).Single();
            var cleaningServiceTariffDetail = cleaningServiceTariff?.CleaningServiceTariffDetails.Where(c => c.GrossWeight >= voyage.Vessel?.GrossTonage).OrderBy(c=>c.GrossWeight).First();
            var currency = await _neginDbContext.Currencies.AsNoTracking().Where(c => c.Date <= vesselStoppage.ATA).OrderBy(c => c.Date).LastOrDefaultAsync();

            decimal priceDVS = 0, priceDCS = 0;
            ulong priceRVS = 0, priceRVSVat = 0, priceRCS = 0, priceRCSVat = 0;
            byte discounrRate = 0;
            if (dwellingHour <= vesselStoppageTariffDetail?.NormalHour)
            {
                priceDVS = Math.Round(Convert.ToDecimal(dwellingHour * vesselStoppageTariffDetail.NormalPrice), 2);
            }
            else
            {
                var extraH = dwellingHour - vesselStoppageTariffDetail?.NormalHour;
                var normalH = dwellingHour - extraH;
                priceDVS = Math.Round((Convert.ToDecimal(normalH * vesselStoppageTariffDetail?.NormalPrice) + Convert.ToDecimal(extraH * vesselStoppageTariffDetail?.ExtraPrice)), 2);
            }
            priceDCS = Math.Round(Convert.ToDecimal(dwellingDay * cleaningServiceTariffDetail?.Price), 2);
            var DiscountTariff = await _neginDbContext.DiscountTariffs.AsNoTracking().Where(c => c.EffectiveDate <= vesselStoppage.ATA).OrderBy(c => c.EffectiveDate).LastOrDefaultAsync();
            if (DiscountTariff != null)
            {
                if (voyage.Vessel?.FlagId == DiscountTariff.FlagId && voyage.Vessel?.GrossTonage <= DiscountTariff.ToGrossTonage)
                {
                    priceDVS = Math.Round(priceDVS - (priceDVS * Convert.ToDecimal(DiscountTariff.DiscountPercent)), 2);
                    priceDCS = Math.Round(priceDCS - (priceDCS * Convert.ToDecimal(DiscountTariff.DiscountPercent)), 2);
                    discounrRate = Convert.ToByte(DiscountTariff.DiscountPercent * 100);
                }
            }

            sumPriceD += (priceDVS + priceDCS);
            if (voyage.Vessel?.FlagId == 207)
            {
                priceRVS = Convert.ToUInt64(priceDVS * currency?.PersianDollerRate);
                priceRCS = Convert.ToUInt64(priceDCS * currency?.PersianDollerRate);
            }
            else
            {
                priceRVS = Convert.ToUInt64(priceDVS * currency?.ForeignDollerRate);
                priceRCS = Convert.ToUInt64(priceDCS * currency?.ForeignDollerRate);
            }
            var vatTariff = await _neginDbContext.VatTariffs.AsNoTracking().Where(c => c.EffectiveDate <= vesselStoppage.ATA).OrderBy(c => c.EffectiveDate).LastOrDefaultAsync();
            if (vatTariff != null)
            {
                priceRVSVat = Convert.ToUInt64(priceRVS * vatTariff.Rate / 100);
                priceRCSVat = Convert.ToUInt64(priceRCS * vatTariff.Rate / 100);
                priceRVS += priceRVSVat;
                priceRCS += priceRCSVat;
                sumPriceRVat += (priceRVSVat + priceRCSVat);
            }
            sumPriceR += (priceRVS + priceRCS);
            totalDwellingDay += dwellingDay;

            PreInvoiceDetailDto preInvoiceDetail = new()
            {
                VesselStoppage = vesselStoppage,
                VesselStoppageInvoiceDetail = new VesselStoppageInvoiceDetail
                {
                    DwellingHour = (uint)dwellingHour,
                    VesselStoppage = vesselStoppage,
                    VesselStoppageId = vesselStoppage.Id,
                    VesselStoppageTariff = vesselStoppageTariff,
                    VesselStoppageTariffId = vesselStoppageTariff.Id,
                    PriceD = priceDVS,
                    PriceR = priceRVS,
                    PriceRVat = priceRVSVat,
                    ApplyNormalPrice = vesselStoppageTariffDetail.NormalPrice,
                    ApplyExtraPrice = vesselStoppageTariffDetail.ExtraPrice,
                    ApplyCurrencyRate = voyage.Vessel.FlagId == 207 ? currency.PersianDollerRate : currency.ForeignDollerRate
                },
                CleaningServiceInvoiceDetail = new CleaningServiceInvoiceDetail
                {
                    DwellingHour = (uint)dwellingHour,
                    VesselStoppage = vesselStoppage,
                    VesselStoppageId = vesselStoppage.Id,
                    CleaningServiceTariff = cleaningServiceTariff,
                    CleaningServiceTariffId = cleaningServiceTariff.Id,
                    PriceD = priceDCS,
                    PriceR = priceRCS,
                    PriceRVat = priceRCSVat,
                    ApplyPrice = cleaningServiceTariffDetail.Price,
                    ApplyCurrencyRate = voyage.Vessel.FlagId == 207 ? currency.PersianDollerRate : currency.ForeignDollerRate
                },
                VATPercent = Convert.ToByte(vatTariff?.Rate),
                StoppageIssuedBy = vesselStoppage.CreatedBy?.UserName,
                Currency = currency,
                DiscountRate = discounrRate
            };
            preInvoiceDetails.Add(preInvoiceDetail);
        }

        var result = new PreInvoiceDto()
        {
            InvoiceNo = invoiceNo.ToString(),
            InvoiceDate = date,
            VoyageId = voyageId,
            Voyage = voyage,
            CurrentUser = _httpContextAccessor.HttpContext.User.Identity?.Name,
            CurrentUserEmail = _httpContextAccessor.HttpContext.User.Identity.GetCurrentUserEmail(),
            PreInvoiceDetails = preInvoiceDetails,
            TotalDwellingHour = (uint)totalDwellingHour,
            TotalDwellingDays = (uint)totalDwellingDay,
            SumPriceD = sumPriceD,
            SumPriceR = sumPriceR,
            SumPriceRVat = sumPriceRVat,
        };

        return result;
    }

    public async Task<BLMessage> Invoicing(PreInvoiceDto preInvoice)
    {
        BLMessage result = new BLMessage() { State = false };

        using var transaction = _neginDbContext.Database.BeginTransaction();
        Invoice newInvoice = new()
        {
            InvoiceNo = preInvoice.InvoiceNo,
            InvoiceDate = preInvoice.InvoiceDate,
            VoyageId = preInvoice.VoyageId,
            Status = InvoiceStatus.IsIssued,
            IsPaied = false,
            SumPriceD = preInvoice.SumPriceD,
            SumPriceR = preInvoice.SumPriceR,
            SumPriceRVat = preInvoice.SumPriceRVat,
            TotalDwellingHour = preInvoice.TotalDwellingHour,
            CreatedById = _httpContextAccessor.HttpContext.User?.Identity?.GetCurrentUserId(),
            CreateDate = DateTime.Now,
        };
        try
        {
            await _neginDbContext.Invoices.AddAsync(newInvoice);
            await _neginDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            return result;
        }

        foreach (var preInvoiceDetail in preInvoice.PreInvoiceDetails)
        {
            VesselStoppageInvoiceDetail vesselStoppageInvoice = new()
            {
                InvoiceId = newInvoice.Id,
                VesselStoppageId = preInvoiceDetail.VesselStoppageInvoiceDetail.VesselStoppageId,
                VesselStoppageTariffId = preInvoiceDetail.VesselStoppageInvoiceDetail.VesselStoppageTariffId,
                CurrencyId = preInvoiceDetail.Currency.Id,
                DwellingHour = preInvoiceDetail.VesselStoppageInvoiceDetail.DwellingHour,
                PriceR = preInvoiceDetail.VesselStoppageInvoiceDetail.PriceR,
                PriceD = preInvoiceDetail.VesselStoppageInvoiceDetail.PriceD,
                PriceRVat = preInvoiceDetail.VesselStoppageInvoiceDetail.PriceRVat,
                ApplyCurrencyRate = preInvoiceDetail.VesselStoppageInvoiceDetail.ApplyCurrencyRate,
                ApplyNormalPrice = preInvoiceDetail.VesselStoppageInvoiceDetail.ApplyNormalPrice,
                ApplyExtraPrice = preInvoiceDetail.VesselStoppageInvoiceDetail.ApplyExtraPrice,
                CreatedById = _httpContextAccessor.HttpContext.User?.Identity?.GetCurrentUserId(),
                CreateDate = DateTime.Now,
            };
            CleaningServiceInvoiceDetail cleaningServiceInvoiceDetail = new()
            {
                InvoiceId = newInvoice.Id,
                VesselStoppageId = preInvoiceDetail.CleaningServiceInvoiceDetail.VesselStoppageId,
                CleaningServiceTariffId = preInvoiceDetail.CleaningServiceInvoiceDetail.CleaningServiceTariffId,
                CurrencyId = preInvoiceDetail.Currency.Id,
                DwellingHour = preInvoiceDetail.CleaningServiceInvoiceDetail.DwellingHour,
                PriceR = preInvoiceDetail.CleaningServiceInvoiceDetail.PriceR,
                PriceD = preInvoiceDetail.CleaningServiceInvoiceDetail.PriceD,
                PriceRVat = preInvoiceDetail.CleaningServiceInvoiceDetail.PriceRVat,
                ApplyCurrencyRate = preInvoiceDetail.CleaningServiceInvoiceDetail.ApplyCurrencyRate,
                ApplyPrice = preInvoiceDetail.CleaningServiceInvoiceDetail.ApplyPrice,
                CreatedById = _httpContextAccessor.HttpContext.User?.Identity?.GetCurrentUserId(),
                CreateDate = DateTime.Now,
            };

            try
            {
                await _neginDbContext.VesselStoppageInvoiceDetails.AddAsync(vesselStoppageInvoice);
                await _neginDbContext.CleaningServiceInvoiceDetails.AddAsync(cleaningServiceInvoiceDetail);
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                return result;
            }
        }
        
        try
        {
            await _neginDbContext.SaveChangesAsync();
            transaction.Commit();
        }
        catch(UniqueConstraintException) 
        {
            result.Message = "This preInvoice was issued before!";
            return result;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
            return result;
        }

        result.State = true;
        result.Message = preInvoice?.InvoiceNo;


        return result;
    }
}
