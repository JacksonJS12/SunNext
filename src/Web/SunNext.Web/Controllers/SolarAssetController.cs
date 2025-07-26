using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunNext.Common;
using AutoMapper;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Services.SolarAsset;
using SunNext.Web.ViewModels.SolarAssets;

namespace SunNext.Web.Controllers
{
    [Authorize]
    public class SolarAssetController : BaseController
    {
        private readonly ISolarAssetService _solarAssetService;
        private readonly IMapper _mapper;

        public SolarAssetController(ISolarAssetService solarAssetService, IMapper mapper)
        {
            this._solarAssetService = solarAssetService;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> All(string? search, DateTime? fromDate, DateTime? toDate, int page = 1,
            int pageSize = 6)
        {
            string userId = GetUserId();
            var totalCount = await this._solarAssetService.CountFilteredAsync(search, fromDate, toDate, userId);
            var assets =
                await this._solarAssetService.GetFilteredAsync(search, fromDate, toDate, page, pageSize, userId);

            var model = this._mapper.Map<IEnumerable<SolarAssetViewModel>>(assets);

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> AdminAll(string? search, DateTime? fromDate, DateTime? toDate, int page = 1,
            int pageSize = 6)
        {
            var totalCount = await this._solarAssetService.CountFilteredWithDeletedAsync(search, fromDate, toDate);
            var assets =
                await this._solarAssetService.GetFilteredWithDeletedAsync(search, fromDate, toDate, page, pageSize);

            var model = this._mapper.Map<IEnumerable<SolarAssetViewModel>>(assets);

            return View(model);
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
        public async Task<IActionResult> Create(SolarAssetFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var prototype = this._mapper.Map<SolarAssetPrototype>(model);
            await this._solarAssetService.CreateAsync(prototype);

            return RedirectToAction(nameof(All));
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
        public async Task<IActionResult> Edit(string id, SolarAssetFormModel model, SolarAssetPrototype prototype)
        {
            if (!ModelState.IsValid)
                return View(model);

            prototype.Id = id;
            string userId = GetUserId();

            await this._solarAssetService.UpdateAsync(id, prototype, userId);
            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            string userId = GetUserId();

            await this._solarAssetService.DeleteAsync(id, userId);
            return RedirectToAction(nameof(All));
        }
        
        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> UnDelete(string id)
        {
            await this._solarAssetService.UnDeleteAsync(id);
            return RedirectToAction(nameof(AdminAll));
        }
    }
}