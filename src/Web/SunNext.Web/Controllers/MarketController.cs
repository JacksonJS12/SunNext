using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SunNext.Web.ViewModels.Market;
using SunNext.Services.Market;

namespace SunNext.Web.Controllers;

public class MarketController : Controller
{
    private readonly IMarketService _marketService;
    private readonly IMapper _mapper;

    public MarketController(IMarketService marketService, IMapper mapper)
    {
        this._marketService = marketService;
        this._mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Targets(string filterDate)
    {
        var bulgariaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time"); // Bulgaria timezone
        DateTime date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, bulgariaTimeZone).Date;

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

        var vm = new TradingTargetsViewModel
        {
            FilterDate = actualDate,
            MarketPrices = _mapper.Map<List<MarketPriceViewModel>>(marketPrototypes)
        };

        return View(vm);
    }

    public IActionResult Index()
    {
        return View();
    }
}