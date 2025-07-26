using System;
using System.ComponentModel.DataAnnotations;

namespace SunNext.Web.ViewModels.SolarAssets
{
    public class SolarAssetFormModel
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required]
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

        [DataType(DataType.Date)]
        public DateTime CommissioningDate { get; set; }

        public string? InstallerName { get; set; }

        [EmailAddress]
        public string? InstallerEmail { get; set; }

        public string? InstallerPhone { get; set; }

        public string? TimeZone { get; set; }

        public string? Address { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}