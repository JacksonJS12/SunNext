using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SunNext.Services.Data.Prototypes.Market;

namespace SunNext.Services.Market;

public class MarketService : IMarketService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MarketService> _logger;
    private const string address = "https://ibex-scraper-api-e5bwewdyfacgetaf.swedencentral-01.azurewebsites.net";

    public MarketService(HttpClient httpClient, ILogger<MarketService> logger)
    {
        this._httpClient = httpClient;
        this._logger = logger;
    }

    public async Task<List<MarketPricePrototype>> GetPricesByDateWithFallbackAsync(DateTime requestedDate)
    {
        var url = $"{address}/api/Scrape/prices-per-date?date={requestedDate:yyyy/MM/dd}";
        this._logger.LogInformation($"Requesting market data for date: {requestedDate:yyyy/MM/dd} from URL: {url}");

        var response = await this._httpClient.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.NotFound || response.StatusCode == HttpStatusCode.NoContent)
        {
            this._logger.LogWarning(
                $"No data found for {requestedDate:yyyy-MM-dd}, falling back to latest available data");

            var fallbackUrl = $"{address}/api/Scrape/get-all-prices";
            var fallbackResponse = await this._httpClient.GetAsync(fallbackUrl);
            fallbackResponse.EnsureSuccessStatusCode();

            var json = await fallbackResponse.Content.ReadAsStringAsync();
            var allPricesRaw = JsonConvert.DeserializeObject<List<MarketPriceRawPrototype>>(json);

            var allPrices = allPricesRaw.Select(p => new MarketPricePrototype
            {
                Hour = p.Hour,
                PricePerMWh = p.PricePerMWh,
                Date = DateTime.Parse($"{p.Date:yyyy/MM/dd}"),
            }).ToList();

            var latestDate = allPrices.Max(p => p.Date);
            this._logger.LogInformation($"Using fallback data from latest available date: {latestDate:yyyy-MM-dd}");

            return allPrices.Where(p => p.Date == latestDate).ToList();
        }

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var pricesRaw = JsonConvert.DeserializeObject<List<MarketPriceRawPrototype>>(content);

        var prices = pricesRaw.Select(p => new MarketPricePrototype
        {
            Hour = p.Hour,
            PricePerMWh = p.PricePerMWh,
            Date = DateTime.Parse($"{p.Date:yyyy/MM/dd}"),
        }).ToList();
        this._logger.LogInformation(
            $"Successfully retrieved {prices?.Count ?? 0} price entries for {requestedDate:yyyy-MM-dd}");
        return prices;
    }
}