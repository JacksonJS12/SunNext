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
    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class SolarAssetController : BaseController
    {
        private readonly ISolarAssetService _solarAssetService;
        private readonly IMapper _mapper;

        public SolarAssetController(ISolarAssetService solarAssetService, IMapper mapper)
        {
            this._solarAssetService = solarAssetService;
            this._mapper = mapper;
        }

        [AllowAnonymous]
        public async Task<IActionResult> All(string? search, DateTime? fromDate, DateTime? toDate, int page = 1, int pageSize = 6)
        {
            var totalCount = await this._solarAssetService.CountFilteredAsync(search, fromDate, toDate);
            var assets = await this._solarAssetService.GetFilteredAsync(search, fromDate, toDate, page, pageSize);

            var model = this._mapper.Map<IEnumerable<SolarAssetViewModel>>(assets);

            return View(model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            var asset = await this._solarAssetService.GetByIdAsync(id);
            if (asset == null)
                return NotFound();

            var model = this._mapper.Map<SolarAssetViewModel>(asset);
            return View(model);
        }

        public IActionResult Create()
        {
            var model = new SolarAssetFormModel()
            {
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

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id)
        {
            var asset = await this._solarAssetService.GetByIdAsync(id);
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

            await this._solarAssetService.UpdateAsync(id, prototype);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await this._solarAssetService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
