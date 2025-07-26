using System;

namespace SunNext.Web.ViewModels.SolarAssets;

public class SolarAssetViewModel
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string? Location { get; set; }

    public double PowerKw { get; set; }

    public double CapacityKw { get; set; }

    public double EfficiencyPercent { get; set; }

    public double EnergyTodayKWh { get; set; }

    public double EnergyMonthKWh { get; set; }

    public double EnergyYearKWh { get; set; }

    public double EnergyTotalKWh { get; set; }

    public bool IsOnline { get; set; }

    public DateTime CommissioningDate { get; set; }

    public string? InstallerName { get; set; }

    public string? InstallerEmail { get; set; }

    public string? InstallerPhone { get; set; }

    public string? TimeZone { get; set; }

    public string? Address { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime LastUpdated { get; set; }
}