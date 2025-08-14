using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SunNext.Common.Enums;
using SunNext.Data.Common.Repositories;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.SolarSystem;
using SunNext.Services.SolarSystem;

namespace SunNext.Services.Data
{
    public class SolarSystemService : ISolarSystemService
    {
        private readonly IDeletableEntityRepository<SunNext.Data.Models.SolarSystem> _solarSystemRepository;
        private readonly IMapper _mapper;
        private ISolarSystemService _solarSystemServiceImplementation;

        public SolarSystemService(
            IDeletableEntityRepository<SunNext.Data.Models.SolarSystem> solarSystemRepository,
            IMapper mapper)
        {
            this._solarSystemRepository = solarSystemRepository;
            this._mapper = mapper;
        }
        


        public async Task<AllSolarSystemsFilteredAndPagedPrototype> AllAsync(AllSolarSystemsQueryPrototype queryModel, string userId)
        {
            IQueryable<SunNext.Data.Models.SolarSystem> systemsQuery = this._solarSystemRepository
                .AllAsNoTracking()
                .Where(x => x.OwnerId == userId);

            if (!string.IsNullOrWhiteSpace(queryModel.SearchString))
            {
                string wildCard = $"%{queryModel.SearchString.ToLower()}%";
                systemsQuery = systemsQuery
                    .Where(x => EF.Functions.Like(x.Name, wildCard));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SolarSystemType))
            {
                systemsQuery = systemsQuery
                    .Where(x => x.Type == queryModel.SolarSystemType);
            }

            if (queryModel.InstallationDateFrom.HasValue)
                systemsQuery = systemsQuery.Where(x => x.CreatedOn >= queryModel.InstallationDateFrom.Value);

            if (queryModel.InstallationDateTo.HasValue)
                systemsQuery = systemsQuery.Where(x => x.CreatedOn <= queryModel.InstallationDateTo.Value);

            systemsQuery = queryModel.SolarSystemSorting switch
            {
                SolarSystemSorting.Newest => systemsQuery
                    .OrderByDescending(x => x.CreatedOn),
                SolarSystemSorting.Oldest => systemsQuery
                    .OrderBy(x => x.CreatedOn),
                SolarSystemSorting.PowerAscending => systemsQuery
                    .OrderBy(x => x.PowerKw),
                SolarSystemSorting.PowerDescending => systemsQuery
                    .OrderByDescending(x => x.PowerKw),
                SolarSystemSorting.NameAscending => systemsQuery
                    .OrderBy(x => x.Name),
                SolarSystemSorting.NameDescending => systemsQuery
                    .OrderByDescending(x => x.Name),
                _ => systemsQuery
                    .OrderByDescending(x => x.CreatedOn)
            };

            int totalSystems = await systemsQuery.CountAsync();

            var systems = await systemsQuery
                .Skip((queryModel.CurrentPage - 1) * queryModel.SolarSystemsPerPage)
                .Take(queryModel.SolarSystemsPerPage)
                .ToListAsync();

            var prototypes = this._mapper.Map<IEnumerable<SolarSystemListItemPrototype>>(systems);

            return new AllSolarSystemsFilteredAndPagedPrototype()
            {
                TotalSolarSystemsCount = totalSystems,
                SolarSystems = prototypes
            };
        }

        public async Task<AllSolarSystemsFilteredAndPagedPrototype> AllWithDeletedAsync(AllSolarSystemsQueryPrototype queryModel)
        {
            IQueryable<SunNext.Data.Models.SolarSystem> systemsQuery = this._solarSystemRepository
                .AllAsNoTrackingWithDeleted();

            if (!string.IsNullOrWhiteSpace(queryModel.SearchString))
            {
                string wildCard = $"%{queryModel.SearchString.ToLower()}%";
                systemsQuery = systemsQuery
                    .Where(x => EF.Functions.Like(x.Name, wildCard));
            }

            if (!string.IsNullOrWhiteSpace(queryModel.SolarSystemType))
            {
                systemsQuery = systemsQuery
                    .Where(x => x.Type == queryModel.SolarSystemType);
            }

            if (queryModel.InstallationDateFrom.HasValue)
                systemsQuery = systemsQuery.Where(x => x.CreatedOn >= queryModel.InstallationDateFrom.Value);

            if (queryModel.InstallationDateTo.HasValue)
                systemsQuery = systemsQuery.Where(x => x.CreatedOn <= queryModel.InstallationDateTo.Value);

            systemsQuery = queryModel.SolarSystemSorting switch
            {
                SolarSystemSorting.Newest => systemsQuery
                    .OrderByDescending(x => x.CreatedOn),
                SolarSystemSorting.Oldest => systemsQuery
                    .OrderBy(x => x.CreatedOn),
                SolarSystemSorting.PowerAscending => systemsQuery
                    .OrderBy(x => x.PowerKw),
                SolarSystemSorting.PowerDescending => systemsQuery
                    .OrderByDescending(x => x.PowerKw),
                SolarSystemSorting.NameAscending => systemsQuery
                    .OrderBy(x => x.Name),
                SolarSystemSorting.NameDescending => systemsQuery
                    .OrderByDescending(x => x.Name),
                _ => systemsQuery
                    .OrderByDescending(x => x.CreatedOn)
            };

            int totalSystems = await systemsQuery.CountAsync();

            var systems = await systemsQuery
                .Skip((queryModel.CurrentPage - 1) * queryModel.SolarSystemsPerPage)
                .Take(queryModel.SolarSystemsPerPage)
                .ToListAsync();

            var prototypes = this._mapper.Map<IEnumerable<SolarSystemListItemPrototype>>(systems);

            return new AllSolarSystemsFilteredAndPagedPrototype()
            {
                TotalSolarSystemsCount = totalSystems,
                SolarSystems = prototypes
            };
        }

        public async Task<SolarSystemPrototype> GetByIdAsync(string id, string userId)
        {
            var entity = await this._solarSystemRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId);

            return this._mapper.Map<SolarSystemPrototype>(entity);
        }

        public async Task CreateAsync(SolarSystemPrototype input)
        {
            var entity = this._mapper.Map<SunNext.Data.Models.SolarSystem>(input);
            entity.Id = Guid.NewGuid().ToString();
            await this._solarSystemRepository.AddAsync(entity);
            
            await this._solarSystemRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(string id, SolarSystemPrototype input, string userId)
        {
            var entity = await this._solarSystemRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId);

            if (entity == null)
                return false;

            this._mapper.Map(input, entity);
            await this._solarSystemRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id, string userId)
        {
            var entity = await this._solarSystemRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id && x.OwnerId == userId);

            if (entity == null)
                return false;

            this._solarSystemRepository.Delete(entity);
            await this._solarSystemRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnDeleteAsync(string id)
        {
            var entity = await this._solarSystemRepository
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            this._solarSystemRepository.Undelete(entity);
            await this._solarSystemRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HardDeleteAsync(string id)
        {
            var entity = await this._solarSystemRepository
                .AllAsNoTrackingWithDeleted()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            this._solarSystemRepository.HardDelete(entity);
            await this._solarSystemRepository.SaveChangesAsync();
            return true;
        }

        public async Task<int> CountAsync()
            => await this._solarSystemRepository.AllWithDeleted().CountAsync();
    }
}