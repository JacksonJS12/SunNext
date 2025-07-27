using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SunNext.Common.Enums;
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
        private ISolarAssetService _solarAssetServiceImplementation;

        public SolarAssetService(
            IDeletableEntityRepository<SunNext.Data.Models.SolarAsset> assetRepository,
            IMapper mapper)
        {
            this._assetRepository = assetRepository;
            this._mapper = mapper;
        }
        

        public async Task<AllSolarAssetsFilteredAndPagedPrototype> AllAsync(AllSolarAssetsQueryPrototype queryModel, string userId)
        {
            IQueryable<SunNext.Data.Models.SolarAsset> assetsQuery = this._assetRepository
                .AllAsNoTracking()
                .Where(x => x.OwnerId == userId);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(queryModel.SearchString))
            {
                string wildCard = $"%{queryModel.SearchString.ToLower()}%";
                assetsQuery = assetsQuery
                    .Where(x => EF.Functions.Like(x.Name, wildCard) ||
                               EF.Functions.Like(x.Location, wildCard));
            }

            // Apply solar asset type filter
            if (!string.IsNullOrWhiteSpace(queryModel.SolarAssetType))
            {
                assetsQuery = assetsQuery
                    .Where(x => x.Type == queryModel.SolarAssetType);
            }

            // Apply date filters
            if (queryModel.InstallationDateFrom.HasValue)
                assetsQuery = assetsQuery.Where(x => x.CreatedOn >= queryModel.InstallationDateFrom.Value);

            if (queryModel.InstallationDateTo.HasValue)
                assetsQuery = assetsQuery.Where(x => x.CreatedOn <= queryModel.InstallationDateTo.Value);

            // Apply sorting
            assetsQuery = queryModel.SolarAssetSorting switch
            {
                SolarAssetSorting.Newest => assetsQuery
                    .OrderByDescending(x => x.CreatedOn),
                SolarAssetSorting.Oldest => assetsQuery
                    .OrderBy(x => x.CreatedOn),
                SolarAssetSorting.PowerAscending => assetsQuery
                    .OrderBy(x => x.PowerKw),
                SolarAssetSorting.PowerDescending => assetsQuery
                    .OrderByDescending(x => x.PowerKw),
                SolarAssetSorting.NameAscending => assetsQuery
                    .OrderBy(x => x.Name),
                SolarAssetSorting.NameDescending => assetsQuery
                    .OrderByDescending(x => x.Name),
                _ => assetsQuery
                    .OrderByDescending(x => x.CreatedOn)
            };

            // Get total count before pagination
            int totalAssets = await assetsQuery.CountAsync();

            // Apply pagination
            var assets = await assetsQuery
                .Skip((queryModel.CurrentPage - 1) * queryModel.SolarAssetsPerPage)
                .Take(queryModel.SolarAssetsPerPage)
                .ToListAsync();

            var prototypes = this._mapper.Map<IEnumerable<SolarAssetListItemPrototype>>(assets);

            return new AllSolarAssetsFilteredAndPagedPrototype()
            {
                TotalSolarAssetsCount = totalAssets,
                SolarAssets = prototypes
            };
        }

        public async Task<AllSolarAssetsFilteredAndPagedPrototype> AllWithDeletedAsync(AllSolarAssetsQueryPrototype queryModel)
        {
            IQueryable<SunNext.Data.Models.SolarAsset> assetsQuery = this._assetRepository
                .AllAsNoTrackingWithDeleted();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(queryModel.SearchString))
            {
                string wildCard = $"%{queryModel.SearchString.ToLower()}%";
                assetsQuery = assetsQuery
                    .Where(x => EF.Functions.Like(x.Name, wildCard) ||
                               EF.Functions.Like(x.Location, wildCard));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SolarAssetType))
            {
                assetsQuery = assetsQuery
                    .Where(x => x.Type == queryModel.SolarAssetType);
            }

            if (queryModel.InstallationDateFrom.HasValue)
                assetsQuery = assetsQuery.Where(x => x.CreatedOn >= queryModel.InstallationDateFrom.Value);

            if (queryModel.InstallationDateTo.HasValue)
                assetsQuery = assetsQuery.Where(x => x.CreatedOn <= queryModel.InstallationDateTo.Value);

            assetsQuery = queryModel.SolarAssetSorting switch
            {
                SolarAssetSorting.Newest => assetsQuery
                    .OrderByDescending(x => x.CreatedOn),
                SolarAssetSorting.Oldest => assetsQuery
                    .OrderBy(x => x.CreatedOn),
                SolarAssetSorting.PowerAscending => assetsQuery
                    .OrderBy(x => x.PowerKw),
                SolarAssetSorting.PowerDescending => assetsQuery
                    .OrderByDescending(x => x.PowerKw),
                SolarAssetSorting.NameAscending => assetsQuery
                    .OrderBy(x => x.Name),
                SolarAssetSorting.NameDescending => assetsQuery
                    .OrderByDescending(x => x.Name),
                _ => assetsQuery
                    .OrderByDescending(x => x.CreatedOn)
            };

            int totalAssets = await assetsQuery.CountAsync();

            // Apply pagination
            var assets = await assetsQuery
                .Skip((queryModel.CurrentPage - 1) * queryModel.SolarAssetsPerPage)
                .Take(queryModel.SolarAssetsPerPage)
                .ToListAsync();

            var prototypes = this._mapper.Map<IEnumerable<SolarAssetListItemPrototype>>(assets);

            return new AllSolarAssetsFilteredAndPagedPrototype()
            {
                TotalSolarAssetsCount = totalAssets,
                SolarAssets = prototypes
            };
        }

        public async Task<SolarAssetPrototype> GetByIdAsync(string id, string userId)
        {
            var entity = await this._assetRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId);

            return this._mapper.Map<SolarAssetPrototype>(entity);
        }

        public async Task CreateAsync(SolarAssetPrototype input)
        {
            var entity = this._mapper.Map<SunNext.Data.Models.SolarAsset>(input);
            entity.Id = Guid.NewGuid().ToString();
            await this._assetRepository.AddAsync(entity);
            await this._assetRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(string id, SolarAssetPrototype input, string userId)
        {
            var entity = await this._assetRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId);

            if (entity == null)
                return false;

            this._mapper.Map(input, entity);
            await this._assetRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            var entity = await this._assetRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId);

            if (entity == null)
                return false;

            this._assetRepository.Delete(entity);
            await this._assetRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnDeleteAsync(string id)
        {
            var entity = await this._assetRepository
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            this._assetRepository.Undelete(entity);
            await this._assetRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HardDeleteAsync(string id)
        {
            var entity = await this._assetRepository
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            this._assetRepository.HardDelete(entity);
            await this._assetRepository.SaveChangesAsync();
            return true;
        }
    }
}