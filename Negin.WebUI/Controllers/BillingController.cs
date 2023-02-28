using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Interfaces;
using Negin.Services.Operation;
using SmartBreadcrumbs.Attributes;
using Negin.WebUI.Models.ViewModels;
using Negin.Framework.Utilities;

namespace Negin.WebUI.Controllers;

[Authorize]
public class BillingController : Controller
{
    private readonly IVesselRepository _vesselRepository;
    private readonly IVoyageRepository _voyageRepository;
    private readonly IVesselStoppageService _vesselStoppageService;

    public BillingController(IVesselRepository vesselRepository, IVoyageRepository voyageRepository, IVesselStoppageService vesselStoppageService)
    {
        _vesselRepository = vesselRepository;
        _voyageRepository = voyageRepository;
        _vesselStoppageService = vesselStoppageService;
    }

    [Breadcrumb("Invoices")]
    public IActionResult List(int pageNumber = 1, int pageCount = 10, string filter = "")
    {
        var voyages = _voyageRepository.GetPaginationVoyagesForBillingAsync(pageNumber, pageCount, filter).Result;
        voyages.PageInfo.Title = "Invoices";
        voyages.PageInfo.Filter = filter;

        var model = new InvoiceViewModel
        {
            Voyages = voyages,
            ActiveVoyages = voyages.Data.Count(),
            Gone = voyages.Data.SelectMany(c=>c.VesselStoppages).Where(c=>c.Status == VesselStoppage.VesselStoppageStatus.Gone).Count(),
            InProcess = voyages.Data.SelectMany(c => c.VesselStoppages).Where(c => c.Status == VesselStoppage.VesselStoppageStatus.InProcess).Count(),
            WaitForVessel = voyages.Data.SelectMany(c => c.VesselStoppages).Where(c => c.Status == VesselStoppage.VesselStoppageStatus.WaitForVessel).Count()
        };

        ViewData["ActiveLink"] = "invoice";
        return View(model);
    }

    public IActionResult VoyageList(int pageNumber = 1, int pageCount = 10, string filter = "") 
    {
        var model = _voyageRepository.GetPaginationVoyagesForBillingAsync(pageNumber, pageCount, filter).Result;
        model.PageInfo.Filter = filter;
        return PartialView("Shared/_VoyageInvoice", model); 
    }

    [Authorize(Roles ="admin")]
    [Breadcrumb("CreateInvoice")]
    public IActionResult CreateInvoice(ulong voyageId, int pageNumber = 1, int pageSize = 10, string filter = "")
    {
        var model = new CreateInvoiceViewModel
        {
            Voyage = _voyageRepository.GetVoyageById(voyageId).Result,
            VesselStoppages = _voyageRepository.GetPaginationVesselStoppagesAsync(voyageId, pageNumber, pageSize, filter).Result
        };
        model.VesselStoppages.PageInfo.Title = "Create Invoice";
        model.VesselStoppages.PageInfo.Filter = filter;
        ViewData["ActiveLink"] = "invoice";
        return View(model);
    }
}
