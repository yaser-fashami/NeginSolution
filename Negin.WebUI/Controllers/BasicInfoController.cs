using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Interfaces;
using Negin.WebUI.Models;
using Negin.WebUI.Models.ViewModels;
using SmartBreadcrumbs.Attributes;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Negin.Framework.Exceptions;
using Negin.Services.BasicInfo;

namespace Negin.WebUI.Controllers
{
    [Authorize]
	public class BasicInfoController : Controller
    {
        private readonly ILogger<BasicInfoController> _logger;
        private readonly IBasicInfoRepository _basicInfoRepository;
		private readonly IBasicInfoService _basicInfoService;
        private readonly IAppVersionService _appVersionService;

        public BasicInfoController(ILogger<BasicInfoController> logger,
			IBasicInfoRepository basicIOnfoRepository,
			IBasicInfoService basicInfoService,
			IAppVersionService appVersionService)
		{
			_logger = logger;
			_basicInfoRepository = basicIOnfoRepository;
			_basicInfoService = basicInfoService;
            _appVersionService = appVersionService;
        }

		#region Vessel
		[Breadcrumb("Vessel")]
		public async Task<IActionResult> List(int pageNumber = 1, int pageCount = 10, string filter = "")
        {
            var model = await _basicInfoRepository.GetPaginationVesselsAsync(pageNumber, pageCount, filter);
            model.PageInfo.Title = "Vessel List";
            model.PageInfo.Filter = filter;
			model.PageInfo.PageName = "List";
            ViewData["ActiveLink"] = "vessel";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateVessel", FromAction = "List", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> CreateVessel()
        {
			ViewData["ActiveLink"] = "vessel";
			VesselViewModel model = await PrepareVesselViewModel();
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateVessel", FromAction = "List", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> CreateVessel(VesselViewModel newVessel)
        {
            if (ModelState.IsValid)
            {
                var result = await _basicInfoRepository.CreateVesselAsync(newVessel.Vessel);
                if (result.State)
                {
                    return RedirectToAction("List");
				}
				else
				{
					ModelState.AddModelError("", result.Message ?? string.Empty);
				}
            }

            var model = await PrepareVesselViewModel();
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [Breadcrumb("EditVessel", FromAction = "List", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> EditVessel(ulong vesselId)
        {
            ViewData["ActiveLink"] = "vessel";
			var model = await PrepareVesselViewModel();
            model.Vessel = await _basicInfoRepository.GetVesselById(vesselId);

			return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Breadcrumb("EditVessel", FromAction = "List", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> EditVessel(VesselViewModel v)
        {
			if (ModelState.IsValid)
            {
                var result = await _basicInfoRepository.UpdateVesselAsync(v.Vessel);
				if (result.State)
				{
					return RedirectToAction("List");
				}
				else
				{
					ModelState.AddModelError("", result.Message ?? string.Empty);
				}
            }
			var model = await PrepareVesselViewModel();
            model.Vessel = v.Vessel;

			return View(model);
		}

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVessel(ulong vesselId)
        {
           await _basicInfoRepository.DeleteVesselById(vesselId);
            return RedirectToAction("List");
        }

        private async Task<VesselViewModel> PrepareVesselViewModel()
        {
            var model = new VesselViewModel();
            model.Countries = await _basicInfoRepository.GetAllCountries() as IList<Country>;
            model.VesselTypes = await _basicInfoRepository.GetAllVesselTypes() as IList<VesselType>;
            return model;
        }

		#endregion

		#region ShippingLineCompany
		[Breadcrumb("ShippingLines", FromAction = "List", FromController = typeof(DashboardController))]
		public async Task<IActionResult> ShippingLineList(int pageNumber = 1, int pageCount = 10, string filter = "")
        {
            var model = await _basicInfoRepository.GetPaginationShippingLineCompaniesAsync(pageNumber, pageCount, filter);
			model.PageInfo.Title = "Shipping Line Companies List";
			model.PageInfo.Filter = filter;
			model.PageInfo.PageName = "ShippingLineList";
            ViewData["ActiveLink"] = "shippingline";
			return View(model);
        }

        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateShippingLine", FromAction = "ShippingLineList", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> CreateShippingLine()
        {
			ViewData["ActiveLink"] = "shippingline";
            ShippingLineViewModel model= new ShippingLineViewModel();
            model.AgentList = await _basicInfoRepository.GetAgentsAsync();
			model.HasPorterage = _appVersionService.Beneficiary == Beneficiary.Negin;

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateShippingLine", FromAction = "ShippingLineList", FromController = typeof(BasicInfoController))]
		public async Task<IActionResult> CreateShippingLine(ShippingLineViewModel input, IFormCollection formCollection)
        {
            if (ModelState.IsValid)
			{
				EditTels(input, formCollection);

				var result = await _basicInfoRepository.CreateShippingLineAsync(input.ShippingLineCompany, input.AgentAssigned);
				if (result.State)
				{
					return RedirectToAction("ShippingLineList");
				}
				else
				{
					ModelState.AddModelError("", result.Message ?? string.Empty);
				}
			}

			ViewData["ActiveLink"] = "shippingline";
            input.AgentList = await _basicInfoRepository.GetAgentsAsync();
            return View(input);
        }

        [Authorize(Roles = "admin")]
        [Breadcrumb("EditShippingLine", FromAction = "ShippingLineList", FromController = typeof(BasicInfoController))]
		public async Task<IActionResult> EditShippingLine(uint id)
        {
			ViewData["ActiveLink"] = "shippingline";
			ShippingLineViewModel model = new ShippingLineViewModel();
			model.ShippingLineCompany = await _basicInfoRepository.GetShippingLineAsync(id);
			model.AgentList = await _basicInfoRepository.GetAgentsAsync();
			model.HasPorterage = _appVersionService.Beneficiary == Beneficiary.Negin;

			return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [Breadcrumb("EditShippingLine", FromAction = "ShippingLineList", FromController = typeof(BasicInfoController))]
		public async Task<IActionResult> EditShippingLine(ShippingLineViewModel input, IFormCollection formCollection)
        {
			if (ModelState.IsValid)
			{
				EditTels(input, formCollection);

				var result = await _basicInfoRepository.EditShippingLine(input.ShippingLineCompany, input.AgentAssigned);
				if (result.State)
				{
					return RedirectToAction("ShippingLineList");
				}
				else
				{
					ModelState.AddModelError("", result.Message ?? string.Empty);
				}
			}

			ViewData["ActiveLink"] = "shippingline";
			input.AgentList = await _basicInfoRepository.GetAgentsAsync();
			return View(input);
        }

        [Authorize(Roles = "admin")]
        public void DeleteShippingLine(uint id)
        {
            _basicInfoRepository.DeleteShippingLine(id);
        }

		private void EditTels(ShippingLineViewModel input, IFormCollection formCollection)
		{
			foreach (var item in formCollection)
			{
				if (item.Key.Contains("tel"))
				{
					input.ShippingLineCompany.Tel += item.Value + ',';
				}
			}
			do
			{
				input.ShippingLineCompany.Tel = input.ShippingLineCompany.Tel?.Substring(0, input.ShippingLineCompany.Tel.Length - 1);
			} while (input.ShippingLineCompany.Tel.EndsWith(','));
		}

        #endregion

        #region Voyage
        [Breadcrumb("Voyages", FromAction = "List", FromController = typeof(DashboardController))]
        public IActionResult VoyageList(int pageNumber = 1, int pageCount = 10, string filter = "")
        {
            var model = _basicInfoRepository.GetPaginationVoyagesAsync(pageNumber, pageCount, filter).Result;
            model.PageInfo.Title = "Voyage List";
            model.PageInfo.Filter = filter;
            ViewData["ActiveLink"] = "voyage";
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateVoyage", FromAction = "VoyageList", FromController = typeof(BasicInfoController))]
        public IActionResult CreateVoyage()
		{
			VoyageViewModel model = PrepareVoyageViewModel();
			return View(model);
		}

		[HttpPost]
        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateVoyage", FromAction = "VoyageList", FromController = typeof(BasicInfoController))]
		public IActionResult CreateVoyage(VoyageViewModel newVoyage)
        {
            if (ModelState.IsValid)
			{
				var result = _basicInfoRepository.CreateVoyageAsync(newVoyage.Voyage).Result;
				if (result.State)
				{
					return RedirectToAction("VoyageList");
				}
				else
				{
					ModelState.AddModelError("", result.Message ?? string.Empty);
				}
			}

			VoyageViewModel model = PrepareVoyageViewModel();
			model.Voyage = newVoyage.Voyage;
			return View(model);
		}


        [Authorize(Roles = "admin")]
        public BLMessage ToggleVoyageStatus(ulong id)
		{
			return _basicInfoRepository.ToggleVoyageStatus(id);
		}

		public JsonResult GetAgentsOfOwner(uint ownerId)
		{
			return Json(_basicInfoRepository.GetAgentsOfOwnerAsync(ownerId).Result);
		}

		private VoyageViewModel PrepareVoyageViewModel()
		{
			ViewData["ActiveLink"] = "voyage";
			VoyageViewModel model = new VoyageViewModel();
			model.VesselList = _basicInfoRepository.GetAllVessels().Result;
			model.OwnerShippinglineList = _basicInfoRepository.GetOwnersAsync().Result;
			return model;
		}

        #endregion

        #region Currencies
        [Breadcrumb("Currencies", FromAction = "List", FromController = typeof(DashboardController))]
        public IActionResult CurrencyList(int pageNumber = 1, int pageCount = 10)
		{
			var model = _basicInfoRepository.GetPaginationCurrenciesAsync(pageNumber, pageCount).Result;
			model.PageInfo.Title = "Currencies";
			model.PageInfo.PageName = "CurrencyList";
            ViewData["ActiveLink"] = "currency";

			return View(model);
		}

        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateCurrencyRate", FromAction = "CurrencyList", FromController = typeof(BasicInfoController))]
        public IActionResult CreateCurrency()
		{
            ViewData["ActiveLink"] = "currency";
            return View();
		}

		[HttpPost]
        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateCurrencyRate", FromAction = "CurrencyList", FromController = typeof(BasicInfoController))]
        public IActionResult CreateCurrency(Currency newCurrency)
		{
			if (ModelState.IsValid)
			{
				var result = _basicInfoRepository.CreateCurrencyAsync(newCurrency).Result;
				if (result.State)
				{
					return RedirectToAction("CurrencyList");
				}
				else
				{
					ModelState.AddModelError("", result.Message ?? string.Empty);
				}

			}
			ViewData["ActiveLink"] = "currency";
            return View();
		}
        #endregion

        #region VesselStoppageTariff
        [Breadcrumb("VesselStoppageTariffs", FromAction = "List", FromController = typeof(DashboardController))]
        public IActionResult VesselStoppageTariffList(int pageNumber = 1, int pageCount = 10, string filter = "")
		{
			var model = _basicInfoRepository.GetPaginationVesselStoppageTariffAsync(pageNumber, pageCount, filter).Result;
            model.PageInfo.Title = "VesselStoppage Tariff List";
            model.PageInfo.Filter = filter;
			model.PageInfo.PageName = "VesselStoppageTariffList";
            ViewData["ActiveLink"] = "vesselstoppagetariff";

            return View(model);
		}

        [Breadcrumb("VesselStoppageTariffDetails", FromAction = "VesselStoppageTariffList", FromController = typeof(BasicInfoController))]
        public IActionResult VesselStoppageTariffDetailList(int id)
		{
			var model = _basicInfoRepository.GetAllVesselStoppageTariffDetailAsync(id).Result;
            ViewData["ActiveLink"] = "vesselstoppagetariff";

            return View(model);
		}

        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateVesselStoppageTariff", FromAction = "VesselStoppageTariffList", FromController = typeof(BasicInfoController))]
		public IActionResult CreateVesselStoppageTariff()
		{
			ViewData["ActiveLink"] = "vesselstoppagetariff";
			return View();
		}

		[Authorize(Roles = "admin")]
		public IActionResult AddVesselStoppageTariff(string description, DateTime effectiveDate)
		{
			if(!string.IsNullOrEmpty(description) && effectiveDate != DateTime.MinValue)
			{
				var result = _basicInfoRepository.CreateVesselStoppageTariffAsync(new VesselStoppageTariff { Description = description, EffectiveDate = effectiveDate}).Result;
				if (!result.State)
				{
					ViewBag.Error = result.Message;
				}
				else
				{
					ViewBag.VesselStoppageTariffId = result.sqlResult;
				}
			}
			var model = new VesselStoppageTariffViewModel()
			{
				VesselTypes = (IList<VesselType>)_basicInfoRepository.GetAllVesselTypes().Result
			};
			return View(model);
		}

		[HttpPost]
        [Authorize(Roles = "admin")]
        public OkObjectResult AddVesselStoppageTariffDetail([FromBody]List<VesselStoppageTariffDetails> vesselStoppageTariffDetails)
		{
			if (vesselStoppageTariffDetails.Any(c=>c.NormalHour > 0 && c.NormalPrice > 0 && c.ExtraPrice > 0))
			{
				var result = _basicInfoRepository.CreateVesselStoppageTariffDetailsAsync(vesselStoppageTariffDetails).Result;
				if (!result.State)
				{
					return Ok(result.Message);
				}
			}
			return Ok(200);
		}
		#endregion

		#region CleaningServiceTariff
		[Breadcrumb("CleaningServiceTariffs", FromAction = "List", FromController = typeof(DashboardController))]
		public IActionResult CleaningServiceTariffList(int pageNumber = 1, int pageCount = 10, string filter = "")
		{
			var model = _basicInfoRepository.GetPaginationCleaningServiceTariffAsync(pageNumber, pageCount, filter).Result;
			model.PageInfo.Title = "CleaningService Tariff List";
			model.PageInfo.Filter = filter;
			model.PageInfo.PageName = "CleaningServiceTariffList";

            ViewData["ActiveLink"] = "cleaningservicetariff";
			return View(model);
		}

		[Breadcrumb("CleaningServiceTariffDetails", FromAction = "CleaningServiceTariffList", FromController = typeof(BasicInfoController))]
		public IActionResult CleaningServiceTariffDetailList(int id)
		{
			var model = _basicInfoRepository.GetAllCleaningServiceTariffDetailAsync(id).Result;
			ViewData["ActiveLink"] = "cleaningservicetariff";

			return View(model);
		}

		[Authorize(Roles = "admin")]
		[Breadcrumb("CreateCleaningServiceTariff", FromAction = "CleaningServiceTariffList", FromController = typeof(BasicInfoController))]
		public IActionResult CreateCleaningServiceTariff()
		{
			ViewData["ActiveLink"] = "cleaningservicetariff";
			return View();
		}

		[Authorize(Roles = "admin")]
		public IActionResult AddCleaningServiceTariff(string description, DateTime effectiveDate)
		{
			if (!string.IsNullOrEmpty(description) && effectiveDate != DateTime.MinValue)
			{
				var result = _basicInfoRepository.CreateCleaningServiceTariffAsync(new CleaningServiceTariff { Description = description, EffectiveDate = effectiveDate }).Result;
				if (!result.State)
				{
					ViewBag.Error = result.Message;
				}
				else
				{
					ViewBag.CleaningServiceTariffId = result.sqlResult;
				}
			}

			return View();
		}

		[HttpPost]
		[Authorize(Roles = "admin")]
		public OkObjectResult AddCleaningServiceTariffDetail([FromBody] List<CleaningServiceTariffDetails> cleaningServiceTariffDetails)
		{
			if (cleaningServiceTariffDetails.Any(c => c.GrossWeight > 0 && c.Price > 0))
			{
				var result = _basicInfoRepository.CreateCleaningServiceTariffDetailsAsync(cleaningServiceTariffDetails).Result;
				if (!result.State)
				{
					return Ok(result.Message);
                }
			}

			return Ok(200);
		}

		#endregion

		#region LoadingDischargeTariff
		[Breadcrumb("LoadingDischargeTariffs", FromAction = "List", FromController = typeof(DashboardController))]
		public IActionResult LoadingDischargeTariffList(int pageNumber = 1, int pageCount = 10, string filter = "")
		{
			var model = _basicInfoRepository.GetPaginationLoadingDischargeTariffAsync(pageNumber, pageCount, filter).Result;
			model.PageInfo.Title = "Loading&Discharge Tariff List";
			model.PageInfo.Filter = filter;
			model.PageInfo.PageName = "LoadingDischargeTariffList";

            ViewData["ActiveLink"] = "loadingdischargetariff";
			return View(model);
		}

        [Authorize(Roles = "admin")]
        [Breadcrumb("CreateLoadingDischargeTariff", FromAction = "LoadingDischargeTariffList", FromController = typeof(BasicInfoController))]
        public IActionResult CreateLoadingDischargeTariff()
        {
            ViewData["ActiveLink"] = "loadingdischargetariff";
            return View();
        }

        public IActionResult AddLoadingDischargeTariff()
        {
			List<LoadingDischargeTariffDetails> model = new();
			try
			{
				model = _basicInfoService.GetLastLoadingDischargeTariffDetailAsync().Result;
			}
			catch (Exception ex)
			{
				ViewBag.ErrorMessage = ex.Message;
			}
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public JsonResult AddLoadingDischargeTariff(List<LoadingDischargeTariffDetails> loadingDischargeTariffDetails, string description, DateTime effectiveDate)
        {
			return _basicInfoService.AddLoadingDischargeTariffDetail(loadingDischargeTariffDetails, description, effectiveDate);
        }

		public IActionResult LoadingDischargeTariffDetailList(int id)
		{
			var model = _basicInfoRepository.GetLoadingDischargeTariffByIdAsync(id).Result;
			ViewData["ActiveLink"] = "loadingdischargetariff";
			return View(model);
		}

		#endregion

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}