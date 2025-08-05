using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using SunNext.Common;
using SunNext.Data.Common.Models;

namespace SunNext.Data.Models;

public class MarketTrade : BaseDeletableModel<string>
{
    public MarketTrade()
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
    public int Hour { get; set; }

    [Range((double)EntityValidationConstants.MarketTrade.AmountMin, double.MaxValue)]
    [Precision(18, 4)]
    public decimal AmountMWh { get; set; }

    [Range((double)EntityValidationConstants.MarketTrade.PriceMin, double.MaxValue)]
    [Precision(18, 4)]
    public decimal PricePerMWh { get; set; }

    [Required]
    public string TradePositionId { get; set; } = null!;

    public TradePosition TradePosition { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;
}