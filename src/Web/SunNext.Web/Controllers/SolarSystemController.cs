using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunNext.Common;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SunNext.Services.Data.Prototypes.SolarSystem;
using SunNext.Services.Simulation;
using SunNext.Services.SolarSystem;
using SunNext.Web.ViewModels.SolarSystem;

namespace SunNext.Web.Controllers
{
    [Authorize]
    public class SolarSystemController : BaseController
    {
        private readonly ISolarSystemService _solarSystemService;
        private readonly ISolarSimulatorService _solarSimulatorService;
        private readonly IMapper _mapper;
        private readonly ILogger<SolarSystemController> _logger;

        public SolarSystemController(
            ISolarSystemService solarSystemService,
            ISolarSimulatorService solarSimulatorService,
            IMapper mapper,
            ILogger<SolarSystemController> logger)
        {
            this._solarSystemService = solarSystemService;
            this._solarSimulatorService = solarSimulatorService;
            this._mapper = mapper;
            this._logger = logger;
        }

      

        [HttpGet]
        public async Task<IActionResult> All([FromQuery] AllSolarSystemQueryModel queryModel)
        {
            try
            {
                string userId = GetUserId();

                var prototypeQueryModel = _mapper.Map<AllSolarSystemsQueryPrototype>(queryModel);

                AllSolarSystemsFilteredAndPagedPrototype prototype =
                    await this._solarSystemService.AllAsync(prototypeQueryModel, userId);

                queryModel.SolarSystems =
                    this._mapper.Map<IEnumerable<SolarSystemListItemViewModel>>(prototype.SolarSystems);
                queryModel.TotalSolarSystems = prototype.TotalSolarSystemsCount;

                return View(queryModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading solar systems");
                TempData["ToasterError"] = "Failed to load solar systems. Please try again.";
                return View(new AllSolarSystemQueryModel());
            }
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> AdminAll([FromQuery] AllSolarSystemQueryModel queryModel)
        {
            try
            {
                var prototypeQueryModel = _mapper.Map<AllSolarSystemsQueryPrototype>(queryModel);

                AllSolarSystemsFilteredAndPagedPrototype prototype =
                    await this._solarSystemService.AllWithDeletedAsync(prototypeQueryModel);

                queryModel.SolarSystems =
                    this._mapper.Map<IEnumerable<SolarSystemListItemViewModel>>(prototype.SolarSystems);
                queryModel.TotalSolarSystems = prototype.TotalSolarSystemsCount;

                return View(queryModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading admin solar systems view");
                TempData["ToasterError"] = "Failed to load solar systems. Please try again.";
                return View(new AllSolarSystemQueryModel());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                string userId = GetUserId();
                var system = await this._solarSystemService.GetByIdAsync(id, userId);
                if (system == null)
                {
                    TempData["ToasterError"] = "Solar system not found.";
                    return RedirectToAction(nameof(All));
                }

                var model = this._mapper.Map<SolarSystemViewModel>(system);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading solar system details for ID: {id}");
                TempData["ToasterError"] = "Failed to load solar system details. Please try again.";
                return RedirectToAction(nameof(All));
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                string userId = GetUserId();

                var model = new SolarSystemFormModel()
                {
                    OwnerId = userId,
                    CommissioningDate = DateTime.UtcNow
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing create solar system form");
                TempData["ErrorMessage"] = "Failed to initialize the create form. Please try again.";
                return RedirectToAction(nameof(All));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SolarSystemFormModel model)
        {
            try
            {
                // Server-side validation
                if (!ModelState.IsValid)
                {
                    var validationErrors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    TempData["ErrorMessage"] = $"Validation failed: {string.Join("; ", validationErrors)}";
                    return View(model);
                }

                // Additional business logic validation
                var businessValidationErrors = ValidateBusinessRules(model);
                if (businessValidationErrors.Any())
                {
                    foreach (var error in businessValidationErrors)
                    {
                        ModelState.AddModelError("", error);
                    }

                    TempData["ErrorMessage"] = $"Validation failed: {string.Join("; ", businessValidationErrors)}";
                    return View(model);
                }

                // Set timestamps
                model.CreatedOn = DateTime.UtcNow;
                model.ModifiedOn = DateTime.UtcNow;

                // Map to prototype and create
                var prototype = this._mapper.Map<SolarSystemPrototype>(model);
                await this._solarSystemService.CreateAsync(prototype);

                TempData["SuccessMessage"] = $"Solar system '{model.Name}' has been created successfully!";
                return RedirectToAction(nameof(All));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when creating solar system");
                TempData["ErrorMessage"] = "Invalid data provided. Please check your inputs and try again.";
                return View(model);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when creating solar system");
                TempData["ErrorMessage"] = ex.Message;
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when creating solar system");
                TempData["ErrorMessage"] =
                    "An unexpected error occurred while creating the solar system. Please try again later.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                string userId = GetUserId();

                var system = await this._solarSystemService.GetByIdAsync(id, userId);
                if (system == null)
                {
                    TempData["ToasterError"] = "Solar system not found.";
                    return RedirectToAction(nameof(All));
                }

                var model = this._mapper.Map<SolarSystemFormModel>(system);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading solar system for editing. ID: {id}");
                TempData["ToasterError"] = "Failed to load solar system for editing. Please try again.";
                return RedirectToAction(nameof(All));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, SolarSystemFormModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var validationErrors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .SelectMany(x => x.Value.Errors)
                        .Select(x => x.ErrorMessage)
                        .ToList();

                    TempData["ToasterError"] = $"Validation failed: {string.Join("; ", validationErrors)}";
                    return View(model);
                }

                // Additional business logic validation
                var businessValidationErrors = ValidateBusinessRules(model);
                if (businessValidationErrors.Any())
                {
                    foreach (var error in businessValidationErrors)
                    {
                        ModelState.AddModelError("", error);
                    }

                    TempData["ToasterError"] = $"Validation failed: {string.Join("; ", businessValidationErrors)}";
                    return View(model);
                }

                var prototype = this._mapper.Map<SolarSystemPrototype>(model);
                prototype.Id = id;
                string userId = GetUserId();

                bool success = await this._solarSystemService.UpdateAsync(id, prototype, userId);
                if (!success)
                {
                    TempData["ToasterError"] = "Solar system not found or you don't have permission to edit it.";
                    return RedirectToAction(nameof(All));
                }

                TempData["ToasterSuccess"] = $"Solar system '{model.Name}' has been updated successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating solar system. ID: {id}");
                TempData["ToasterError"] = "Failed to update solar system. Please try again.";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                string userId = GetUserId();

                bool success = await this._solarSystemService.DeleteAsync(id, userId);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Solar system not found or you don't have permission to delete it.";
                    return RedirectToAction(nameof(All));
                }

                TempData["SuccessMessage"] = "Solar system has been deleted successfully.";
                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting solar system. ID: {id}");
                TempData["ErrorMessage"] = "Failed to delete solar system. Please try again.";
                return RedirectToAction(nameof(All));
            }
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> UnDelete(string id)
        {
            try
            {
                bool success = await this._solarSystemService.UnDeleteAsync(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Solar system not found or cannot be restored.";
                    return RedirectToAction(nameof(AdminAll));
                }

                TempData["SuccessMessage"] = "Solar system has been restored successfully.";
                return RedirectToAction(nameof(AdminAll));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring solar system. ID: {id}");
                TempData["ErrorMessage"] = "Failed to restore solar system. Please try again.";
                return RedirectToAction(nameof(AdminAll));
            }
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> HardDelete(string id)
        {
            try
            {
                bool success = await this._solarSystemService.HardDeleteAsync(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = "Solar system not found or cannot be permanently deleted.";
                    return RedirectToAction(nameof(AdminAll));
                }

                TempData["WarningMessage"] = "Solar system has been permanently deleted and cannot be recovered.";
                return RedirectToAction(nameof(AdminAll));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error permanently deleting solar system. ID: {id}");
                TempData["ErrorMessage"] = "Failed to permanently delete solar system. Please try again.";
                return RedirectToAction(nameof(AdminAll));
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SimulateAll()
        {
            try
            {
                DateTime date = DateTime.UtcNow.Date;
                await this._solarSimulatorService.GenerateForAllSystemsAsync(date);
                TempData["SuccessMessage"] = "Simulation data generated successfully for all solar systems.";

                return RedirectToAction("AdminOverview", "User");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating simulation data for all systems");
                TempData["ErrorMessage"] = "Failed to generate simulation data. Please try again.";
                return RedirectToAction("AdminOverview", "User");
            }
        }

        private List<string> ValidateBusinessRules(SolarSystemFormModel model)
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