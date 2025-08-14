using System;
using System.ComponentModel.DataAnnotations;
using SunNext.Common;
using SunNext.Data.Common.Models;

namespace SunNext.Services.Data.Prototypes.SolarSystem;

public class SolarSystemPrototype : BaseDeletableModel<string>
{
    public SolarSystemPrototype()
    {
        this.Id = Guid.NewGuid().ToString();
    }
    [Required]
    public string Id { get; set; } = null!;

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
    public double PowerKw { get; set; }

    [Range(EntityValidationConstants.SolarSystem.CapacityMin, double.MaxValue)]
    public double CapacityKw { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EfficiencyMin, EntityValidationConstants.SolarSystem.EfficiencyMax)]
    public double EfficiencyPercent { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    public double EnergyTodayKWh { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    public double EnergyMonthKWh { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    public double EnergyYearKWh { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    public double EnergyTotalKWh { get; set; }

    public bool IsOnline { get; set; }

    [DataType(DataType.Date)]
    public DateTime CommissioningDate { get; set; }

    [StringLength(EntityValidationConstants.SolarSystem.InstallerNameMaxLength)]
    public string? InstallerName { get; set; }

    [EmailAddress]
    public string? InstallerEmail { get; set; }

    [Phone]
    public string? InstallerPhone { get; set; }

    [StringLength(EntityValidationConstants.SolarSystem.TimeZoneMaxLength)]
    public string? TimeZone { get; set; }

    [StringLength(EntityValidationConstants.SolarSystem.AddressMaxLength)]
    public string? Address { get; set; }

    [Url]
    [StringLength(EntityValidationConstants.SolarSystem.ImageUrlMaxLength)]
    public string? ImageUrl { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    public double DailyEnergyNeedKWh { get; set; }

    [Range(0, 100)]
    public double LocalReservePercent { get; set; }

    public bool CanSellToMarket { get; set; }
    public bool SelfConsumptionOnly { get; set; }
}
