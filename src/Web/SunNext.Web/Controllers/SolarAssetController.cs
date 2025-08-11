using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunNext.Common;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Services.Simulation;
using SunNext.Services.SolarAsset;
using SunNext.Web.ViewModels.SolarAssets;

namespace SunNext.Web.Controllers
{
    [Authorize]
    public class SolarAssetController : BaseController
    {
        private readonly ISolarAssetService _solarAssetService;
        private readonly ISolarSimulatorService _solarSimulatorService;
        private readonly IMapper _mapper;
        private readonly ILogger<SolarAssetController> _logger;
        private const double avgSunHours = 6.2;

        public SolarAssetController(
            ISolarAssetService solarAssetService,
            ISolarSimulatorService solarSimulatorService,
            IMapper mapper, 
            ILogger<SolarAssetController> logger)
        {
            this._solarAssetService = solarAssetService;
            this._solarSimulatorService = solarSimulatorService;
            this._mapper = mapper;
            this._logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> All([FromQuery] AllSolarAssetsQueryModel queryModel)
        {
            string userId = GetUserId();

            var prototypeQueryModel = _mapper.Map<AllSolarAssetsQueryPrototype>(queryModel);

            AllSolarAssetsFilteredAndPagedPrototype prototype =
                await this._solarAssetService.AllAsync(prototypeQueryModel, userId);

            queryModel.SolarAssets = this._mapper.Map<IEnumerable<SolarAssetListItemViewModel>>(prototype.SolarAssets);
            queryModel.TotalSolarAssets = prototype.TotalSolarAssetsCount;


            return View(queryModel);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> AdminAll([FromQuery] AllSolarAssetsQueryModel queryModel)
        {
            var prototypeQueryModel = _mapper.Map<AllSolarAssetsQueryPrototype>(queryModel);

            AllSolarAssetsFilteredAndPagedPrototype prototype =
                await this._solarAssetService.AllWithDeletedAsync(prototypeQueryModel);

            queryModel.SolarAssets = this._mapper.Map<IEnumerable<SolarAssetListItemViewModel>>(prototype.SolarAssets);
            queryModel.TotalSolarAssets = prototype.TotalSolarAssetsCount;


            return View(queryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            string userId = GetUserId();
            var asset = await this._solarAssetService.GetByIdAsync(id, userId);
            if (asset == null)
                return NotFound();

            var model = this._mapper.Map<SolarAssetViewModel>(asset);
            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            string userId = GetUserId();

            var model = new SolarAssetFormModel()
            {
                OwnerId = userId,
                CommissioningDate = DateTime.UtcNow
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SolarAssetFormModel model)
        {
            // Server-side validation
            if (!ModelState.IsValid)
            {
                Console.WriteLine(ModelState.IsValid.ToString());
                TempData["ErrorMessage"] = "Please fix the validation errors and try again.";
                return View(model);
            }

            try
            {
                // Additional business logic validation
                var validationErrors = ValidateBusinessRules(model);
                if (validationErrors.Any())
                {
                    foreach (var error in validationErrors)
                    {
                        ModelState.AddModelError("", error);
                    }

                    TempData["ErrorMessage"] = "Please fix the validation errors and try again.";
                    return View(model);
                }

                // Set timestamps
                model.CreatedOn = DateTime.UtcNow;
                model.ModifiedOn = DateTime.UtcNow;

                // Auto-calculate daily energy need if not provided or zero
                if (model.DailyEnergyNeedKWh <= 0)
                {
                    const double avgSunHours = SolarAssetController.avgSunHours; // Average sun hours per day
                    model.DailyEnergyNeedKWh = Math.Round(
                        model.CapacityKw * avgSunHours * (model.EfficiencyPercent / 100.0), 2);
                }

                // Map to prototype and create
                var prototype = this._mapper.Map<SolarAssetPrototype>(model);
                await this._solarAssetService.CreateAsync(prototype);

                TempData["SuccessMessage"] = $"Solar asset '{model.Name}' has been created successfully!";
                return RedirectToAction(nameof(All));
            }
            catch (ArgumentException ex)
            {
                TempData["ErrorMessage"] = "Invalid data provided. Please check your inputs.";
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again later.";
                return View(model);
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            string userId = GetUserId();

            var asset = await this._solarAssetService.GetByIdAsync(id, userId);
            if (asset == null)
                return NotFound();

            var model = this._mapper.Map<SolarAssetFormModel>(asset);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, SolarAssetFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var prototype = this._mapper.Map<SolarAssetPrototype>(model);
            prototype.Id = id;
            string userId = GetUserId();

            bool success = await this._solarAssetService.UpdateAsync(id, prototype, userId);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = GetUserId();

            bool success = await this._solarAssetService.DeleteAsync(id, userId);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> UnDelete(string id)
        {
            bool success = await this._solarAssetService.UnDeleteAsync(id);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(AdminAll));
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> HardDelete(string id)
        {
            bool success = await this._solarAssetService.HardDeleteAsync(id);
            if (!success)
                return NotFound();

            return RedirectToAction(nameof(AdminAll));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SimulateAll()
        {
            DateTime date = DateTime.UtcNow.Date;
            await this._solarSimulatorService.GenerateForAllAssetsAsync(date);
            TempData["Success"] = "Simulation data generated for all solar assets.";

            return RedirectToAction("AdminOverview", "User");
        }

        private List<string> ValidateBusinessRules(SolarAssetFormModel model)
        {
            var errors = new List<string>();

            // Validate that PowerKw is not greater than CapacityKw
            if (model.PowerKw > model.CapacityKw)
            {
                errors.Add("Max output power cannot be greater than installed capacity.");
            }

            // Validate commissioning date is not in the future
            if (model.CommissioningDate > DateTime.Today)
            {
                errors.Add("Commissioning date cannot be in the future.");
            }

            // Validate energy values consistency
            if (model.EnergyTodayKWh > model.EnergyMonthKWh)
            {
                errors.Add("Today's energy cannot be greater than this month's energy.");
            }

            if (model.EnergyMonthKWh > model.EnergyYearKWh)
            {
                errors.Add("This month's energy cannot be greater than this year's energy.");
            }

            if (model.EnergyYearKWh > model.EnergyTotalKWh)
            {
                errors.Add("This year's energy cannot be greater than total energy.");
            }

            // Validate conflicting settings
            if (model.CanSellToMarket && model.SelfConsumptionOnly)
            {
                errors.Add("Cannot have both 'Can Sell to Market' and 'Self-Consumption Only' enabled.");
            }

            // Validate efficiency is reasonable
            if (model.EfficiencyPercent < 10)
            {
                errors.Add("System efficiency seems unusually low. Please verify the value.");
            }

            // Validate that commissioning date is not too old (optional business rule)
            if (model.CommissioningDate < DateTime.Today.AddYears(-30))
            {
                errors.Add("Commissioning date seems unusually old. Please verify the date.");
            }

            // Validate time zone format (basic validation)
            if (!string.IsNullOrEmpty(model.TimeZone) && !model.TimeZone.Contains("/"))
            {
                errors.Add("Time zone should be in format 'Region/City' (e.g., 'Europe/Sofia').");
            }

            return errors;
        }
    }
}