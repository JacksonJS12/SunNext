using System;
using SunNext.Data.Common.Models;

namespace SunNext.Data.Models;

public class Battery : BaseDeletableModel<string>
{
    public Battery()
    {
        this.Id = Guid.NewGuid().ToString();
    }

    // Name or label of the battery
    public string Model { get; set; }

    // Maximum energy capacity of the battery in kWh
    public double CapacityKWh { get; set; }

    // Current stored energy in the battery in kWh
    public double CurrentEnergyKWh { get; set; }

    // Charging power limit in kW
    public double MaxChargeRateKW { get; set; }

    // Discharging power limit in kW
    public double MaxDischargeRateKW { get; set; }

    // Whether the battery is currently charging
    public bool IsCharging { get; set; }

    // Whether the battery is currently discharging
    public bool IsDischarging { get; set; }

    // Charge the battery by a specified amount (kWh)
    public void Charge(double energyKWh)
    {
        if (energyKWh <= 0 || IsDischarging)
            return;

        IsCharging = true;
        CurrentEnergyKWh = Math.Min(CapacityKWh, CurrentEnergyKWh + energyKWh);
        IsCharging = false;
    }

    // Discharge the battery by a specified amount (kWh)
    public void Discharge(double energyKWh)
    {
        if (energyKWh <= 0 || IsCharging)
            return;

        IsDischarging = true;
        CurrentEnergyKWh = Math.Max(0, CurrentEnergyKWh - energyKWh);
        IsDischarging = false;
    }

    // Get the current state of charge in percentage
    public double GetStateOfChargePercent()
    {
        return (CurrentEnergyKWh / CapacityKWh) * 100;
    }
}
