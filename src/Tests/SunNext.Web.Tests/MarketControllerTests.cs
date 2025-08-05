using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SunNext.Common;
using SunNext.Services.Data.Prototypes.Market;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Services.Market;
using SunNext.Services.Simulation;
using SunNext.Services.SolarAsset;
using SunNext.Services.VirtualWallet;
using SunNext.Web.Controllers;
using SunNext.Web.ViewModels.Market;
using SunNext.Web.ViewModels.SolarAssets;
using SunNext.Web.ViewModels.VirtualWalletView;
using Xunit;

namespace SunNext.Web.Tests.Controllers
{
    public class MarketControllerTests
    {
        private readonly Mock<IMarketService> _mockMarketService;
        private readonly Mock<ISolarAssetService> _mockSolarAssetService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IVirtualWalletService> _mockWalletService;
        private readonly Mock<ISolarSimulatorService> _mockSimulationService;
        private readonly MarketController _controller;
        private const string TestUserId = "test-user-id";

        public MarketControllerTests()
        {
            _mockMarketService = new Mock<IMarketService>();
            _mockSolarAssetService = new Mock<ISolarAssetService>();
            _mockMapper = new Mock<IMapper>();
            _mockWalletService = new Mock<IVirtualWalletService>();
            _mockSimulationService = new Mock<ISolarSimulatorService>();

            _controller = new MarketController(
                _mockMarketService.Object,
                _mockMapper.Object,
                _mockSolarAssetService.Object,
                _mockWalletService.Object,
                _mockSimulationService.Object);

            // Setup user context
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, TestUserId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext()
            {
                User = principal
            };

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
        }


        [Fact]
        public async Task AddTradePosition_WithInvalidModel_RedirectsToTargets()
        {
            // Arrange
            var tradeDate = DateTime.Today;
            var model = new TradePositionInputModel
            {
                TradeDate = tradeDate
            };

            _controller.ModelState.AddModelError("AssetId", "Asset is required");

            // Act
            var result = await _controller.AddTradePosition(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(MarketController.Targets), redirectResult.ActionName);
            Assert.Equal(tradeDate.ToString("yyyy-MM-dd"), redirectResult.RouteValues["filterDate"]);
        }

       

        [Fact]
        public void Index_ReturnsView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }
}