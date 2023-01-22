using Microsoft.AspNetCore.Mvc;
using Negin.Core.Domain.Entities.Basic;
using Negin.Core.Domain.Interfaces;
using Negin.WebUI.Models;
using Negin.WebUI.Models.ViewModels;
using SmartBreadcrumbs.Attributes;
using System.Diagnostics;
using static Negin.Framework.Exceptions.SqlException;
using Microsoft.AspNetCore.Authorization;

namespace Negin.WebUI.Controllers
{
	[Authorize]
	public class BasicInfoController : Controller
    {
        private readonly ILogger<BasicInfoController> _logger;
        private readonly IVesselRepository _vesselRepository;
        private readonly IShippingLineCompanyRepository _shippingLineCompanyRepository;
        private readonly IVoyageRepository _voyageRepository;


        public BasicInfoController(ILogger<BasicInfoController> logger, IVesselRepository vesselRepository, IShippingLineCompanyRepository shippingLineCompanyRepository, IVoyageRepository voyageRepository)
        {
            _logger = logger;
            _vesselRepository = vesselRepository;
            _shippingLineCompanyRepository = shippingLineCompanyRepository;
            _voyageRepository = voyageRepository;
        }

		#region Vessel
		[DefaultBreadcrumb("Home")]
		public async Task<IActionResult> List(int pageNumber = 1, int pageCount = 10, string filter = "")
        {
            var model = await _vesselRepository.GetPaginationVesselsAsync(pageNumber, pageCount, filter);
            model.PageInfo.Title = "Vessel List";
            model.PageInfo.Filter = filter;
            ViewData["ActiveLink"] = "vessel";
            return View(model);
        }

