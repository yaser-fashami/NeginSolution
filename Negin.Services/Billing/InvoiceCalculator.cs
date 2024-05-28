using EntityFramework.Exceptions.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Negin.Core.Domain.Dtos;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Billing;
using Negin.Core.Domain.Entities.Operation;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Utilities;
using Negin.Infrastructure;
using System.Text;
using static Negin.Core.Domain.Entities.Billing.Invoice;

namespace Negin.Services.Billing;

public class InvoiceCalculator : IInvoiceCalculator
{
    private readonly NeginDbContext _neginDbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAppVersionService _appVersionService;

    public InvoiceCalculator(NeginDbContext neginDbContext, IHttpContextAccessor httpContextAccessor, IAppVersionService appVersionService)
    {
        _neginDbContext = neginDbContext;
        _httpContextAccessor = httpContextAccessor;
        _appVersionService = appVersionService;
    }

    public async Task<PreInvoiceDto> CalculateAsync(ulong voyageId, IEnumerable<ulong> vesselStoppagesId)
    {
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

        uint totalDwellingHour = 0, totalDwellingDay = 0; 
        ulong sumPriceR = 0, sumPriceRVat = 0;
        decimal sumPriceD = 0;
        double discountPercent = 0;
        List<PreInvoiceDetailDto> preInvoiceDetails = new();
        Currency currency = null;

        foreach (var vesselStoppage in vesselStoppages)
        {
            var dwellingHour = CalculateTotalDwellingHour(vesselStoppage, out uint dwellingDay, out uint stormHour);
            totalDwellingHour += dwellingHour;
            totalDwellingDay += dwellingDay;

            var vesselStoppageTariff = PrepareVesselStoppageTariff(vesselStoppage.ATA.Value).Result;
            var cleaningServiceTariff = PrepareCleaningServiceTariff(vesselStoppage.ATA.Value).Result;
            var vesselStoppageTariffDetail = PrepareVesselStoppageTariffDetails(voyage, vesselStoppageTariff);
            var cleaningServiceTariffDetail = PrepareCleaningServiceTariffDetails(voyage, cleaningServiceTariff);

            if (currency == null)
            {
                currency = await PrepareCurrentCurrencyRateAsync(vesselStoppage);
            }
            decimal priceDVS = CalculateVesselStoppagePriceDollar(vesselStoppageTariffDetail, voyage, dwellingHour);
            decimal priceDCS = CalculateCleaningServicePriceDollar(cleaningServiceTariffDetail, dwellingDay);
            discountPercent = CalculateDiscount(voyage, vesselStoppage.ATA.Value, ref priceDVS, ref priceDCS);

            sumPriceD += priceDVS + priceDCS;

            ulong priceRVS = CalculatePriceRial(voyage, currency, priceDVS);
            ulong priceRCS = CalculatePriceRial(voyage, currency, priceDCS);
            ulong priceRVSVat = 0, priceRCSVat = 0;

            VatTariff vatTariff = await _neginDbContext.VatTariffs.AsNoTracking().OrderBy(c => c.EffectiveDate).LastAsync();
            if (vatTariff != null)
            {
                priceRVSVat = Convert.ToUInt64(priceRVS * vatTariff.Rate / 100);
                priceRCSVat = Convert.ToUInt64(priceRCS * vatTariff.Rate / 100);
                sumPriceRVat += (priceRVSVat + priceRCSVat);
            }
            sumPriceR += priceRVS + priceRCS + sumPriceRVat;

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
                StoppageIssuedBy = vesselStoppage?.CreatedBy?.UserName,
                Currency = currency,
                DiscountPercent = (byte)discountPercent,
                StormHour = stormHour
            };
            preInvoiceDetails.Add(preInvoiceDetail);
        }

