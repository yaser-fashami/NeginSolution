using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Interfaces;
using SmartBreadcrumbs.Attributes;
using Negin.WebUI.Models.ViewModels;
using Negin.Services.Billing;
using Negin.Core.Domain.Dtos;
using Negin.Framework.Exceptions;
using Newtonsoft.Json;
using Negin.Core.Domain.Entities.Operation;
using Negin.Core.Domain.Entities.Billing;

namespace Negin.WebUI.Controllers;

[Authorize]
public class BillingController : Controller
{
    private readonly IBasicInfoRepository _basicInfoRepository;
    private readonly IInvoiceCalculator _invoiceCalculator;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOperationRepository _operationRepository;

    public BillingController(IBasicInfoRepository basicInfoRepository, IInvoiceCalculator invoiceCalculator, IInvoiceRepository invoiceRepository, IOperationRepository operationRepository)
    {
        _basicInfoRepository = basicInfoRepository;
        _invoiceCalculator = invoiceCalculator;
        _invoiceRepository = invoiceRepository;
        _operationRepository = operationRepository;
    }

    [Breadcrumb("VesselStoppageInvoice", FromAction = "List", FromController = typeof(DashboardController))]
    public async Task<IActionResult> List(int pageNumber = 1, int pageCount = 10, string filter = "", bool? invoiced = null)
    {
        var voyages = await _invoiceRepository.GetPaginationDataForBillingAsync(pageNumber, pageCount, filter, invoiced);
        voyages.PageInfo.Title = "Vessel Stoppage Invoices";
        voyages.PageInfo.Filter = filter;
        voyages.PageInfo.PageName = "List";
        voyages.PageInfo.Specification = $"{invoiced}";

        var model = new InvoiceViewModel
        {
            Items = voyages
        };

        ViewData["ActiveLink"] = "invoice";
        return View(model);
    }

    [Authorize(Roles ="admin")]
    [Breadcrumb("CreateInvoice", FromAction ="List", FromController = typeof(BillingController))]
    public IActionResult CreateInvoice(ulong voyageId, int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        var model = new CreateInvoiceViewModel
        {
            Voyage = _basicInfoRepository.GetVoyageById(voyageId).Result,
            VesselStoppages = _operationRepository.GetPaginationVesselStoppagesForInvoiceAsync(voyageId, pageNumber, pageSize).Result
        };
        model.VesselStoppages.PageInfo.Title = "Create Invoice";
        model.VesselStoppages.PageInfo.Filter = filter;
        ViewData["ActiveLink"] = "invoice";
        return View(model);
    }

    [HttpPost]
	[Authorize(Roles = "admin")]
	[Breadcrumb("PreInvoice", FromAction = "List", FromController = typeof(BillingController))]
	public IActionResult PreInvoice(ulong voyageId, IEnumerable<ulong> vesselStoppages)
    {
        PreInvoiceViewModel model = null;
        try
        {
            var preInvoice = _invoiceCalculator.CalculateAsync(voyageId, vesselStoppages).Result;

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            };
            HttpContext.Session.SetString("PreInvoice", JsonConvert.SerializeObject(preInvoice, jsonSerializerSettings));
            model = new PreInvoiceViewModel(preInvoice);
        }
        catch (Exception ex)
        {

            throw;
        }

