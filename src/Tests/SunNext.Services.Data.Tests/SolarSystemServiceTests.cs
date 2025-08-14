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
using SunNext.Services.Data.Prototypes.SolarSystem;
using SunNext.Services.SolarSystem;
using Xunit;

namespace SunNext.Services.Data.Tests
{
    public class SolarSystemServiceTests
    {
        private readonly Mock<IDeletableEntityRepository<SunNext.Data.Models.SolarSystem>> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SolarSystemService _service;

        public SolarSystemServiceTests()
        {
            _mockRepository = new Mock<IDeletableEntityRepository<SunNext.Data.Models.SolarSystem>>();
            _mockMapper = new Mock<IMapper>();
            _service = new SolarSystemService(_mockRepository.Object, _mockMapper.Object);
        }

        public class AllSolarSystemsQueryPrototype
        {
            public string SearchString { get; set; }
            public string SolarSystemType { get; set; }
            public DateTime? InstallationDateFrom { get; set; }
            public DateTime? InstallationDateTo { get; set; }
            public SolarSystemSorting SolarSystemSorting { get; set; }
            public int CurrentPage { get; set; } = 1;
            public int SolarSystemsPerPage { get; set; } = 10;
        }

        public class AllSolarSystemsFilteredAndPagedPrototype
        {
            public IEnumerable<SolarSystemListItemPrototype> SolarSystems { get; set; }
            public int TotalSolarSystemsCount { get; set; }
        }

        public class SolarSystemListItemPrototype
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public decimal PowerKw { get; set; }
        }

        public class SolarSystemPrototype
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public decimal PowerKw { get; set; }
            public string OwnerId { get; set; }
        }
    }
}