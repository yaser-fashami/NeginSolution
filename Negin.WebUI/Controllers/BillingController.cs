using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Interfaces;
using SmartBreadcrumbs.Attributes;
using Negin.WebUI.Models.ViewModels;
using Negin.Services.Billing;
using Negin.Core.Domain.Dtos;
using Negin.Framework.Exceptions;
using Newtonsoft.Json;

namespace Negin.WebUI.Controllers;

[Authorize]
public class BillingController : Controller
{
    private readonly IVoyageRepository _voyageRepository;
    private readonly IInvoiceCalculator _invoiceCalculator;
    private readonly IInvoiceRepository _invoiceRepository;

    public BillingController(IVoyageRepository voyageRepository, IInvoiceCalculator invoiceCalculator, IInvoiceRepository invoiceRepository)
    {
        _voyageRepository = voyageRepository;
        _invoiceCalculator = invoiceCalculator;
        _invoiceRepository = invoiceRepository;
    }

    [Breadcrumb("Issuing")]
    public IActionResult List(int pageNumber = 1, int pageCount = 10, string filter = "")
    {
        var voyages = _voyageRepository.GetPaginationVoyagesForBillingAsync(pageNumber, pageCount, filter).Result;
        voyages.PageInfo.Title = "Invoices";
        voyages.PageInfo.Filter = filter;

        var model = new InvoiceViewModel
        {
            Voyages = voyages,
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
            Voyage = _voyageRepository.GetVoyageById(voyageId).Result,
            VesselStoppages = _voyageRepository.GetPaginationVesselStoppagesForInvoiceAsync(voyageId, pageNumber, pageSize).Result
        };
        model.VesselStoppages.PageInfo.Title = "Create Invoice";
        model.VesselStoppages.PageInfo.Filter = filter;
        ViewData["ActiveLink"] = "invoice";
        return View(model);
    }

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

    [Breadcrumb("Issued", FromAction = "List", FromController = typeof(DashboardController))]
    public IActionResult IssuedInvoiceList(int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        var invoices = _invoiceRepository.GetPaginationInvoiceAsync(pageNumber, pageSize, filter).Result;
        invoices.PageInfo.Title = "Issued Invoices";
        invoices.PageInfo.Filter = filter;

        ViewData["ActiveLink"] = "issued";
        return View(invoices);
    }

	[Authorize(Roles = "admin")]
	[Breadcrumb("InvoiceDetails", FromAction = "IssuedInvoiceList", FromController = typeof(BillingController))]
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

}