        [Breadcrumb("CreateVessel", FromAction = "List", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> CreateVessel()
        {
			ViewData["ActiveLink"] = "vessel";
			VesselViewModel model = await PrepareVesselViewModel();
            return View(model);
        }

        [HttpPost]
        [Breadcrumb("CreateVessel", FromAction = "List", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> CreateVessel(VesselViewModel newVessel)
        {
            if (ModelState.IsValid)
            {
                var result = await _vesselRepository.CreateVesselAsync(newVessel.Vessel);
                if (result.State == SqlExceptionMessages.Success)
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

        [Breadcrumb("EditVessel", FromAction = "List", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> EditVessel(ulong vesselId)
        {
            ViewData["ActiveLink"] = "vessel";
			var model = await PrepareVesselViewModel();
            model.Vessel = await _vesselRepository.GetVesselById(vesselId);

			return View(model);
        }

        [HttpPost]
        [Breadcrumb("EditVessel", FromAction = "List", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> EditVessel(VesselViewModel v)
        {
			if (ModelState.IsValid)
            {
                var result = await _vesselRepository.UpdateVesselAsync(v.Vessel);
				if (result.State == SqlExceptionMessages.Success)
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
        public async Task<IActionResult> DeleteVessel(ulong vesselId)
        {
           await _vesselRepository.DeleteVesselById(vesselId);
            return RedirectToAction("List");
        }

        private async Task<VesselViewModel> PrepareVesselViewModel()
        {
            var model = new VesselViewModel();
            model.Countries = await _vesselRepository.GetAllCountries() as IList<Country>;
            model.VesselTypes = await _vesselRepository.GetAllVesselTypes() as IList<VesselType>;
            return model;
        }

		#endregion

		#region ShippingLineCompany
		[Breadcrumb("ShippingLines")]
		public async Task<IActionResult> ShippingLineList(int pageNumber = 1, int pageCount = 10, string filter = "")
        {
            var model = await _shippingLineCompanyRepository.GetPaginationShippingLineCompaniesAsync(pageNumber, pageCount, filter);
			model.PageInfo.Title = "Shipping Line Companies List";
			model.PageInfo.Filter = filter;
            ViewData["ActiveLink"] = "shippingline";
			return View(model);
        }

        [Breadcrumb("CreateShippingLine", FromAction = "ShippingLineList", FromController = typeof(BasicInfoController))]
        public async Task<IActionResult> CreateShippingLine()
        {
			ViewData["ActiveLink"] = "shippingline";
            ShippingLineViewModel model= new ShippingLineViewModel();
            model.AgentList = await _shippingLineCompanyRepository.GetAgentsAsync();

			return View(model);
        }

        [HttpPost]
		[Breadcrumb("CreateShippingLine", FromAction = "ShippingLineList", FromController = typeof(BasicInfoController))]
		public async Task<IActionResult> CreateShippingLine(ShippingLineViewModel input, IFormCollection formCollection)
        {
            if (ModelState.IsValid)
			{
				EditTels(input, formCollection);

				var result = await _shippingLineCompanyRepository.CreateShippingLineAsync(input.ShippingLineCompany, input.AgentAssigned);
				if (result.State == SqlExceptionMessages.Success)
				{
					return RedirectToAction("ShippingLineList");
				}
				else
				{
					ModelState.AddModelError("", result.Message ?? string.Empty);
				}
			}

			ViewData["ActiveLink"] = "shippingline";
            input.AgentList = await _shippingLineCompanyRepository.GetAgentsAsync();
            return View(input);
        }

		[Breadcrumb("EditShippingLine", FromAction = "ShippingLineList", FromController = typeof(BasicInfoController))]
		public async Task<IActionResult> EditShippingLine(uint id)
        {
			ViewData["ActiveLink"] = "shippingline";
			ShippingLineViewModel model = new ShippingLineViewModel();
			model.ShippingLineCompany = await _shippingLineCompanyRepository.GetShippingLineAsync(id);
			model.AgentList = await _shippingLineCompanyRepository.GetAgentsAsync();
			return View(model);
        }

        [HttpPost]
		[Breadcrumb("EditShippingLine", FromAction = "ShippingLineList", FromController = typeof(BasicInfoController))]
		public async Task<IActionResult> EditShippingLine(ShippingLineViewModel input, IFormCollection formCollection)
        {
			if (ModelState.IsValid)
			{
				EditTels(input, formCollection);

				var result = await _shippingLineCompanyRepository.EditShippingLine(input.ShippingLineCompany, input.AgentAssigned);
				if (result.State == SqlExceptionMessages.Success)
				{
					return RedirectToAction("ShippingLineList");
				}
				else
				{
					ModelState.AddModelError("", result.Message ?? string.Empty);
				}
			}

			ViewData["ActiveLink"] = "shippingline";
			input.AgentList = await _shippingLineCompanyRepository.GetAgentsAsync();
			return View(input);
        }

        public void DeleteShippingLine(uint id)
        {
            _shippingLineCompanyRepository.DeleteShippingLine(id);
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
        [Breadcrumb("Voyages")]
        public IActionResult VoyageList(int pageNumber = 1, int pageCount = 10, string filter = "")
        {
            var model = _voyageRepository.GetPaginationVoyagesAsync(pageNumber, pageCount, filter).Result;
            model.PageInfo.Title = "Voyage List";
            model.PageInfo.Filter = filter;
            ViewData["ActiveLink"] = "voyage";
            return View(model);
        }

        [Breadcrumb("CreateVoyage", FromAction = "VoyageList", FromController = typeof(BasicInfoController))]
        public IActionResult CreateVoyage()
		{
			VoyageViewModel model = PrepareVoyageViewModel();
			return View(model);
		}

		[HttpPost]
		[Breadcrumb("CreateVoyage", FromAction = "VoyageList", FromController = typeof(BasicInfoController))]
		public IActionResult CreateVoyage(VoyageViewModel newVoyage)
        {
            if (ModelState.IsValid)
			{
				var result = _voyageRepository.CreateVoyageAsync(newVoyage.Voyage).Result;
				if (result.State == SqlExceptionMessages.Success)
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

        [Breadcrumb("EditVoyage", FromAction = "VoyageList", FromController = typeof(BasicInfoController))]
        public IActionResult EditVoyage(ulong id)
		{
            VoyageViewModel model = PrepareVoyageViewModel();
			model.Voyage = _voyageRepository.GetVoyageById(id).Result;
			model.AgentsOfOwnerList = _shippingLineCompanyRepository.GetAgentsOfOwnerAsync(model.Voyage.OwnerId).Result;
            return View(model);
		}

		[HttpPost]
		[Breadcrumb("EditVoyage", FromAction = "VoyageList", FromController = typeof(BasicInfoController))]
		public IActionResult EditVoyage(VoyageViewModel input)
		{
			if (ModelState.IsValid)
			{
				var result = _voyageRepository.UpdateVoyageAsync(input.Voyage).Result;
				if (result.State == SqlExceptionMessages.Success)
				{
					return RedirectToAction("VoyageList");
				}
				else
				{
					ModelState.AddModelError("", result.Message ?? string.Empty);
				}
			}

			VoyageViewModel model = PrepareVoyageViewModel();
			model.Voyage = _voyageRepository.GetVoyageById(input.Voyage.Id).Result;
			model.AgentsOfOwnerList = _shippingLineCompanyRepository.GetAgentsOfOwnerAsync(model.Voyage.OwnerId).Result;
			return View(model);
		}

		public void ToggleVoyageStatus(ulong id)
		{
			_voyageRepository.ToggleVoyageStatus(id);
		}

		public JsonResult GetAgentsOfOwner(uint ownerId)
		{
			return Json(_shippingLineCompanyRepository.GetAgentsOfOwnerAsync(ownerId).Result);
		}

		private VoyageViewModel PrepareVoyageViewModel()
		{
			ViewData["ActiveLink"] = "voyage";
			VoyageViewModel model = new VoyageViewModel();
			model.VesselList = _vesselRepository.GetAllVessels().Result;
			model.OwnerShippinglineList = _shippingLineCompanyRepository.GetOwnersAsync().Result;
			return model;
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