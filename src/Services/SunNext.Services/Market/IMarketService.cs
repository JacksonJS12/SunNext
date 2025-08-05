

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunNext.Services.Data.Prototypes.Market;

namespace SunNext.Services.Market;

public interface IMarketService
{
    Task<List<MarketPricePrototype>> GetPricesByDateWithFallbackAsync(DateTime requestedDate);
    Task<int> CountAsync();
    Task GenerateAndSaveDailyTradesAsync(DateTime date, string userId);
    Task AddTradePositionAsync(TradePositionPrototype dto, string userId);
}