using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using SunNext.Common.Enums;
using SunNext.Data.Common.Repositories;
using SunNext.Services.Data;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Services.SolarAsset;
using Xunit;

namespace SunNext.Services.Data.Tests
{
    public class SolarAssetServiceTests
    {
        private readonly Mock<IDeletableEntityRepository<SunNext.Data.Models.SolarAsset>> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SolarAssetService _service;

        public SolarAssetServiceTests()
        {
            _mockRepository = new Mock<IDeletableEntityRepository<SunNext.Data.Models.SolarAsset>>();
            _mockMapper = new Mock<IMapper>();
            _service = new SolarAssetService(_mockRepository.Object, _mockMapper.Object);
        }

        public class AllSolarAssetsQueryPrototype
        {
            public string SearchString { get; set; }
            public string SolarAssetType { get; set; }
            public DateTime? InstallationDateFrom { get; set; }
            public DateTime? InstallationDateTo { get; set; }
            public SolarAssetSorting SolarAssetSorting { get; set; }
            public int CurrentPage { get; set; } = 1;
            public int SolarAssetsPerPage { get; set; } = 10;
        }

        public class AllSolarAssetsFilteredAndPagedPrototype
        {
            public IEnumerable<SolarAssetListItemPrototype> SolarAssets { get; set; }
            public int TotalSolarAssetsCount { get; set; }
        }

        public class SolarAssetListItemPrototype
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public decimal PowerKw { get; set; }
        }

        public class SolarAssetPrototype
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public decimal PowerKw { get; set; }
            public string OwnerId { get; set; }
        }
    }
}