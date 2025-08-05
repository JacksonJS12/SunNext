using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SunNext.Data.Common.Models;

namespace SunNext.Data.Models;

public class SolarSimulationData : BaseDeletableModel<string>
{
    public SolarSimulationData()
    {
        this.Id = Guid.NewGuid().ToString();
    }
    public DateTime Timestamp { get; set; } // Could be hourly or daily

    public double Irradiance { get; set; } // W/m²

    public double Temperature { get; set; } // °C

    public double Efficiency { get; set; } // %

    public double EnergyGenerated { get; set; } // kWh

    public double PowerOutput { get; set; } // kW at that timestamp

    [Required]
    public string SolarAssetId { get; set; }

    [ForeignKey(nameof(SolarAssetId))]
    public virtual SolarAsset SolarAsset { get; set; } = null!;
}