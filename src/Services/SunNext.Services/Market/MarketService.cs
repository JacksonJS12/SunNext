using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SunNext.Data.Common.Repositories;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.Market;
using SunNext.Services.Simulation;

namespace SunNext.Services.Market;

public class MarketService : IMarketService
{
    private readonly IDeletableEntityRepository<MarketTrade> _marketRepository;
    private readonly ISolarSimulatorService _solarSimulatorService;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MarketService> _logger;
    private readonly IMapper _mapper;
    private const string address = "http://ibex-scraper.somee.com";
    const decimal minimumAcceptablePrice = 170; // temporary
    public MarketService(
        HttpClient httpClient,
        ILogger<MarketService> logger,
        IDeletableEntityRepository<MarketTrade> marketRepository,
        ISolarSimulatorService solarSimulatorService,
        IMapper mapper)
    {
        _httpClient = httpClient;
        _logger = logger;
        _marketRepository = marketRepository;
        _solarSimulatorService = solarSimulatorService;
        _mapper = mapper;
    }

    public async Task GenerateAndSaveDailyTradesAsync(DateTime date, string userId)
    {
        var marketPrices = await GetPricesByDateWithFallbackAsync(date);
        var energyBySystem = await _solarSimulatorService.GetTotalEnergyGeneratedPerSystemAsync(date);

        foreach (var (systemId, totalEnergy) in energyBySystem)
        {
            var remainingEnergy = totalEnergy;

            var sortedPrices = marketPrices
                .OrderByDescending(p => p.PricePerMWh)
                .ToList();

            foreach (var price in sortedPrices)
            {
                if (remainingEnergy <= 0 || price.PricePerMWh < minimumAcceptablePrice)
                    break;

                var amountToSell = remainingEnergy;

                remainingEnergy -= amountToSell;

                var trade = new MarketTrade
                {
                    Id = Guid.NewGuid().ToString(),
                    TradeDate = DateOnly.FromDateTime(price.Date),
                    Hour = price.Hour,
                    SolarSystemId = systemId,
                    AmountMWh = (decimal)amountToSell,
                    PricePerMWh = price.PricePerMWh,
                    UserId = userId
                };

                await _marketRepository.AddAsync(trade);
            }
        }

        await _marketRepository.SaveChangesAsync();
    }

    public async Task AddTradePositionAsync(TradePositionPrototype dto, string userId)
    {
        var ibexPrices = await this.GetPricesByDateWithFallbackAsync(dto.TradeDate);

        var selectedHours = Enumerable.Range(dto.StartHour, dto.EndHour - dto.StartHour + 1);
        var matchingPrices = ibexPrices.Where(p => selectedHours.Contains(p.Hour)).ToList();

        if (!matchingPrices.Any())
            throw new InvalidOperationException("No IBEX prices found for the selected range.");

        foreach (var hour in selectedHours)
        {
            var ibexPrice = matchingPrices.First(p => p.Hour == hour);

            var trade = new MarketTrade
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                SolarSystemId = dto.SolarSystemId,
                TradeDate = DateOnly.FromDateTime(dto.TradeDate),
                Hour = hour,
                AmountMWh = 1, // Simulated fixed value or real input
                PricePerMWh = ibexPrice.PricePerMWh
            };

            await _marketRepository.AddAsync(trade);
        }

        await _marketRepository.SaveChangesAsync();
    }


    public async Task<List<MarketPricePrototype>> GetPricesByDateWithFallbackAsync(DateTime requestedDate)
    {
        var url = $"{address}/api/Scrape/prices-per-date?date={requestedDate:yyyy/MM/dd}";
        _logger.LogInformation($"Requesting market data for {requestedDate:yyyy/MM/dd} from: {url}");

        var response = await _httpClient.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.NoContent)
        {
            _logger.LogWarning($"No data for {requestedDate:yyyy-MM-dd}, using fallback data.");

            var fallbackUrl = $"{address}/api/Scrape/get-all-prices";
            var fallbackResponse = await _httpClient.GetAsync(fallbackUrl);
            fallbackResponse.EnsureSuccessStatusCode();

            var json = await fallbackResponse.Content.ReadAsStringAsync();
            var allPricesRaw = JsonConvert.DeserializeObject<List<MarketPriceRawPrototype>>(json);

            var allPrices = allPricesRaw.Select(p => new MarketPricePrototype
            {
                Hour = p.Hour,
                PricePerMWh = p.PricePerMWh,
                Date = DateTime.Parse($"{p.Date:yyyy/MM/dd}")
            }).ToList();

            var latestDate = allPrices.Max(p => p.Date);
            return allPrices.Where(p => p.Date == latestDate).ToList();
        }

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var pricesRaw = JsonConvert.DeserializeObject<List<MarketPriceRawPrototype>>(content);

        return pricesRaw.Select(p => new MarketPricePrototype
        {
            Hour = p.Hour,
            PricePerMWh = p.PricePerMWh,
            Date = DateTime.Parse($"{p.Date:yyyy/MM/dd}")
        }).ToList();
    }

    public async Task<int> CountAsync()
        => await _marketRepository.AllWithDeleted().CountAsync();
}
