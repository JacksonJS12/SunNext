using System;
using System.Collections.Generic;

namespace SunNext.Web.ViewModels.Market;

public class TradingTargetsViewModel
{
    public List<MarketPriceViewModel> MarketPrices { get; set; } = new List<MarketPriceViewModel>();
    public DateTime? FilterDate { get; set; }
    public TradePositionInputModel TradePositionInput { get; set; } = new TradePositionInputModel();

}