using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SunNext.Common;
using SunNext.Services.Data.Prototypes.Market;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Web.ViewModels.Market;
using SunNext.Services.Market;
using SunNext.Services.Simulation;
using SunNext.Services.SolarAsset;
using SunNext.Services.VirtualWallet;
using SunNext.Web.ViewModels.SolarAssets;
using SunNext.Web.ViewModels.VirtualWalletView;
using static SunNext.Common.GlobalConstants;

namespace SunNext.Web.Controllers;

public class MarketController : BaseController
{
    private readonly IMarketService _marketService;
    private readonly ISolarAssetService _solarAssetService;
    private readonly IMapper _mapper;
    private readonly IVirtualWalletService _walletService;
    private readonly ISolarSimulatorService _simulationDataService;

    public MarketController(
        IMarketService marketService,
        IMapper mapper,
        ISolarAssetService solarAssetService,
        IVirtualWalletService walletService,
        ISolarSimulatorService simulationDataService)
    {
        this._marketService = marketService;
        this._mapper = mapper;
        this._solarAssetService = solarAssetService;
        this._walletService = walletService;
        this._simulationDataService = simulationDataService;
    }

    [HttpGet]
    public async Task<IActionResult> Wallet()
    {
        var userId = GetUserId();
        var today = GlobalConstants.TodayEESTTime.Date;

        // Get wallet
        var wallet = await _walletService.GetWalletByUserIdAsync(userId);

        bool alreadyCharged = wallet?.Transactions.Any(t => t.Timestamp.Date == today) == true;
        
        // 1. Get today's generated energy from simulation
       var todayGeneratedKWh = await _simulationDataService
            .GetTotalEnergyGeneratedByUserAsync(userId, today);

        if (!alreadyCharged)
        {
            // 2. Fill wallet with that amount
            await _walletService.FillWalletForUserAsync(userId, todayGeneratedKWh, "Simulated Generation");

            // 3. Reload wallet to get updated balance
            wallet = await _walletService.GetWalletByUserIdAsync(userId);
        }

        var vm = new WalletViewModel
        {
            UserId = userId,
            TodayGeneratedKWh = todayGeneratedKWh,
            BalanceKWh = wallet?.EnergyBalanceKWh ?? 0,
            LastUpdated = DateTime.UtcNow,
            WasChargedToday = alreadyCharged
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Targets(string filterDate)
    {
        var date = TodayEESTTime;

        if (!string.IsNullOrEmpty(filterDate))
        {
            if (DateTime.TryParseExact(filterDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                date = parsedDate;
            }
            else if (DateTime.TryParse(filterDate, CultureInfo.InvariantCulture,
                         DateTimeStyles.None, out DateTime fallbackDate))
            {
                date = fallbackDate;
            }
        }

        var marketPrototypes = await this._marketService.GetPricesByDateWithFallbackAsync(date);
        var actualDate = marketPrototypes.FirstOrDefault()?.Date ?? date;

        var tradeInput = await this.BuildTradePositionInputModelAsync(actualDate);

        var vm = new TradingTargetsViewModel
        {
            FilterDate = actualDate,
            MarketPrices = _mapper.Map<List<MarketPriceViewModel>>(marketPrototypes),
            TradePositionInput = tradeInput
        };

        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddTradePosition(TradePositionInputModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Targets), new { filterDate = model.TradeDate.ToString("yyyy-MM-dd") });
        }

        var prototype = this._mapper.Map<TradePositionPrototype>(model);
        var userId = GetUserId();

        await this._marketService.AddTradePositionAsync(prototype, userId);

        return RedirectToAction(nameof(Targets), new { filterDate = model.TradeDate.ToString("yyyy-MM-dd") });
    }


    public async Task<TradePositionInputModel> BuildTradePositionInputModelAsync(DateTime date)
    {
        var userId = GetUserId(); 

        var queryModel = new AllSolarAssetsQueryPrototype
        {
            CurrentPage = 1,
            SolarAssetsPerPage = 1000,
            SearchString = null,
            SolarAssetType = null,
        };

        var result = await _solarAssetService.AllAsync(queryModel, userId);

        var assetList = result.SolarAssets
            .Select(a => new SimpleAssetViewModel
            {
                Id = a.Id,
                Name = a.Name
            })
            .ToList();

        return new TradePositionInputModel
        {
            TradeDate = date,
            Assets = assetList
        };
    }
    public IActionResult Index()
    {
        return View();
    }
}