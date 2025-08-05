using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SunNext.Common;
using SunNext.Web.ViewModels.SolarAssets;

namespace SunNext.Web.ViewModels.Market;
public class TradePositionInputModel
{
    [Required]
    [Display(Name = "Solar Asset")]
    public string SolarAssetId { get; set; } = null!;


    [Required]
    public DateTime TradeDate { get; set; }

    [Range(EntityValidationConstants.MarketTrade.HourMin, EntityValidationConstants.MarketTrade.HourMax)]
    public int StartHour { get; set; }

    [Range(EntityValidationConstants.MarketTrade.HourMin, EntityValidationConstants.MarketTrade.HourMax)]
    public int EndHour { get; set; }

    [Range((double)EntityValidationConstants.DailyPVTradingPosition.EnergyUsedMin, double.MaxValue)]
    public decimal TotalEnergyMWh { get; set; }

    [Range((double)EntityValidationConstants.DailyPVTradingPosition.AvgPriceMin, double.MaxValue)]
    public decimal AverageSellPriceBGN { get; set; }

    [Range((double)EntityValidationConstants.DailyPVTradingPosition.ProfitMin, double.MaxValue)]
    public decimal TotalProfitBGN { get; set; }

    [MaxLength(EntityValidationConstants.MarketTrade.StrategyTagMaxLength)]
    public string? StrategyTag { get; set; }
    public List<SimpleAssetViewModel> Assets { get; set; } = new();

}