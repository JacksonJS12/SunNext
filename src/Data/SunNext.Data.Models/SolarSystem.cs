using System;
using System.ComponentModel.DataAnnotations;
using SunNext.Common;
using SunNext.Data.Common.Models;

namespace SunNext.Data.Models;

public class SolarSystem : BaseDeletableModel<string>
{
    public SolarSystem()
    {
        this.Id = Guid.NewGuid().ToString();
        this.CreatedOn = DateTime.UtcNow;
        this.ModifiedOn = DateTime.UtcNow;
    }

    [Required]
    [MaxLength(EntityValidationConstants.SolarSystem.NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(EntityValidationConstants.SolarSystem.TypeMaxLength)]
    public string Type { get; set; } = null!;

    public ApplicationUser Owner { get; set; } = null!;
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

    public DateTime CommissioningDate { get; set; }

    [MaxLength(EntityValidationConstants.SolarSystem.InstallerNameMaxLength)]
    public string? InstallerName { get; set; }

    [MaxLength(EntityValidationConstants.SolarSystem.InstallerEmailMaxLength)]
    public string? InstallerEmail { get; set; }

    [MaxLength(EntityValidationConstants.SolarSystem.InstallerPhoneMaxLength)]
    public string? InstallerPhone { get; set; }

    [MaxLength(EntityValidationConstants.SolarSystem.TimeZoneMaxLength)]
    public string? TimeZone { get; set; }

    [MaxLength(EntityValidationConstants.SolarSystem.AddressMaxLength)]
    public string? Address { get; set; }

    [MaxLength(EntityValidationConstants.SolarSystem.ImageUrlMaxLength)]
    public string? ImageUrl { get; set; }

    [Range(EntityValidationConstants.SolarSystem.EnergyMin, double.MaxValue)]
    public double DailyEnergyNeedKWh { get; set; }

    [Range(0, 100)]
    public double LocalReservePercent { get; set; }

    public bool CanSellToMarket { get; set; }
    public bool SelfConsumptionOnly { get; set; }
}
