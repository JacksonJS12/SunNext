using System;

namespace SunNext.Services.Data.Prototypes.Market;

public class TradePositionPrototype
{
    public string SolarAssetId { get; set; } 
    public DateTime TradeDate { get; set; }
    public int StartHour { get; set; }
    public int EndHour { get; set; }
    public string? StrategyTag { get; set; }
}