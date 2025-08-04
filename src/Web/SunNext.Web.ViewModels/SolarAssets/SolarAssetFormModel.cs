using System;
using System.ComponentModel.DataAnnotations;

namespace SunNext.Web.ViewModels.SolarAssets;

public class SolarAssetFormModel
{
    [Required]
    [Display(Name = "Asset Name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [Display(Name = "System Type")]
    [StringLength(50)]
    public string Type { get; set; } = null!;

    [Required]
    public string OwnerId { get; set; } = null!;

    [Range(0, double.MaxValue)]
    [Display(Name = "Power (kW)")]
    public double PowerKw { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Capacity (kWp)")]
    public double CapacityKw { get; set; }

    [Range(0, 100)]
    [Display(Name = "Efficiency (%)")]
    public double EfficiencyPercent { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Today's Energy (kWh)")]
    public double EnergyTodayKWh { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Monthly Energy (kWh)")]
    public double EnergyMonthKWh { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Yearly Energy (kWh)")]
    public double EnergyYearKWh { get; set; }

    [Range(0, double.MaxValue)]
    [Display(Name = "Total Energy (kWh)")]
    public double EnergyTotalKWh { get; set; }

    public bool IsOnline { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Commissioning Date")]
    public DateTime CommissioningDate { get; set; }

    [Display(Name = "Installer Name")]
    [StringLength(100)]
    public string? InstallerName { get; set; }

    [EmailAddress]
    [Display(Name = "Installer Email")]
    public string? InstallerEmail { get; set; }

    [Phone]
    [Display(Name = "Installer Phone")]
    public string? InstallerPhone { get; set; }

    [Display(Name = "Time Zone")]
    public string? TimeZone { get; set; }

    [Display(Name = "Installation Address")]
    [StringLength(200)]
    public string? Address { get; set; }

    [Url]
    [Display(Name = "Image URL")]
    public string? ImageUrl { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime LastUpdated { get; set; }
}
