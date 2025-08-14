using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunNext.Services.Data.Prototypes.SolarSystem;

namespace SunNext.Services.SolarSystem
{
    public interface ISolarSystemService
    {
        Task<AllSolarSystemsFilteredAndPagedPrototype> AllAsync(AllSolarSystemsQueryPrototype queryModel, string userId);

        Task<AllSolarSystemsFilteredAndPagedPrototype> AllWithDeletedAsync(AllSolarSystemsQueryPrototype queryModel);

        Task<SolarSystemPrototype> GetByIdAsync(string id, string userId);

        Task CreateAsync(SolarSystemPrototype input);

        Task<bool> UpdateAsync(string id, SolarSystemPrototype input, string userId);

        Task<bool> DeleteAsync(string id, string userId);

        Task<bool> UnDeleteAsync(string id);
        Task<bool> HardDeleteAsync(string id);
        Task<int> CountAsync();
    }
}