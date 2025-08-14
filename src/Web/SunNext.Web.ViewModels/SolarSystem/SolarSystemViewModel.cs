using System;
using System.ComponentModel.DataAnnotations;
using SunNext.Data.Common.Models;

namespace SunNext.Web.ViewModels.SolarSystem;

public class SolarSystemViewModel : BaseDeletableModel<string>
{
    [Display(Name = "System Name")]
    public string Name { get; set; } = null!;

    [Display(Name = "System Type")]
    public string Type { get; set; } = null!;

    [Display(Name = "Power (kW)")]
    public double PowerKw { get; set; }

    [Display(Name = "Capacity (kWp)")]
    public double CapacityKw { get; set; }

    [Display(Name = "Efficiency (%)")]
    public double EfficiencyPercent { get; set; }

    [Display(Name = "Today's Energy (kWh)")]
    public double EnergyTodayKWh { get; set; }

    [Display(Name = "Monthly Energy (kWh)")]
    public double EnergyMonthKWh { get; set; }

    [Display(Name = "Yearly Energy (kWh)")]
    public double EnergyYearKWh { get; set; }

    [Display(Name = "Total Energy (kWh)")]
    public double EnergyTotalKWh { get; set; }

    [Display(Name = "Is Online")]
    public bool IsOnline { get; set; }

    [Display(Name = "Commissioning Date")]
    [DataType(DataType.Date)]
    public DateTime CommissioningDate { get; set; }

    [Display(Name = "Installer Name")]
    public string? InstallerName { get; set; }

    [Display(Name = "Installer Email")]
    public string? InstallerEmail { get; set; }

    [Display(Name = "Installer Phone")]
    public string? InstallerPhone { get; set; }

    [Display(Name = "Time Zone")]
    public string? TimeZone { get; set; }

    [Display(Name = "Installation Address")]
    public string? Address { get; set; }

    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }
}