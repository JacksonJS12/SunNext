

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunNext.Services.Data.Prototypes.Market;

namespace SunNext.Services.Market;

public interface IMarketService
{
    public Task<List<MarketPricePrototype>> GetPricesByDateWithFallbackAsync(DateTime requestedDate);
}