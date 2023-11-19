using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Aggregates.Basic;
using Negin.Core.Domain.Aggregates.Operation;
using Negin.Core.Domain.Interfaces;
using Negin.Services.Operation;
using SmartBreadcrumbs.Attributes;

namespace Negin.WebUI.Controllers;

[Authorize]
public class OperationController : Controller
{
    private readonly IVesselRepository _vesselRepository;
    private readonly IVoyageRepository _voyageRepository;
    private readonly IVesselStoppageService _vesselStoppageService;

    public OperationController(IVesselRepository vesselRepository, IVoyageRepository voyageRepository, IVesselStoppageService vesselStoppageService)
    {
        _vesselRepository = vesselRepository;
        _voyageRepository = voyageRepository;
        _vesselStoppageService = vesselStoppageService;
    }

    [Breadcrumb("VesselStoppages")]
    public IActionResult List()
    {
        var model = _vesselRepository.GetVesselsAssignedVoyage().Result;
        ViewData["ActiveLink"] = "vesselStoppage";

        return View("SelectVesselStoppage", model);
    }


    public JsonResult GetVoyageNoIn(ulong vesselId)
    {
        return Json(_voyageRepository.GetVoyageByVesselId(vesselId).Result);
    }

    public IActionResult VesselStoppageList(string vesselName, ulong voyageId, int pageNumber = 1, int pageCount = 10, string filter = "")
    {
        var model = _voyageRepository.GetPaginationVesselStoppagesAsync(voyageId, pageNumber, pageCount, filter).Result;
        ViewBag.VesselName = vesselName;
        ViewBag.VoyageId = voyageId;
        return View(model);
    }

    [Authorize(Roles = "admin")]
    [Breadcrumb("AddVesselStoppage", FromAction = "List", FromController = typeof(OperationController))]
    public IActionResult AddVesselStoppage(string vesselName, ulong voyageId)
    {
        ViewData["ActiveLink"] = "vesselStoppage";
        ViewBag.VoyageId = voyageId;
        ViewBag.Ports = _voyageRepository.GetAllPorts().Result;
        ViewBag.VesselName = vesselName;
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [Breadcrumb("AddVesselStoppage", FromAction = "List", FromController = typeof(OperationController))]
    public IActionResult AddVesselStoppage(VesselStoppage v, IFormCollection formCollection)
    {
        if (ModelState.IsValid)
        {
            var result = _vesselStoppageService.AddVesselStoppage(v, formCollection);
            if (result.State)
            {
                return RedirectToAction("List");
            }
            else
            {
                ModelState.AddModelError("", result.Message ?? string.Empty);
            }
        }

        ViewData["ActiveLink"] = "vesselStoppage";
        ViewBag.VoyageId = v.VoyageId;
        ViewBag.Ports = _voyageRepository.GetAllPorts().Result;
        ViewBag.VesselName = formCollection["vesselName"];
        ViewBag.VoyageNoIn = formCollection["voyageNoIn"];
        return View();
    }

    [Authorize(Roles = "admin")]
    [Breadcrumb("EditVesselStoppage", FromAction = "List", FromController = typeof(OperationController))]
    public IActionResult EditVesselStoppage(string vesselName, ulong id, ulong voyageId)
    {
        ViewData["ActiveLink"] = "vesselStoppage";
        ViewBag.VesselStoppageId = id;
        ViewBag.VoyageId = voyageId;
        ViewBag.Ports = _voyageRepository.GetAllPorts().Result;
        var model = _voyageRepository.GetVesselStoppageByVoyageId(id).Result;
        ViewBag.VesselName = vesselName;
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    [Breadcrumb("EditVesselStoppage", FromAction = "List", FromController = typeof(OperationController))]
    public IActionResult EditVesselStoppage(VesselStoppage v, IFormCollection formCollection)
    {
        if (ModelState.IsValid)
        {
            var result = _vesselStoppageService.UpdateVesselStoppage(v, formCollection);
            if (result.State)
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
        ViewBag.VoyageId = v.VoyageId;
        ViewBag.Ports = _voyageRepository.GetAllPorts().Result;
        ViewBag.VesselName = formCollection["vesselName"];
        return View(v);
    }
}
