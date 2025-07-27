using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunNext.Services.Data.Prototypes.SolarAsset;

namespace SunNext.Services.SolarAsset
{
    public interface ISolarAssetService
    {
        Task<AllSolarAssetsFilteredAndPagedPrototype> AllAsync(AllSolarAssetsQueryPrototype queryModel, string userId);

        Task<AllSolarAssetsFilteredAndPagedPrototype> AllWithDeletedAsync(AllSolarAssetsQueryPrototype queryModel);

        Task<SolarAssetPrototype> GetByIdAsync(string id, string userId);

        Task CreateAsync(SolarAssetPrototype input);

        Task<bool> UpdateAsync(string id, SolarAssetPrototype input, string userId);

        Task<bool> DeleteAsync(string id, string userId);

        Task<bool> UnDeleteAsync(string id);
        Task<bool> HardDeleteAsync(string id);
    }
}