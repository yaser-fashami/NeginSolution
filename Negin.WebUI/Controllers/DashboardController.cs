using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Interfaces;
using Negin.WebUI.Models.ViewModels;
using SmartBreadcrumbs.Attributes;

namespace Negin.WebUI.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IBasicInfoRepository _basicInfoRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    public DashboardController(IBasicInfoRepository basicInfoRepository, IInvoiceRepository invoiceRepository)
    {
        _basicInfoRepository = basicInfoRepository;
        _invoiceRepository = invoiceRepository;
    }

    [DefaultBreadcrumb("Dashboard")]
    public async Task<IActionResult> List(string year = "1402")
    {
        var voyages = _basicInfoRepository.GetAllVoyageForDashboard().Result;

        var model = new DashboardViewModel
        {
            ActiveVoyages = voyages.ActiveVoyages,
            Gone = voyages.Gone,
            InProcess = voyages.InProcess,
            WaitForVessel = voyages.WaitForVessel,
            VesselStoppageCount = voyages.VesselStoppageCount,
            Invoiced = voyages.Invoiced,
            NotInvoiced = voyages.NotInvoiced,
            Confirmed = (uint)await _invoiceRepository.InvoiceConfirmedCount(),
            Years = await _invoiceRepository.GetExistYear(),
            Chart1Data = _invoiceRepository.GetSumPriceProc(year)
        };

        ViewData["ActiveLink"] = "dashboard";

		return View(model);
	}
}