		ViewData["ActiveLink"] = "invoice";
		return View(model);
    }

    [Authorize(Roles = "admin")]
    public BLMessage Invoicing()
    {
        BLMessage result = null;
        try
        {
            var preInvoiceStr = HttpContext.Session.GetString("PreInvoice");
            if (string.IsNullOrEmpty(preInvoiceStr))
            {
                result = new BLMessage() { State = false, Message = "You have to go back and go through the steps again" };
            }
            else
            {
                var preInvoice = JsonConvert.DeserializeObject<PreInvoiceDto>(preInvoiceStr);
                if (preInvoice != null)
                {
                    result = _invoiceCalculator.Invoicing(preInvoice).Result;
                    if (result.State == true)
                    {
                        HttpContext.Session.Remove("PreInvoice");
                    }
                }    
            }
        }
        catch (Exception)
        {

            throw;
        }

        return result;
    }

    //[Breadcrumb("Issued", FromAction = "List", FromController = typeof(DashboardController))]
    //public IActionResult IssuedInvoiceList(int pageNumber = 1, int pageSize = 10, string filter = "")
    //{
    //    var invoices = _invoiceRepository.GetPaginationInvoiceAsync(pageNumber, pageSize, filter).Result;
    //    invoices.PageInfo.Title = "Issued Invoices";
    //    invoices.PageInfo.Filter = filter;
    //    invoices.PageInfo.PageName = "IssuedInvoiceList";

    //    ViewData["ActiveLink"] = "issued";
    //    return View(invoices);
    //}

	[Authorize(Roles = "admin")]
	[Breadcrumb("InvoiceDetails", FromAction = "List", FromController = typeof(BillingController))]
    public IActionResult IssuedInvoiceDetails(ulong id)
    {
        var invoice = _invoiceRepository.GetInvoiceDetailsById(id).Result;

        ViewData["ActiveLink"] = "issued";
        return View(invoice);
    }

    [Authorize(Roles = "admin")]
    public BLMessage CancelInvoice(ulong id)
    {
        return _invoiceRepository.CancelInvoice(id).Result;
    }

    [Authorize(Roles = "admin")]
    public BLMessage ConfirmInvoice(ulong id)
    {
        return _invoiceRepository.ConfirmInvoice(id).Result;
    }

	[Breadcrumb("Loading&DischargeInvoice", FromAction = "List", FromController = typeof(DashboardController))]
	public IActionResult LoadingDischargeInvoiceList(int pageNumber = 1, int pageCount = 10, string filter = "")
    {
        var loadingDischarges = _operationRepository.GetPaginationLoadingDischargeForInvoicingAsync(pageNumber, pageCount, filter).Result;
        loadingDischarges.PageInfo.Title = "Loading & Discharge Invoices";
        loadingDischarges.PageInfo.Filter = filter;
        loadingDischarges.PageInfo.PageName = "LoadingDischargeInvoice";

		ViewData["ActiveLink"] = "loadingDischargeInvoice";
        return View(loadingDischarges);
    }

    [Authorize(Roles = "admin")]
    [Breadcrumb("CreateL/DInvoice", FromAction = "LoadingDischargeInvoiceList", FromController = typeof(BillingController))]
    public IActionResult CreateLoadingDischargeInvoice(ulong loadingDischargeId)
    {
        ViewBag.Companies = _basicInfoRepository.GetAllPorterageCompanies().Result;
        ViewBag.LoadingDischargeId = loadingDischargeId;
        ViewData["ActiveLink"] = "loadingDischargeInvoice";
        return View();
    }

    [HttpPost]
	[Authorize(Roles = "admin")]
	[Breadcrumb("CreateL/DInvoice", FromAction = "LoadingDischargeInvoiceList", FromController = typeof(BillingController))]
	public IActionResult CreateLoadingDischargeInvoice(LoadingDischargeInvoice loadingDischargeInvoice)
    {
        ViewData["ActiveLink"] = "loadingDischargeInvoice";

        if (ModelState.IsValid)
        {
            var preLoadingDischargeInvoiceDto = _invoiceCalculator.CalculateLoadingDischargeInvoiceAsync(
                                                        new LoadingDischargeInvoiceDto(loadingDischargeInvoice.LoadingDischargeId,
                                                                                       loadingDischargeInvoice.ShippingLineCompanyId,
                                                                                       loadingDischargeInvoice.DiscountPercent)).Result;
            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            };
            HttpContext.Session.SetString("PreLoadingDischargeInvoice", JsonConvert.SerializeObject(preLoadingDischargeInvoiceDto, jsonSerializerSettings));

            return View("PreLoadingDischargeInvoice", preLoadingDischargeInvoiceDto);
        }
        ViewBag.Companies = _basicInfoRepository.GetAllPorterageCompanies().Result;
        ViewBag.LoadingDischargeId = loadingDischargeInvoice.Id;
        return View();
    }

    [Authorize(Roles = "admin")]
    public BLMessage LoadingDischargeInvoicing()
    {
        BLMessage result = null;
        try
        {
            var preLoadingDischargeInvoiceStr = HttpContext.Session.GetString("PreLoadingDischargeInvoice");
            if (string.IsNullOrEmpty(preLoadingDischargeInvoiceStr))
            {
                result = new BLMessage() { State = false, Message = "You have to go back and go through the steps again" };
            }
            else
            {
                var preLoadingDischargeInvoice = JsonConvert.DeserializeObject<PreLoadingDischargeInvoiceDto>(preLoadingDischargeInvoiceStr);
                if (preLoadingDischargeInvoice != null)
                {
                    result = _invoiceCalculator.LoadingDischargeInvoicing(preLoadingDischargeInvoice).Result;
                    if (result.State == true)
                    {
                        HttpContext.Session.Remove("PreLoadingDischargeInvoice");
                    }
                }
            }
        }
        catch (Exception)
        {

            throw;
        }

        return result;
    }

    [Breadcrumb("Details", FromAction = "LoadingDischargeInvoiceList", FromController = typeof(BillingController))]
    public IActionResult LoadingDischargeInvoiceDetail(ulong loadingDischargeId)
    {
        var LDInvoice = _invoiceRepository.GetLoadingDischargeInvoice(loadingDischargeId).Result;

        ViewData["ActiveLink"] = "loadingDischargeInvoice";
        return View(LDInvoice);
    }

    [Authorize(Roles = "admin")]
    public BLMessage CancelLoadinDischargeInvoice(ulong id)
    {
        return _invoiceRepository.CancelLoadinDischargeInvoice(id).Result;
    }

    [Authorize(Roles = "admin")]
    public BLMessage ConfirmLoadingDischargeInvoice(ulong id)
    {
        return _invoiceRepository.ConfirmLoadingDischargeInvoice(id).Result;
    }

}
