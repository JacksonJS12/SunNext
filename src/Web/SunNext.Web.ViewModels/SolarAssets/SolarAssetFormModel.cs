using System;
using System.ComponentModel.DataAnnotations;
using SunNext.Common;

namespace SunNext.Web.ViewModels.SolarAssets;

public class SolarAssetFormModel
{
    [Required]
    [Display(Name = "Asset Name")]
    [StringLength(EntityValidationConstants.SolarAsset.NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [Display(Name = "System Type")]
    [StringLength(EntityValidationConstants.SolarAsset.TypeMaxLength)]
    public string Type { get; set; } = null!;

    [Required]
    public string OwnerId { get; set; } = null!;

    [Range(EntityValidationConstants.SolarAsset.PowerMin, double.MaxValue)]
    [Display(Name = "Power (kW)")]
    public double PowerKw { get; set; }

    [Range(EntityValidationConstants.SolarAsset.CapacityMin, double.MaxValue)]
    [Display(Name = "Capacity (kWp)")]
    public double CapacityKw { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EfficiencyMin, EntityValidationConstants.SolarAsset.EfficiencyMax)]
    [Display(Name = "Efficiency (%)")]
    public double EfficiencyPercent { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EnergyMin, double.MaxValue)]
    [Display(Name = "Today's Energy (kWh)")]
    public double EnergyTodayKWh { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EnergyMin, double.MaxValue)]
    [Display(Name = "Monthly Energy (kWh)")]
    public double EnergyMonthKWh { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EnergyMin, double.MaxValue)]
    [Display(Name = "Yearly Energy (kWh)")]
    public double EnergyYearKWh { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EnergyMin, double.MaxValue)]
    [Display(Name = "Total Energy (kWh)")]
    public double EnergyTotalKWh { get; set; }

    public bool IsOnline { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Commissioning Date")]
    public DateTime CommissioningDate { get; set; }

    [Display(Name = "Installer Name")]
    [StringLength(EntityValidationConstants.SolarAsset.InstallerNameMaxLength)]
    public string? InstallerName { get; set; }

    [EmailAddress]
    [Display(Name = "Installer Email")]
    [StringLength(EntityValidationConstants.SolarAsset.InstallerEmailMaxLength)]
    public string? InstallerEmail { get; set; }

    [Phone]
    [Display(Name = "Installer Phone")]
    [StringLength(EntityValidationConstants.SolarAsset.InstallerPhoneMaxLength)]
    public string? InstallerPhone { get; set; }

    [Display(Name = "Time Zone")]
    [StringLength(EntityValidationConstants.SolarAsset.TimeZoneMaxLength)]
    public string? TimeZone { get; set; }

    [Display(Name = "Installation Address")]
    [StringLength(EntityValidationConstants.SolarAsset.AddressMaxLength)]
    public string? Address { get; set; }

    [Url]
    [Display(Name = "Image URL")]
    [StringLength(EntityValidationConstants.SolarAsset.ImageUrlMaxLength)]
    public string? ImageUrl { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime LastUpdated { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EnergyMin, double.MaxValue)]
    [Display(Name = "Avg. Daily Energy Need (kWh)")]
    public double DailyEnergyNeedKWh { get; set; }

    [Range(0, 100)]
    [Display(Name = "Local Reserve (%)")]
    public double LocalReservePercent { get; set; }

    [Display(Name = "Can Sell to Market")]
    public bool CanSellToMarket { get; set; }

    [Display(Name = "Self-Consumption Only")]
    public bool SelfConsumptionOnly { get; set; }
}
