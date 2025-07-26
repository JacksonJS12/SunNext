using System;
using System.ComponentModel.DataAnnotations;
using SunNext.Common;
using SunNext.Data.Common.Models;

namespace SunNext.Data.Models;

public class SolarAsset : BaseDeletableModel<string>
{
    public SolarAsset()
    {
        this.Id = Guid.NewGuid().ToString();
        this.CreatedOn = DateTime.UtcNow;
        this.ModifiedOn = DateTime.UtcNow;
    }

    [Required]
    [MaxLength(EntityValidationConstants.SolarAsset.NameMaxLength)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(EntityValidationConstants.SolarAsset.TypeMaxLength)]
    public string Type { get; set; } = null!;

    [MaxLength(EntityValidationConstants.SolarAsset.LocationMaxLength)]
    public string? Location { get; set; }

    [Range(EntityValidationConstants.SolarAsset.PowerMin, double.MaxValue)]
    public double PowerKw { get; set; }

    [Range(EntityValidationConstants.SolarAsset.CapacityMin, double.MaxValue)]
    public double CapacityKw { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EfficiencyMin, EntityValidationConstants.SolarAsset.EfficiencyMax)]
    public double EfficiencyPercent { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EnergyMin, double.MaxValue)]
    public double EnergyTodayKWh { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EnergyMin, double.MaxValue)]
    public double EnergyMonthKWh { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EnergyMin, double.MaxValue)]
    public double EnergyYearKWh { get; set; }

    [Range(EntityValidationConstants.SolarAsset.EnergyMin, double.MaxValue)]
    public double EnergyTotalKWh { get; set; }

    [MaxLength(EntityValidationConstants.SolarAsset.InstallerNameMaxLength)]
    public string? InstallerName { get; set; }

    [MaxLength(EntityValidationConstants.SolarAsset.InstallerEmailMaxLength)]
    public string? InstallerEmail { get; set; }

    [MaxLength(EntityValidationConstants.SolarAsset.InstallerPhoneMaxLength)]
    public string? InstallerPhone { get; set; }

    [MaxLength(EntityValidationConstants.SolarAsset.TimeZoneMaxLength)]
    public string? TimeZone { get; set; }

    [MaxLength(EntityValidationConstants.SolarAsset.AddressMaxLength)]
    public string? Address { get; set; }

    [MaxLength(EntityValidationConstants.SolarAsset.ImageUrlMaxLength)]
    public string? ImageUrl { get; set; }
}