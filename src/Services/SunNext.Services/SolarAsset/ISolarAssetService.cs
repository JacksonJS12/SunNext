using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunNext.Services.Data.Prototypes.SolarAsset;

namespace SunNext.Services.SolarAsset;

public interface ISolarAssetService
{
    Task<IEnumerable<SolarAssetPrototype>> GetAllWithDeletedAsync();

    Task<IEnumerable<SolarAssetPrototype>> GetFilteredAsync(string search, DateTime? from, DateTime? to, int page,
        int pageSize, string userId);

    Task<int> CountFilteredAsync(string search, DateTime? from, DateTime? to, string userId);

    Task<SolarAssetPrototype> GetByIdAsync(string id, string userId);

    Task CreateAsync(SolarAssetPrototype asset);

    Task<bool> UpdateAsync(string id, SolarAssetPrototype updated, string userId);

    Task<bool> DeleteAsync(string id, string userId);
    Task<IEnumerable<SolarAssetPrototype>> GetFilteredWithDeletedAsync(string search, DateTime? fromDate, DateTime? toDate, int page, int pageSize);
    Task<int> CountFilteredWithDeletedAsync(string search, DateTime? fromDate, DateTime? toDate);
    Task<bool> UnDeleteAsync(string id);
}