using System;

namespace SunNext.Services.Data.Prototypes.Market;

public class MarketPricePrototype
{
    public DateTime Date { get; set; }
    public int Hour { get; set; }
    public decimal PricePerMWh { get; set; }
}