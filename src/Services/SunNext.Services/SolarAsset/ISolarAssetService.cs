using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunNext.Services.Data.Prototypes.SolarAsset;

namespace SunNext.Services.SolarAsset;

public interface ISolarAssetService
{
    Task<IEnumerable<SolarAssetPrototype>> GetAllAsync();

    Task<IEnumerable<SolarAssetPrototype>> GetFilteredAsync(string? search, DateTime? from, DateTime? to, int page, int pageSize);

    Task<int> CountFilteredAsync(string? search, DateTime? from, DateTime? to);

    Task<SolarAssetPrototype?> GetByIdAsync(string id);

    Task CreateAsync(SolarAssetPrototype asset);

    Task<bool> UpdateAsync(string id, SolarAssetPrototype updated);

    Task<bool> DeleteAsync(string id);
}