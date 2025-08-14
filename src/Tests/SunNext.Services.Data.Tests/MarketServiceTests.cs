using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using SunNext.Data.Common.Repositories;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.Market;
using SunNext.Services.Market;
using SunNext.Services.Simulation;
using Xunit;

namespace SunNext.Services.Market.Tests
{
    public class MarketServiceTests
    {
        private readonly Mock<IDeletableEntityRepository<MarketTrade>> _mockRepository;
        private readonly Mock<ISolarSimulatorService> _mockSolarSimulatorService;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<ILogger<MarketService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;
        private readonly MarketService _service;

        public MarketServiceTests()
        {
            _mockRepository = new Mock<IDeletableEntityRepository<MarketTrade>>();
            _mockSolarSimulatorService = new Mock<ISolarSimulatorService>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            _mockLogger = new Mock<ILogger<MarketService>>();
            _mockMapper = new Mock<IMapper>();

            _service = new MarketService(
                _httpClient,
                _mockLogger.Object,
                _mockRepository.Object,
                _mockSolarSimulatorService.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task GenerateAndSaveDailyTradesAsync_ShouldCreateTradesForEachSystem()
        {
            // Arrange
            var date = DateTime.Today;
            var userId = "test-user-id";
            
            var marketPrices = new List<MarketPricePrototype>
            {
                new MarketPricePrototype { Hour = 1, PricePerMWh = 200, Date = date },
                new MarketPricePrototype { Hour = 2, PricePerMWh = 180, Date = date },
                new MarketPricePrototype { Hour = 3, PricePerMWh = 160, Date = date }
            };

            var energyBySystem = new Dictionary<string, double>
            {
                { "system1", 100.5 },
                { "system2", 50.0 }
            };

            SetupHttpResponse(JsonConvert.SerializeObject(marketPrices.Select(p => new MarketPriceRawPrototype
            {
                Hour = p.Hour,
                PricePerMWh = p.PricePerMWh,
                Date = DateOnly.FromDateTime(p.Date)
            })));

            _mockSolarSimulatorService.Setup(s => s.GetTotalEnergyGeneratedPerSystemAsync(date))
                .ReturnsAsync(energyBySystem);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<MarketTrade>()))
                .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            // Act
            await _service.GenerateAndSaveDailyTradesAsync(date, userId);

            // Assert
            _mockSolarSimulatorService.Verify(s => s.GetTotalEnergyGeneratedPerSystemAsync(date), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MarketTrade>()), Times.AtLeastOnce);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddTradePositionAsync_ShouldCreateTradesForSelectedHours()
        {
            // Arrange
            var dto = new Data.Prototypes.Market.TradePositionPrototype
            {
                TradeDate = DateTime.Today,
                StartHour = 10,
                EndHour = 12,
                SolarSystemId = "system1"
            };
            var userId = "test-user-id";

            var marketPrices = new List<MarketPricePrototype>
            {
                new MarketPricePrototype { Hour = 10, PricePerMWh = 200, Date = dto.TradeDate },
                new MarketPricePrototype { Hour = 11, PricePerMWh = 180, Date = dto.TradeDate },
                new MarketPricePrototype { Hour = 12, PricePerMWh = 160, Date = dto.TradeDate }
            };

            SetupHttpResponse(JsonConvert.SerializeObject(marketPrices.Select(p => new MarketPriceRawPrototype
            {
                Hour = p.Hour,
                PricePerMWh = p.PricePerMWh,
                Date = DateOnly.FromDateTime(p.Date)
            })));

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<MarketTrade>()))
                .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            // Act
            await _service.AddTradePositionAsync(dto, userId);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<MarketTrade>()), Times.Exactly(3));
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddTradePositionAsync_WithNoMatchingPrices_ShouldThrowException()
        {
            // Arrange
            var dto = new Data.Prototypes.Market.TradePositionPrototype
            {
                TradeDate = DateTime.Today,
                StartHour = 10,
                EndHour = 12,
                SolarSystemId = "system1"
            };
            var userId = "test-user-id";

            SetupHttpResponse(JsonConvert.SerializeObject(new List<MarketPriceRawPrototype>()));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AddTradePositionAsync(dto, userId));
        }

        [Fact]
        public async Task GetPricesByDateWithFallbackAsync_WithValidData_ShouldReturnPrices()
        {
            // Arrange
            var requestedDate = DateTime.Today;
            var expectedPrices = new List<MarketPriceRawPrototype>
            {
                new MarketPriceRawPrototype { Hour = 1, PricePerMWh = 200, Date = DateOnly.FromDateTime(requestedDate) },
                new MarketPriceRawPrototype { Hour = 2, PricePerMWh = 180, Date = DateOnly.FromDateTime(requestedDate) }
            };

            SetupHttpResponse(JsonConvert.SerializeObject(expectedPrices));

            // Act
            var result = await _service.GetPricesByDateWithFallbackAsync(requestedDate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Hour);
            Assert.Equal(200, result[0].PricePerMWh);
        }

        [Fact]
        public async Task GetPricesByDateWithFallbackAsync_WithNotFound_ShouldUseFallback()
        {
            // Arrange
            var requestedDate = DateTime.Today;
            var fallbackPrices = new List<MarketPriceRawPrototype>
            {
                new MarketPriceRawPrototype { Hour = 1, PricePerMWh = 150, Date = DateOnly.FromDateTime(requestedDate.AddDays(-1)) },
                new MarketPriceRawPrototype { Hour = 1, PricePerMWh = 160, Date = DateOnly.FromDateTime(requestedDate) }
            };

            // First call returns NotFound
            _mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound))
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(fallbackPrices))
                });

            // Act
            var result = await _service.GetPricesByDateWithFallbackAsync(requestedDate);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Should return only latest date prices
            Assert.Equal(requestedDate, result[0].Date);
        }
        
        private void SetupHttpResponse(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", 
                    ItExpr.IsAny<HttpRequestMessage>(), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(statusCode)
                {
                    Content = new StringContent(content)
                });
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    // Helper classes for testing
    public class MarketPriceRawPrototype
    {
        public int Hour { get; set; }
        public decimal PricePerMWh { get; set; }
        public DateOnly Date { get; set; }
    }

    public class TradePositionPrototype
    {
        public DateTime TradeDate { get; set; }
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public string SolarSystemId { get; set; }
    }
}