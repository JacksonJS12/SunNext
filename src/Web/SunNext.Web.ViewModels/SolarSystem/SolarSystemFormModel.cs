using System;
using System.ComponentModel.DataAnnotations;
using SunNext.Common;
using SunNext.Data.Common.Models;

namespace SunNext.Web.ViewModels.SolarSystem;

public class SolarSystemFormModel : BaseDeletableModel<string>
{
    public SolarSystemFormModel()
    {
        this.Id = Guid.NewGuid().ToString();
    }
    [Required]
    [Display(Name = "System Name")]
    [StringLength(EntityValidationConstants.SolarSystem.NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [Display(Name = "System Type")]
    [StringLength(EntityValidationConstants.SolarSystem.TypeMaxLength)]
    public string Type { get; set; } = null!;

    [Required]
    public string OwnerId { get; set; } = null!;

    [Range(EntityValidationConstants.SolarSystem.PowerMin, double.MaxValue)]
    [Display(Name = "Power (kW)")]
    public double PowerKw { get; set; }

    [Range(EntityValidationConstants.SolarSystem.CapacityMin, double.MaxValue)]
    [Display(Name = "Capacity (kWp)")]
    public double CapacityKw { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EfficiencyMin, EntityValidationConstants.SolarSystem.EfficiencyMax)]
    [Display(Name = "Efficiency (%)")]
    public double EfficiencyPercent { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    [Display(Name = "Today's Energy (kWh)")]
    public double EnergyTodayKWh { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    [Display(Name = "Monthly Energy (kWh)")]
    public double EnergyMonthKWh { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    [Display(Name = "Yearly Energy (kWh)")]
    public double EnergyYearKWh { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    [Display(Name = "Total Energy (kWh)")]
    public double EnergyTotalKWh { get; set; }

    public bool IsOnline { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Commissioning Date")]
    public DateTime CommissioningDate { get; set; }

    [Display(Name = "Installer Name")]
    [StringLength(EntityValidationConstants.SolarSystem.InstallerNameMaxLength)]
    public string? InstallerName { get; set; }

    [EmailAddress]
    [Display(Name = "Installer Email")]
    [StringLength(EntityValidationConstants.SolarSystem.InstallerEmailMaxLength)]
    public string? InstallerEmail { get; set; }

    [Phone]
    [Display(Name = "Installer Phone")]
    [StringLength(EntityValidationConstants.SolarSystem.InstallerPhoneMaxLength)]
    public string? InstallerPhone { get; set; }

    [Display(Name = "Time Zone")]
    [StringLength(EntityValidationConstants.SolarSystem.TimeZoneMaxLength)]
    public string? TimeZone { get; set; }

    [Display(Name = "Installation Address")]
    [StringLength(EntityValidationConstants.SolarSystem.AddressMaxLength)]
    public string? Address { get; set; }

    [Url]
    [Display(Name = "Image URL")]
    [StringLength(EntityValidationConstants.SolarSystem.ImageUrlMaxLength)]
    public string? ImageUrl { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
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
