using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SunNext.Data.Common.Repositories;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Services.SolarAsset;

namespace SunNext.Services.Data
{
    public class SolarAssetService : ISolarAssetService
    {
        private readonly IDeletableEntityRepository<SunNext.Data.Models.SolarAsset> _assetRepository;
        private readonly IMapper _mapper;

        public SolarAssetService(
            IDeletableEntityRepository<SunNext.Data.Models.SolarAsset> assetRepository,
            IMapper mapper)
        {
            this._assetRepository = assetRepository;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<SolarAssetPrototype>> GetAllAsync()
        {
            var assets = await this._assetRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

            return this._mapper.Map<IEnumerable<SolarAssetPrototype>>(assets);
        }

        public async Task<IEnumerable<SolarAssetPrototype>> GetFilteredAsync(string? search, DateTime? from, DateTime? to, int page, int pageSize)
        {
            var query = this._assetRepository.AllAsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search) || x.Location.Contains(search));

            if (from.HasValue)
                query = query.Where(x => x.CreatedOn >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.CreatedOn <= to.Value);

            var result = await query
                .OrderByDescending(x => x.CreatedOn)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return this._mapper.Map<IList<SolarAssetPrototype>>(result);
        }

        public async Task<int> CountFilteredAsync(string? search, DateTime? from, DateTime? to)
        {
            var query = this._assetRepository.AllAsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(x => x.Name.Contains(search) || x.Location.Contains(search));

            if (from.HasValue)
                query = query.Where(x => x.CreatedOn >= from.Value);

            if (to.HasValue)
                query = query.Where(x => x.CreatedOn <= to.Value);

            return await query.CountAsync();
        }

        public async Task<SolarAssetPrototype?> GetByIdAsync(string id)
        {
            var entity = await this._assetRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return this._mapper.Map<SolarAssetPrototype>(entity);
        }

        public async Task CreateAsync(SolarAssetPrototype input)
        {
            var entity = this._mapper.Map<SunNext.Data.Models.SolarAsset>(input);
            entity.Id = Guid.NewGuid().ToString();
            await this._assetRepository.AddAsync(entity);
            await this._assetRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(string id, SolarAssetPrototype input)
        {
            var entity = await this._assetRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            this._mapper.Map(input, entity);
            await this._assetRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await this._assetRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            this._assetRepository.Delete(entity);
            await this._assetRepository.SaveChangesAsync();
            return true;
        }
    }
}
