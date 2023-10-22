using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Interfaces;
using Negin.WebUI.Models.ViewModels;
using SmartBreadcrumbs.Attributes;

namespace Negin.WebUI.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IVoyageRepository _voyageRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    public DashboardController(IVoyageRepository voyageRepository, IInvoiceRepository invoiceRepository)
    {
        _voyageRepository = voyageRepository;
        _invoiceRepository = invoiceRepository;
    }

    [DefaultBreadcrumb("Home")]
    public async Task<IActionResult> List(string year = "1402")
    {
        var voyages = _voyageRepository.GetAllVoyageForDashboard().Result;

        var model = new DashboardViewModel
        {
            ActiveVoyages = voyages.ActiveVoyages,
            Gone = voyages.Gone,
            InProcess = voyages.InProcess,
            WaitForVessel = voyages.WaitForVessel,
            VesselStoppageCount = voyages.VesselStoppageCount,
            Invoiced = voyages.Invoiced,
            Confirmed = (uint)await _invoiceRepository.InvoiceConfirmedCount(),
            Years = await _invoiceRepository.GetExistYear(),
            Chart1Data = _invoiceRepository.GetSumPriceProc(year)
        };

        ViewData["ActiveLink"] = "dashboard";

		return View(model);
	}
}
