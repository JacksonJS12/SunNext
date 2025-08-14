using System;
using SunNext.Data.Common.Models;

namespace SunNext.Services.Data.Prototypes.Simulation;

public class SolarSimulationDataPrototype : BaseDeletableModel<string>
{
    public DateTime Timestamp { get; set; }

    public double Irradiance { get; set; } // W/m²

    public double Temperature { get; set; } // °C

    public double Efficiency { get; set; } // %

    public double EnergyGenerated { get; set; } // kWh

    public double PowerOutput { get; set; } // kW

    public string SolarSystemId { get; set; }
}