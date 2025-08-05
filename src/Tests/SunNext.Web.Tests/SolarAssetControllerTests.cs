using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using SunNext.Common;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Services.Simulation;
using SunNext.Services.SolarAsset;
using SunNext.Web.Controllers;
using SunNext.Web.ViewModels.SolarAssets;
using Xunit;

namespace SunNext.Web.Tests.Controllers
{
    public class SolarAssetControllerTests
    {
        private readonly Mock<ISolarAssetService> _mockSolarAssetService;
        private readonly Mock<ISolarSimulatorService> _mockSolarSimulatorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly SolarAssetController _controller;
        private const string TestUserId = "test-user-id";

        public SolarAssetControllerTests()
        {
            _mockSolarAssetService = new Mock<ISolarAssetService>();
            _mockSolarSimulatorService = new Mock<ISolarSimulatorService>();
            _mockMapper = new Mock<IMapper>();

            _controller = new SolarAssetController(
                _mockSolarAssetService.Object,
                _mockSolarSimulatorService.Object,
                _mockMapper.Object);

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

            // Setup TempData
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
            tempDataDictionaryFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(tempData);
            _controller.TempData = tempData;
        }

        [Fact]
        public async Task All_ReturnsViewWithQueryModel()
        {
            // Arrange
            var queryModel = new AllSolarAssetsQueryModel();
            var prototypeQueryModel = new AllSolarAssetsQueryPrototype();
            var prototype = new AllSolarAssetsFilteredAndPagedPrototype
            {
                SolarAssets = new List<SolarAssetListItemPrototype>(),
                TotalSolarAssetsCount = 0
            };
            var viewModels = new List<SolarAssetListItemViewModel>();

            _mockMapper.Setup(m => m.Map<AllSolarAssetsQueryPrototype>(queryModel))
                .Returns(prototypeQueryModel);
            _mockSolarAssetService.Setup(s => s.AllAsync(prototypeQueryModel, TestUserId))
                .ReturnsAsync(prototype);
            _mockMapper.Setup(m => m.Map<IEnumerable<SolarAssetListItemViewModel>>(prototype.SolarAssets))
                .Returns(viewModels);

            // Act
            var result = await _controller.All(queryModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(queryModel, viewResult.Model);
            Assert.Equal(viewModels, queryModel.SolarAssets);
            Assert.Equal(0, queryModel.TotalSolarAssets);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewWithModel()
        {
            // Arrange
            var assetId = "test-asset-id";
            var assetPrototype = new SolarAssetPrototype { Id = assetId };
            var viewModel = new SolarAssetViewModel { Id = assetId };

            _mockSolarAssetService.Setup(s => s.GetByIdAsync(assetId, TestUserId))
                .ReturnsAsync(assetPrototype);
            _mockMapper.Setup(m => m.Map<SolarAssetViewModel>(assetPrototype))
                .Returns(viewModel);

            // Act
            var result = await _controller.Details(assetId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var assetId = "invalid-id";
            _mockSolarAssetService.Setup(s => s.GetByIdAsync(assetId, TestUserId))
                .ReturnsAsync((SolarAssetPrototype)null);

            // Act
            var result = await _controller.Details(assetId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Create_Get_ReturnsViewWithModel()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<SolarAssetFormModel>(viewResult.Model);
            Assert.Equal(TestUserId, model.OwnerId);
            Assert.True(model.CommissioningDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task Create_Post_WithValidModel_RedirectsToAll()
        {
            // Arrange
            var model = new SolarAssetFormModel
            {
                CapacityKw = 10,
                EfficiencyPercent = 85,
                DailyEnergyNeedKWh = 0 // Will be calculated
            };
            var prototype = new SolarAssetPrototype();

            _mockMapper.Setup(m => m.Map<SolarAssetPrototype>(model))
                .Returns(prototype);
            _mockSolarAssetService.Setup(s => s.CreateAsync(prototype))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SolarAssetController.All), redirectResult.ActionName);
            
            // Verify daily energy was calculated
            var expectedDailyEnergy = Math.Round(10 * 6.2 * (85 / 100.0), 2);
            Assert.Equal(expectedDailyEnergy, model.DailyEnergyNeedKWh);
        }

        [Fact]
        public async Task Create_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var model = new SolarAssetFormModel();
            _controller.ModelState.AddModelError("Name", "Name is required");

            // Act
            var result = await _controller.Create(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ReturnsViewWithModel()
        {
            // Arrange
            var assetId = "test-asset-id";
            var assetPrototype = new SolarAssetPrototype { Id = assetId };
            var formModel = new SolarAssetFormModel();

            _mockSolarAssetService.Setup(s => s.GetByIdAsync(assetId, TestUserId))
                .ReturnsAsync(assetPrototype);
            _mockMapper.Setup(m => m.Map<SolarAssetFormModel>(assetPrototype))
                .Returns(formModel);

            // Act
            var result = await _controller.Edit(assetId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(formModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Post_WithValidModel_RedirectsToDetails()
        {
            // Arrange
            var assetId = "test-asset-id";
            var model = new SolarAssetFormModel();
            var prototype = new SolarAssetPrototype { Id = assetId };

            _mockMapper.Setup(m => m.Map<SolarAssetPrototype>(model))
                .Returns(prototype);
            _mockSolarAssetService.Setup(s => s.UpdateAsync(assetId, prototype, TestUserId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Edit(assetId, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SolarAssetController.Details), redirectResult.ActionName);
            Assert.Equal(assetId, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Delete_WithValidId_RedirectsToAll()
        {
            // Arrange
            var assetId = "test-asset-id";
            _mockSolarAssetService.Setup(s => s.DeleteAsync(assetId, TestUserId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(assetId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SolarAssetController.All), redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var assetId = "invalid-id";
            _mockSolarAssetService.Setup(s => s.DeleteAsync(assetId, TestUserId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(assetId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SimulateAll_GeneratesDataAndRedirects()
        {
            // Arrange
            _mockSolarSimulatorService.Setup(s => s.GenerateForAllAssetsAsync(It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SimulateAll();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminOverview", redirectResult.ActionName);
            Assert.Equal("User", redirectResult.ControllerName);
            Assert.Equal("Simulation data generated for all solar assets.", _controller.TempData["Success"]);
        }
    }
}