        var invoiceNo = PrepareInvoiceNumber(out DateTime date);

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
            SumPriceD = sumPriceD + RialVatToDollarVat(currency, sumPriceRVat, voyage),
            SumPriceR = sumPriceR,
            SumPriceRVat = sumPriceRVat,
            DiscountPercent = (byte)discountPercent
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
            DiscountPercent = preInvoice.DiscountPercent,
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
            await transaction.CommitAsync();
        }
        catch(UniqueConstraintException) 
        {
            await transaction.RollbackAsync();
            result.Message = "This preInvoice was issued before!";
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            result.Message = ex.Message;
            return result;
        }

        result.State = true;
        result.Message = preInvoice?.InvoiceNo;


        return result;
    }

    public async Task<PreLoadingDischargeInvoiceDto> CalculateLoadingDischargeInvoiceAsync(LoadingDischargeInvoiceDto loadingDischargeDto)
	{
        LoadingDischarge loadingDischarge;
        LoadingDischargeTariff loadingDischargeTariff;
        uint perTonPrice = 0;
        ulong loadingDischargePriceR;

		try
        {
            loadingDischarge = await _neginDbContext.LoadingDischarges
                                                            .Include(c => c.VesselStoppage)
                                                            .ThenInclude(c => c.Voyage)
                                                            .ThenInclude(c => c.Vessel)
                                                            .ThenInclude(c => c.Flag)
                                                            .Include(c => c.VesselStoppage)
                                                            .ThenInclude(c => c.Voyage)
                                                            .ThenInclude(c => c.Vessel)
                                                            .ThenInclude(c => c.Nationality)
                                                            .Include(c => c.VesselStoppage)
                                                            .ThenInclude(c => c.CreatedBy)
                                                            .Include(c => c.VesselStoppage)
                                                            .ThenInclude(c => c.Voyage)
                                                            .ThenInclude(c => c.Vessel)
                                                            .ThenInclude(c => c.Type).AsNoTracking()
                                                            .Where(c => c.Id == loadingDischargeDto.loadingDischargeId)
                                                            .SingleAsync();


            loadingDischargeTariff = await _neginDbContext.LoadingDischargeTariffs
                                                        .Include(c => c.LoadingDischargeTariffDetails).AsNoTracking()
                                                        .OrderByDescending(c => c.EffectiveDate)
                                                        .Where(c => c.EffectiveDate <= loadingDischarge.VesselStoppage.ATA)
                                                        .FirstAsync();

        }
        catch (Exception)
        {

            throw;
        }

        var tariffPrice = loadingDischargeTariff.LoadingDischargeTariffDetails.Single(c => c.Id == loadingDischarge.LoadingDischargeTariffDetailId).Price;
        if (loadingDischarge.Method == LoadingDischarge.LoadingDischargeMethod.LOAD)
        {
            perTonPrice = tariffPrice * 75/100 * 36/100;
        }
        else if (loadingDischarge.Method == LoadingDischarge.LoadingDischargeMethod.DISC)
        {
			perTonPrice = tariffPrice * 36 / 100;
		}
        loadingDischargePriceR = UInt64.Parse((loadingDischarge.Tonage * (double)perTonPrice).ToString());

        ulong craneTariff = 0, inventoryTariff = 0, inventoryCostR = 0, discountAmount = 0;
        if (loadingDischarge.HasCrane)
        {
            craneTariff = loadingDischargeTariff.LoadingDischargeTariffDetails.Where(c => c.GroupName.Contains("بارگیری")).Select(c => c.Price).Single();
        };
        if (loadingDischarge.HasInventory)
        {
            inventoryTariff = loadingDischargeTariff.LoadingDischargeTariffDetails.Where(c => c.GroupName.Contains("انبارداری")).Select(c => c.Price).Single();
            inventoryCostR = (ulong)(inventoryTariff * loadingDischarge.Tonage);
        }
        ulong totalCostR = loadingDischargePriceR + craneTariff + inventoryCostR;
        if (loadingDischargeDto.DiscountPercent > 0)
        {
            discountAmount = totalCostR * loadingDischargeDto.DiscountPercent / 100;
        }
        byte vatPercent = (byte)(await _neginDbContext.VatTariffs.OrderBy(c => c.EffectiveDate).LastAsync()).Rate;
        ulong vatCostR = totalCostR * vatPercent / 100;

        PreLoadingDischargeInvoiceDto result = new PreLoadingDischargeInvoiceDto(
            InvoiceNo: PrepareLoadingDischargeInvoiceNumber(out DateTime date),
            InvoiceDate: date,
            LoadingDiascharge: loadingDischarge,
            LoadingDischargeTariffId: loadingDischargeTariff.Id,
            ShippingLineCompany: await _neginDbContext.ShippingLines.SingleAsync(c => c.Id == loadingDischargeDto.shippingLineCompanyId),
            DiscountPercent: loadingDischargeDto.DiscountPercent,
            DiscountAmount: discountAmount,
            PerTonPrice: perTonPrice,
            LDCostR: loadingDischargePriceR,
            Tonage: loadingDischarge.Tonage,
            CraneCostR: craneTariff,
            InventoryTariffPrice: inventoryTariff,
            InventoryCostR: inventoryCostR,
            TotalCostR: totalCostR,
            VesselStoppage: loadingDischarge.VesselStoppage,
            Vessel: loadingDischarge.VesselStoppage.Voyage.Vessel,
            VatPercent: vatPercent,
            VatCostR: vatCostR,
            CurrentUser: _httpContextAccessor.HttpContext.User.Identity?.Name,
            CurrentUserEmail: _httpContextAccessor.HttpContext.User.Identity.GetCurrentUserEmail()
        );

        return result;
    }

    public async Task<BLMessage> LoadingDischargeInvoicing(PreLoadingDischargeInvoiceDto preLoadingDischargeInvoiceDto)
    {
        BLMessage result = new BLMessage() { State = false };

        LoadingDischargeInvoice newLoadingDischargeInvoice = new()
        {
            InvoiceNo = preLoadingDischargeInvoiceDto.InvoiceNo,
            InvoiceDate = preLoadingDischargeInvoiceDto.InvoiceDate,
            LoadingDischargeId = preLoadingDischargeInvoiceDto.LoadingDiascharge.Id,
            LoadingDischargeTariffId = preLoadingDischargeInvoiceDto.LoadingDischargeTariffId,
            ShippingLineCompanyId = preLoadingDischargeInvoiceDto.ShippingLineCompany.Id,
            DiscountPercent = preLoadingDischargeInvoiceDto.DiscountPercent,
            Tonage = preLoadingDischargeInvoiceDto.Tonage,
            LDCostR = preLoadingDischargeInvoiceDto.LDCostR,
            CraneCostR = preLoadingDischargeInvoiceDto.CraneCostR,
            InventoryCostR = preLoadingDischargeInvoiceDto.InventoryCostR,
            VatPercent = preLoadingDischargeInvoiceDto.VatPercent,
            VatCostR = preLoadingDischargeInvoiceDto.VatCostR,
            TotalPriceR = preLoadingDischargeInvoiceDto.TotalCostR - preLoadingDischargeInvoiceDto.DiscountAmount + preLoadingDischargeInvoiceDto.VatCostR,
            CreatedById = _httpContextAccessor.HttpContext.User?.Identity?.GetCurrentUserId(),
            CreateDate = DateTime.Now
        };

        try
        {
            await _neginDbContext.LoadingDischargeInvoices.AddAsync(newLoadingDischargeInvoice);
            await _neginDbContext.SaveChangesAsync();
            result.State = true;
            result.Message = preLoadingDischargeInvoiceDto?.InvoiceNo;
        }
        catch (Exception ex)
        {
            result.Message = ex.Message;
        }

        return result;
    }


    #region Private Methods
    private string PrepareInvoiceNumber(out DateTime date)
    {
        date = DateTime.Now;
        var shamsiDate = date.MiladiToPersianDate();
        var invoiceNo = new StringBuilder();
        if (_appVersionService.Beneficiary == Beneficiary.Negin)
        {
            invoiceNo.Append('N');
        }
        else if (_appVersionService.Beneficiary == Beneficiary.SinaOil)
        {
            invoiceNo.Append('O');
        }
        invoiceNo.Append(shamsiDate.year);
        invoiceNo.Append(shamsiDate.month.ToString().Length == 1 ? "0" + shamsiDate.month : shamsiDate.month);
        invoiceNo.Append('-');
        var lastInvoice = _neginDbContext.Invoices.OrderBy(c => c.Id).LastOrDefaultAsync().Result;
        var lastinvoiceNo = lastInvoice != null ? (int.Parse(lastInvoice.InvoiceNo.Substring(8, lastInvoice.InvoiceNo.Length - 8)) + 1).ToString() : "65";
        if (shamsiDate.year > lastInvoice?.InvoiceDate.MiladiToPersianDate().year)
        {
            lastinvoiceNo = "1";
        }
        invoiceNo.Append(lastinvoiceNo);

        return invoiceNo.ToString();
    }

	private string PrepareLoadingDischargeInvoiceNumber(out DateTime date)
    {
        date = DateTime.Now;
        var shamsiDate = date.MiladiToPersianDate();
        var invoiceNo = new StringBuilder();
		invoiceNo.Append("NB");
        invoiceNo.Append(shamsiDate.year);
        invoiceNo.Append(shamsiDate.month.ToString().Length == 1 ? "0" + shamsiDate.month : shamsiDate.month);
        invoiceNo.Append('-');
        var lastInvoice = _neginDbContext.LoadingDischargeInvoices.OrderBy(c => c.Id).LastOrDefault();
        var lastinvoiceNo = lastInvoice != null ? (int.Parse(lastInvoice.InvoiceNo.Substring(9, lastInvoice.InvoiceNo.Length - 9)) + 1).ToString() : "1";
        if (shamsiDate.year > lastInvoice?.InvoiceDate.MiladiToPersianDate().year)
        {
            lastinvoiceNo = "1";
        }
        invoiceNo.Append(lastinvoiceNo);

        return invoiceNo.ToString();
    }

    private uint CalculateTotalDwellingHour(VesselStoppage vesselStoppage, out uint dwellingDay, out uint stormHour)
    {
        vesselStoppage.ATADayOfTheWeek = vesselStoppage.ATA?.DayOfWeek;
        vesselStoppage.ETADayOfTheWeek = vesselStoppage.ETA?.DayOfWeek;
        vesselStoppage.ATDDayOfTheWeek = vesselStoppage.ATD?.DayOfWeek;
        vesselStoppage.ETDDayOfTheWeek = vesselStoppage.ETD?.DayOfWeek;
        vesselStoppage.StartStormDayOfTheWeek = vesselStoppage.StartStorm?.DayOfWeek;
        vesselStoppage.EndStormDayOfTheWeek = vesselStoppage.EndStorm?.DayOfWeek;
        TimeSpan stormHours = TimeSpan.Zero;
        stormHour = 0;
        if (vesselStoppage.StartStorm != null && vesselStoppage.EndStorm != null)
        {
            stormHours = (TimeSpan)(vesselStoppage.EndStorm - vesselStoppage.StartStorm);
            stormHour = (uint)stormHours.TotalHours;
        }
        TimeSpan diffrence = (TimeSpan)(vesselStoppage.ATD - vesselStoppage.ATA);
        diffrence -= stormHours;
        uint dwellingHour = (uint)Math.Ceiling(diffrence.TotalHours);
        dwellingDay = dwellingHour % 24 == 0 ? dwellingHour / 24 : (dwellingHour / 24) + 1;
        return dwellingHour;
    }

    private async Task<Currency> PrepareCurrentCurrencyRateAsync(VesselStoppage vesselStoppage)
    {
        return await _neginDbContext.Currencies.AsNoTracking().Where(c => c.Date <= vesselStoppage.ATA).OrderBy(c => c.Date).LastAsync();
    }

    #region Tariffs
    private async Task<VesselStoppageTariff> PrepareVesselStoppageTariff(DateTime ata)
    {
        return await _neginDbContext.VesselStoppageTariffs.Include(c => c.VesselStoppageTariffDetails).AsNoTracking()
                         .Where(c => c.EffectiveDate <= ata).OrderBy(c => c.EffectiveDate).LastAsync();

    }
    private VesselStoppageTariffDetails PrepareVesselStoppageTariffDetails(Voyage voyage, VesselStoppageTariff vesselStoppageTariff)
    {
        return vesselStoppageTariff.VesselStoppageTariffDetails.Where(c => c.VesselTypeId == voyage.Vessel?.VesselTypeId).Single();
    }
    private async Task<CleaningServiceTariff> PrepareCleaningServiceTariff(DateTime ata)
    {
        return await _neginDbContext.CleaningServiceTariffs.Include(c => c.CleaningServiceTariffDetails).AsNoTracking()
            .Where(c => c.EffectiveDate <= ata).OrderBy(c => c.EffectiveDate).LastAsync();

    }
    private CleaningServiceTariffDetails PrepareCleaningServiceTariffDetails(Voyage voyage, CleaningServiceTariff cleaningServiceTariff)
    {
        return cleaningServiceTariff.CleaningServiceTariffDetails.Where(c => c.GrossWeight >= voyage.Vessel?.GrossTonage).OrderBy(c => c.GrossWeight).First();
    }

    #endregion

    #region Pricing
    private decimal CalculateVesselStoppagePriceDollar(VesselStoppageTariffDetails vesselStoppageTariffDetail, Voyage voyage, uint dwellingHour)
    {
        decimal priceDVS = 0;
        if (dwellingHour <= vesselStoppageTariffDetail?.NormalHour || (vesselStoppageTariffDetail?.NormalHour == 1 && vesselStoppageTariffDetail?.ExtraPrice == 0))
        {
            priceDVS = Math.Round(Convert.ToDecimal((dwellingHour * vesselStoppageTariffDetail.NormalPrice * voyage.Vessel?.GrossTonage) / 100), 2);
        }
        else
        {
            var extraH = dwellingHour - vesselStoppageTariffDetail?.NormalHour;
            var normalH = dwellingHour - extraH;
            priceDVS = Math.Round((Convert.ToDecimal((normalH * vesselStoppageTariffDetail?.NormalPrice * voyage.Vessel?.GrossTonage) / 100) + Convert.ToDecimal((extraH * vesselStoppageTariffDetail?.ExtraPrice * voyage.Vessel?.GrossTonage) / 100)), 2);
        }
        return priceDVS;
    }
    private decimal CalculateCleaningServicePriceDollar(CleaningServiceTariffDetails cleaningServiceTariffDetail, uint dwellingDay)
    {
        return Math.Round(Convert.ToDecimal(dwellingDay * cleaningServiceTariffDetail?.Price), 2);
    }

    private double CalculateDiscount(Voyage voyage, DateTime ata,ref decimal priceDVS, ref decimal priceDCS)
    {
        if (voyage.Vessel?.FlagId == 207 && voyage.Vessel.GrossTonage <= 1000)
        {
            DiscountTariff? discountTariff =  _neginDbContext.DiscountTariffs.AsNoTracking()
                                            .Where(c => c.EffectiveDate <= ata).OrderBy(c => c.EffectiveDate).LastOrDefault();
            priceDVS = Math.Round(priceDVS - (priceDVS * Convert.ToDecimal(discountTariff?.DiscountPercent)), 2);
            priceDCS = Math.Round(priceDCS - (priceDCS * Convert.ToDecimal(discountTariff?.DiscountPercent)), 2);
            return discountTariff != null ? discountTariff.DiscountPercent * 100 : 0;
        }
        return 0;
    }
    private ulong CalculatePriceRial(Voyage voyage, Currency currency, decimal priceD)
    {
        if (voyage.Vessel?.FlagId == 207)
        {
            return Convert.ToUInt64(priceD * currency.PersianDollerRate);
        }
        else
        {
            return Convert.ToUInt64(priceD * currency.ForeignDollerRate);
        }

    }
    private decimal RialVatToDollarVat(Currency currency, ulong sumPriceRVat, Voyage voyage)
    {
        decimal result;
        if (voyage.Vessel?.FlagId == 207)
        {
            result = (decimal)sumPriceRVat / currency.PersianDollerRate;
        }
        else
        {
            result = (decimal)sumPriceRVat / currency.ForeignDollerRate;
        }
        return decimal.Round(result, 2);
    }

    #endregion

    #endregion

}
