using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.Simulation;

namespace SunNext.Services.Simulation;

public interface ISolarSimulatorService
{
    Task GenerateForAllAssetsAsync(DateTime? date);
    Task<Dictionary<string, double>> GetTotalEnergyGeneratedPerAssetAsync(DateTime date);
    Task<double> GetTotalEnergyGeneratedByUserAsync(string userId, DateTime date);
}