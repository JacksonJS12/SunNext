using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Moq;
using SunNext.Common;
using SunNext.Services.Data.Prototypes.SolarSystem;
using SunNext.Services.Simulation;
using SunNext.Services.SolarSystem;
using SunNext.Web.Controllers;
using SunNext.Web.ViewModels.SolarSystem;
using Xunit;

namespace SunNext.Web.Tests.Controllers
{
    public class SolarSystemControllerTests
    {
        private readonly Mock<ISolarSystemService> _mockSolarSystemService;
        private readonly Mock<ISolarSimulatorService> _mockSolarSystemSimulatorService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<SolarSystemController>> _mockLogger;
        private readonly SolarSystemController _controller;
        private const string TestUserId = "test-user-id";

        public SolarSystemControllerTests()
        {
            _mockSolarSystemService = new Mock<ISolarSystemService>();
            _mockSolarSystemSimulatorService = new Mock<ISolarSimulatorService>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<SolarSystemController>>();

            _controller = new SolarSystemController(
                _mockSolarSystemService.Object,
                _mockSolarSystemSimulatorService.Object,
                _mockMapper.Object,
                _mockLogger.Object);

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
            var queryModel = new AllSolarSystemQueryModel();
            var prototypeQueryModel = new AllSolarSystemsQueryPrototype();
            var prototype = new AllSolarSystemsFilteredAndPagedPrototype
            {
                SolarSystems = new List<SolarSystemListItemPrototype>(),
                TotalSolarSystemsCount = 0
            };
            var viewModels = new List<SolarSystemListItemViewModel>();

            _mockMapper.Setup(m => m.Map<AllSolarSystemsQueryPrototype>(queryModel))
                .Returns(prototypeQueryModel);
            _mockSolarSystemService.Setup(s => s.AllAsync(prototypeQueryModel, TestUserId))
                .ReturnsAsync(prototype);
            _mockMapper.Setup(m => m.Map<IEnumerable<SolarSystemListItemViewModel>>(prototype.SolarSystems))
                .Returns(viewModels);

            // Act
            var result = await _controller.All(queryModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(queryModel, viewResult.Model);
            Assert.Equal(viewModels, queryModel.SolarSystems);
            Assert.Equal(0, queryModel.TotalSolarSystems);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewWithModel()
        {
            // Arrange
            var systemId = "test-system-id";
            var systemPrototype = new SolarSystemPrototype { Id = systemId };
            var viewModel = new SolarSystemViewModel { Id = systemId };

            _mockSolarSystemService.Setup(s => s.GetByIdAsync(systemId, TestUserId))
                .ReturnsAsync(systemPrototype);
            _mockMapper.Setup(m => m.Map<SolarSystemViewModel>(systemPrototype))
                .Returns(viewModel);

            // Act
            var result = await _controller.Details(systemId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var systemId = "invalid-id";
            _mockSolarSystemService.Setup(s => s.GetByIdAsync(systemId, TestUserId))
                .ReturnsAsync((SolarSystemPrototype)null);

            // Act
            var result = await _controller.Details(systemId);

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
            var model = Assert.IsType<SolarSystemFormModel>(viewResult.Model);
            Assert.Equal(TestUserId, model.OwnerId);
            Assert.True(model.CommissioningDate <= DateTime.UtcNow);
        }

        [Fact]
        public async Task Create_Post_WithValidModel_RedirectsToAll()
        {
            // Arrange
            var model = new SolarSystemFormModel
            {
                CapacityKw = 10,
                EfficiencyPercent = 85,
                DailyEnergyNeedKWh = 0 // Will be calculated
            };
            var prototype = new SolarSystemPrototype();

            _mockMapper.Setup(m => m.Map<SolarSystemPrototype>(model))
                .Returns(prototype);
            _mockSolarSystemService.Setup(s => s.CreateAsync(prototype))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SolarSystemController.All), redirectResult.ActionName);
            
            // Verify daily energy was calculated
            var expectedDailyEnergy = Math.Round(10 * 6.2 * (85 / 100.0), 2);
            Assert.Equal(expectedDailyEnergy, model.DailyEnergyNeedKWh);
        }

        [Fact]
        public async Task Create_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var model = new SolarSystemFormModel();
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
            var systemId = "test-system-id";
            var systemPrototype = new SolarSystemPrototype { Id = systemId };
            var formModel = new SolarSystemFormModel();

            _mockSolarSystemService.Setup(s => s.GetByIdAsync(systemId, TestUserId))
                .ReturnsAsync(systemPrototype);
            _mockMapper.Setup(m => m.Map<SolarSystemFormModel>(systemPrototype))
                .Returns(formModel);

            // Act
            var result = await _controller.Edit(systemId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(formModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Post_WithValidModel_RedirectsToDetails()
        {
            // Arrange
            var systemId = "test-system-id";
            var model = new SolarSystemFormModel();
            var prototype = new SolarSystemPrototype { Id = systemId };

            _mockMapper.Setup(m => m.Map<SolarSystemPrototype>(model))
                .Returns(prototype);
            _mockSolarSystemService.Setup(s => s.UpdateAsync(systemId, prototype, TestUserId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Edit(systemId, model);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SolarSystemController.Details), redirectResult.ActionName);
            Assert.Equal(systemId, redirectResult.RouteValues["id"]);
        }

        [Fact]
        public async Task Delete_WithValidId_RedirectsToAll()
        {
            // Arrange
            var systemId = "test-system-id";
            _mockSolarSystemService.Setup(s => s.DeleteAsync(systemId, TestUserId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(systemId);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(SolarSystemController.All), redirectResult.ActionName);
        }

        [Fact]
        public async Task Delete_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var systemId = "invalid-id";
            _mockSolarSystemService.Setup(s => s.DeleteAsync(systemId, TestUserId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(systemId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SimulateAll_GeneratesDataAndRedirects()
        {
            // Arrange
            _mockSolarSystemSimulatorService.Setup(s => s.GenerateForAllSystemsAsync(It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.SimulateAll();

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("AdminOverview", redirectResult.ActionName);
            Assert.Equal("User", redirectResult.ControllerName);
            Assert.Equal("Simulation data generated for all solar systems.", _controller.TempData["Success"]);
        }
    }
}