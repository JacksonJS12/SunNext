using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SunNext.Common;
using SunNext.Data.Common.Repositories;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.Simulation;

namespace SunNext.Services.Simulation;

public class SolarSimulationService : ISolarSimulatorService
{
    private readonly IDeletableEntityRepository<SolarSimulationData> _simulationRepo;
    private readonly IDeletableEntityRepository<SunNext.Data.Models.SolarSystem> _solarSystemRepo;
    private readonly IMapper _mapper;

    public SolarSimulationService(
        IDeletableEntityRepository<SolarSimulationData> simulationRepo,
        IDeletableEntityRepository<SunNext.Data.Models.SolarSystem> solarSystemRepo,
        IMapper mapper)
    {
        _simulationRepo = simulationRepo;
        _solarSystemRepo = solarSystemRepo;
        _mapper = mapper;
    }

    public async Task GenerateForAllSystemsAsync(DateTime? date)
    {
        var simulationDate = date ?? GlobalConstants.TodayEESTTime;

        var systems = await this._solarSystemRepo.All().ToListAsync();

        foreach (var system in systems)
        {
            var hasData = await this._simulationRepo.All()
                .AnyAsync(x => x.SolarSystemId == system.Id && x.Timestamp.Date == simulationDate);

            if (!hasData)
            {
                var simulated = SimulateDaily(system, simulationDate);
                await _simulationRepo.AddRangeAsync(simulated);

                var totalEnergyForDay = simulated.Sum(x => x.EnergyGenerated);

                var lastModified = system.ModifiedOn ?? system.CreatedOn;

                if (lastModified.Month != simulationDate.Month || lastModified.Year != simulationDate.Year)
                {
                    system.EnergyMonthKWh = 0;
                }

                if (lastModified.Year != simulationDate.Year)
                {
                    system.EnergyYearKWh = 0;
                }

                system.EnergyTodayKWh = totalEnergyForDay;
                system.EnergyMonthKWh += totalEnergyForDay;
                system.EnergyYearKWh += totalEnergyForDay;
                system.EnergyTotalKWh += totalEnergyForDay;
                system.ModifiedOn = DateTime.UtcNow;
            }
        }


        await _simulationRepo.SaveChangesAsync();
        await _solarSystemRepo.SaveChangesAsync();
    }

    private List<SolarSimulationData> SimulateDaily(SunNext.Data.Models.SolarSystem system, DateTime day)
    {
        var list = new List<SolarSimulationData>();
        var random = new Random();

        for (int hour = 0; hour < 24; hour++)
        {
            var irradiance = random.Next(200, 950); // Simulated irradiance (W/m²)
            var temperature = random.Next(15, 35); // Simulated temperature (°C)
            var efficiency = system.EfficiencyPercent / 100.0;

            var powerOutput = (system.CapacityKw * ((double)irradiance / 1000)) * efficiency;
            var energyGenerated = powerOutput; // Assuming 1-hour period

            list.Add(new SolarSimulationData
            {
                Id = Guid.NewGuid().ToString(),
                Timestamp = day.AddHours(hour),
                Irradiance = irradiance,
                Temperature = temperature,
                Efficiency = efficiency,
                PowerOutput = powerOutput,
                EnergyGenerated = energyGenerated,
                SolarSystemId = system.Id,
                CreatedOn = DateTime.UtcNow,
            });
        }

        return list;
    }

    public async Task<Dictionary<string, double>> GetTotalEnergyGeneratedPerSystemAsync(DateTime date)
    {
        return await _simulationRepo
            .AllAsNoTracking()
            .Where(x => x.Timestamp.Date == date.Date)
            .GroupBy(x => x.SolarSystemId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Sum(x => x.EnergyGenerated));
    }

    public async Task<double> GetTotalEnergyGeneratedByUserAsync(string userId, DateTime date)
    {
        var systems = await _solarSystemRepo.All()
            .Where(a => a.OwnerId == userId)
            .Select(a => a.Id)
            .ToListAsync();

        var energy = await _simulationRepo.All()
            .Where(s => systems.Contains(s.SolarSystemId) && s.Timestamp.Date == date.Date)
            .SumAsync(s => s.EnergyGenerated);

        return energy;
    }

}