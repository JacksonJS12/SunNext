using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SunNext.Common;
using SunNext.Data.Common.Models;

namespace SunNext.Data.Models;

public class TradePosition : BaseDeletableModel<string>
{
    public TradePosition()
    {
        this.Id = Guid.NewGuid().ToString();
        this.CreatedOn = DateTime.UtcNow;
    }

    [Required]
    public string SolarAssetId { get; set; } = null!;

    public SolarAsset SolarAsset { get; set; } = null!;

    [Required]
    public DateOnly TradeDate { get; set; }

    [Range(EntityValidationConstants.MarketTrade.HourMin, EntityValidationConstants.MarketTrade.HourMax)]
    public int StartHour { get; set; }

    [Range(EntityValidationConstants.MarketTrade.HourMin, EntityValidationConstants.MarketTrade.HourMax)]
    public int EndHour { get; set; }

    [MaxLength(EntityValidationConstants.MarketTrade.StrategyTagMaxLength)]
    public string? StrategyTag { get; set; }

    public bool IsClosed { get; set; }

    public ICollection<MarketTrade> Trades { get; set; } = new HashSet<MarketTrade>();

    [NotMapped]
    public decimal TotalEnergyMWh =>
        Trades.Sum(t => t.AmountMWh);

    [NotMapped]
    public decimal AverageSellPriceBGN =>
        Trades.Any() ? Trades.Average(t => t.PricePerMWh) : 0;

    [NotMapped]
    public decimal TotalProfitBGN =>
        Trades.Sum(t => t.AmountMWh * t.PricePerMWh); 
}