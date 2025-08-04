using System;
using System.ComponentModel.DataAnnotations;
using SunNext.Common;
using SunNext.Data.Common.Models;

namespace SunNext.Services.Data.Prototypes.SolarAsset;

public class SolarAssetPrototype : BaseDeletableModel<string>
{
    [Required] public string Id { get; set; } = null!;

    [Required]
    [Display(Name = "Asset Name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Required]
    [Display(Name = "System Type")]
    [StringLength(50)]
    public string Type { get; set; } = null!;

    [Required] public string OwnerId { get; set; } = null!;

    [Range(0, double.MaxValue)] public double PowerKw { get; set; }

    [Range(0, double.MaxValue)] public double CapacityKw { get; set; }

    [Range(0, 100)] public double EfficiencyPercent { get; set; }

    [Range(0, double.MaxValue)] public double EnergyTodayKWh { get; set; }

    [Range(0, double.MaxValue)] public double EnergyMonthKWh { get; set; }

    [Range(0, double.MaxValue)] public double EnergyYearKWh { get; set; }

    [Range(0, double.MaxValue)] public double EnergyTotalKWh { get; set; }

    public bool IsOnline { get; set; }

    [DataType(DataType.Date)] public DateTime CommissioningDate { get; set; }

    [StringLength(100)] public string? InstallerName { get; set; }

    [EmailAddress] public string? InstallerEmail { get; set; }

    [Phone] public string? InstallerPhone { get; set; }

    public string? TimeZone { get; set; }

    [StringLength(200)] public string? Address { get; set; }

    [Url] public string? ImageUrl { get; set; }
}