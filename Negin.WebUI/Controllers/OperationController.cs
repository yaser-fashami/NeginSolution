using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Entities.Operation;
using Negin.Core.Domain.Interfaces;
using Negin.Framework.Exceptions;
using Negin.Framework.Utilities;
using Negin.Services.Operation;
using Negin.WebUI.Models.ViewModels;
using SmartBreadcrumbs.Attributes;

namespace Negin.WebUI.Controllers;

[Authorize]
public class OperationController : Controller
{
    private readonly IBasicInfoRepository _basicInfoRepository;
    private readonly IOperationRepository _operationRepository;
    private readonly IVesselStoppageService _vesselStoppageService;

    public OperationController(IVesselStoppageService vesselStoppageService, IOperationRepository operationRepository, IBasicInfoRepository basicRepository)
    {
        _vesselStoppageService = vesselStoppageService;
        _operationRepository = operationRepository;
        _basicInfoRepository = basicRepository;
    }

    [Breadcrumb("VesselStoppages")]
    public IActionResult List()
    {
        var model = _basicInfoRepository.GetAllVessels().Result;
        ViewData["ActiveLink"] = "vesselStoppage";

        return View("SelectVesselStoppage", model);
    }


    public JsonResult GetVoyageNoIn(ulong vesselId)
    {
        return Json(_basicInfoRepository.GetVoyageByVesselId(vesselId).Result);
    }

    public IActionResult VesselStoppageList(string vesselName, ulong voyageId, int pageNumber = 1, int pageCount = 10, string filter = "")
    {
        var model = _operationRepository.GetPaginationVesselStoppagesAsync(vesselName, pageNumber, pageCount, filter).Result;
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
        ViewBag.Ports = _basicInfoRepository.GetAllPorts().Result;
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
        ViewBag.Ports = _basicInfoRepository.GetAllPorts().Result;
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
        ViewBag.Ports = _basicInfoRepository.GetAllPorts().Result;
        var model = _operationRepository.GetVesselStoppageByVoyageId(id).Result;
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
        ViewBag.Ports = _basicInfoRepository.GetAllPorts().Result;
        ViewBag.VesselName = formCollection["vesselName"];
        return View(v);
    }



    [Authorize(Roles = "admin")]
    [Breadcrumb("Loading&Discharge", FromAction = "List", FromController = typeof(DashboardController))]
    public IActionResult LoadingAndDischarge()
    {
        var model = _operationRepository.GetAllVSForLoadAndDischargeAsync().Result;
        ViewData["ActiveLink"] = "loadingAndDischarge";
        return View("LoadingAndDischarge", model);
    }

    public IActionResult LoadingDischargeList(ulong vesselStoppageId, int pageNumber = 1, int pageCount = 10, string filter = "")
    {
        var model = _operationRepository.GetPaginationLoadingDischargeAsync(vesselStoppageId, pageNumber, pageCount, filter).Result;
        model.PageInfo.Title = "Loading & Discharge List";
        model.PageInfo.Filter = filter;
        model.PageInfo.PageNumber = pageNumber;
        ViewBag.vsId = vesselStoppageId;
        return View(model);
    }

    [Authorize(Roles = "admin")]
    [Breadcrumb("AddLoading&Discharge", FromAction = "LoadingAndDischarge", FromController = typeof(OperationController))]
    public IActionResult AddLoadingDischarge(string m, int vsId)
    {
        CreateLoadingDischargeViewModel model = new();
        if (m == LoadingDischarge.LoadingDischargeMethod.LOAD.ToString())
        {
            ViewData["Title"] = "Loading";
        }
        else if (m == LoadingDischarge.LoadingDischargeMethod.DISC.ToString())
        {
            ViewData["Title"] = "Discharge";
        }
        model.Method = m;
        model.VesselStoppageId = vsId;
        model.LoadingDischargeTariffDetails = _basicInfoRepository.GetAllLoadingDischargeTariffDetailsAsync().Result;

        ViewData["ActiveLink"] = "loadingAndDischarge";
        return View(model);
    }

    [HttpPost]
    public JsonResult AddLoadingDischarge(LoadingDischarge loadingDischarge)
    {
        var result = _operationRepository.CreateLoadingDischargeAsync(loadingDischarge).Result;
        return Json(new { message = result.Message, state = result.State });
    }

    [Authorize(Roles = "admin")]
    [Breadcrumb("EditLoadingDischarge", FromAction = "LoadingAndDischarge", FromController = typeof(OperationController))]
    public async Task<IActionResult> EditLoadingDischarge(ulong id)
    {
        var model = await _operationRepository.GetLoadingDischargeById(id);
        ViewBag.LoadingDischargeTariffDetails = await _basicInfoRepository.GetAllLoadingDischargeTariffDetailsAsync();

        if (model.Method == LoadingDischarge.LoadingDischargeMethod.LOAD)
        {
            ViewBag.Method = "Loading";
        }
        else if(model.Method == LoadingDischarge.LoadingDischargeMethod.DISC)
        {
            ViewBag.Method = "Discharge";
        }
        ViewData["ActiveLink"] = "loadingAndDischarge";
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public BLMessage EditLoadingDischarge(LoadingDischarge loadingDischarge)
    {
        BLMessage result = new();
        if (loadingDischarge.Tonage != 0)
        {
            var res = _operationRepository.UpdateLoadingDischargeAsync(loadingDischarge).Result;
            result.Message = res.Message;
            result.State = res.State;
        }
        else
        {
            result.Message = "Tonage can`t be zero or Null";
            result.State = false;
        }
        return result;
    }

    [Authorize(Roles = "admin")]
    public BLMessage DeleteLoadingDischarge(ulong id)
    {
        BLMessage result = new();
        var res = _operationRepository.DeleteLoadingDischargeAsync(id).Result;
        result.State = res.State;
        result.Message = res.Message;
        return result;
    }
}
