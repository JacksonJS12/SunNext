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
    private readonly IDeletableEntityRepository<SunNext.Data.Models.SolarAsset> _solarAssetRepo;
    private readonly IMapper _mapper;

    public SolarSimulationService(
        IDeletableEntityRepository<SolarSimulationData> simulationRepo,
        IDeletableEntityRepository<SunNext.Data.Models.SolarAsset> solarAssetRepo,
        IMapper mapper)
    {
        _simulationRepo = simulationRepo;
        _solarAssetRepo = solarAssetRepo;
        _mapper = mapper;
    }

    public async Task GenerateForAllAssetsAsync(DateTime? date)
    {
        var simulationDate = date ?? GlobalConstants.TodayEESTTime;

        var assets = await this._solarAssetRepo.All().ToListAsync();

        foreach (var asset in assets)
        {
            var hasData = await this._simulationRepo.All()
                .AnyAsync(x => x.SolarAssetId == asset.Id && x.Timestamp.Date == simulationDate);

            if (!hasData)
            {
                var simulated = SimulateDaily(asset, simulationDate);
                await _simulationRepo.AddRangeAsync(simulated);

                var totalEnergyForDay = simulated.Sum(x => x.EnergyGenerated);

                var lastModified = asset.ModifiedOn ?? asset.CreatedOn;

                if (lastModified.Month != simulationDate.Month || lastModified.Year != simulationDate.Year)
                {
                    asset.EnergyMonthKWh = 0;
                }

                if (lastModified.Year != simulationDate.Year)
                {
                    asset.EnergyYearKWh = 0;
                }

                asset.EnergyTodayKWh = totalEnergyForDay;
                asset.EnergyMonthKWh += totalEnergyForDay;
                asset.EnergyYearKWh += totalEnergyForDay;
                asset.EnergyTotalKWh += totalEnergyForDay;
                asset.ModifiedOn = DateTime.UtcNow;
            }
        }


        await _simulationRepo.SaveChangesAsync();
        await _solarAssetRepo.SaveChangesAsync();
    }

    private List<SolarSimulationData> SimulateDaily(SunNext.Data.Models.SolarAsset asset, DateTime day)
    {
        var list = new List<SolarSimulationData>();
        var random = new Random();

        for (int hour = 0; hour < 24; hour++)
        {
            var irradiance = random.Next(200, 950); // Simulated irradiance (W/m²)
            var temperature = random.Next(15, 35); // Simulated temperature (°C)
            var efficiency = asset.EfficiencyPercent / 100.0;

            var powerOutput = (asset.CapacityKw * ((double)irradiance / 1000)) * efficiency;
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
                SolarAssetId = asset.Id,
                CreatedOn = DateTime.UtcNow,
            });
        }

        return list;
    }

    public async Task<Dictionary<string, double>> GetTotalEnergyGeneratedPerAssetAsync(DateTime date)
    {
        return await _simulationRepo
            .AllAsNoTracking()
            .Where(x => x.Timestamp.Date == date.Date)
            .GroupBy(x => x.SolarAssetId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Sum(x => x.EnergyGenerated));
    }

    public async Task<double> GetTotalEnergyGeneratedByUserAsync(string userId, DateTime date)
    {
        var assets = await _solarAssetRepo.All()
            .Where(a => a.OwnerId == userId)
            .Select(a => a.Id)
            .ToListAsync();

        var energy = await _simulationRepo.All()
            .Where(s => assets.Contains(s.SolarAssetId) && s.Timestamp.Date == date.Date)
            .SumAsync(s => s.EnergyGenerated);

        return energy;
    }

}