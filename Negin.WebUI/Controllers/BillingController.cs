using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Entities.Billing;
using Negin.Core.Domain.Interfaces;
using SmartBreadcrumbs.Attributes;
using System.ComponentModel;
using static Negin.Framework.Exceptions.SqlException;

namespace Negin.WebUI.Controllers;

[Authorize]
public class BillingController : Controller
{
    private readonly IVesselRepository _vesselRepository;
    private readonly IVoyageRepository _voyageRepository;

    public BillingController(IVesselRepository vesselRepository, IVoyageRepository voyageRepository)
    {
        _vesselRepository = vesselRepository;
        _voyageRepository = voyageRepository;
    }

    [Breadcrumb("VesselStoppages")]
    public IActionResult List()
    {
        var model = _vesselRepository.GetAllVessels().Result;
        ViewData["ActiveLink"] = "vesselStoppage";

        return View("SelectVesselStoppage", model);
    }

    public JsonResult GetVoyageNoIn(ulong vesselId)
    {
        return Json(_voyageRepository.GetVoyageByVesselId(vesselId).Result);
    }

    public IActionResult VesselStoppageList(ulong voyageId, int pageNumber = 1, int pageCount = 10, string filter = "")
    {
        var model = _voyageRepository.GetPaginationVesselStoppagesAsync(voyageId, pageNumber, pageCount, filter).Result;
        ViewBag.VoyageId = voyageId;
        return View(model);
    }

    [Breadcrumb("AddVesselStoppages", FromAction = "List", FromController = typeof(BillingController))]
    public IActionResult AddVesselStoppage(ulong voyageId)
    {
        ViewData["ActiveLink"] = "vesselStoppage";
        ViewBag.VoyageId = voyageId;
        ViewBag.Ports = _voyageRepository.GetAllPorts().Result;
        return View();
    }

    [HttpPost]
    [Breadcrumb("AddVesselStoppages", FromAction = "List", FromController = typeof(BillingController))]
    public IActionResult AddVesselStoppage(VesselStoppage v, IFormCollection formCollection)
    {
        if (ModelState.IsValid)
        {
            var result = _voyageRepository.CreateVesselStoppageAsync(v, formCollection).Result;
            if (result.State == SqlExceptionMessages.Success)
            {
                return RedirectToAction("List");
            }
            else
            {
                ModelState.AddModelError("", result.Message ?? string.Empty);
            }
        }

        return View();
    }

    [Breadcrumb("EditVesselStoppages", FromAction = "List", FromController = typeof(BillingController))]
    public IActionResult EditVesselStoppage(ulong id)
    {
        ViewData["ActiveLink"] = "vesselStoppage";
        ViewBag.VesselStoppageId = id;
        ViewBag.Ports = _voyageRepository.GetAllPorts().Result;
        var model = _voyageRepository.GetVesselStoppageByVoyageId(id).Result;
        return View(model);
    }

    [HttpPost]
    [Breadcrumb("EditVesselStoppages", FromAction = "List", FromController = typeof(BillingController))]
    public IActionResult EditVesselStoppage(VesselStoppage v, IFormCollection formCollection)
    {
        if (ModelState.IsValid)
        {
            var result = _voyageRepository.UpdateVesselStoppageAsync(v, formCollection).Result;
            if (result.State == SqlExceptionMessages.Success)
            {
                return RedirectToAction("List");
            }
            else
            {
                ModelState.AddModelError("", result.Message ?? string.Empty);
            }

        }
        ViewData["ActiveLink"] = "vesselStoppage";
        ViewBag.VesselStoppageId = v.Id;
        ViewBag.Ports = _voyageRepository.GetAllPorts().Result;
        return View(v);
    }
}
