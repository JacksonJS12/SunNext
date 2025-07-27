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

            return RedirectToAction(nameof(All));
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
    }
}