using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SunNext.Services.Market;
using SunNext.Services.SolarSystem;
using SunNext.Services.User;
using SunNext.Web.Controllers;
using SunNext.Web.ViewModels.User;
using Xunit;

namespace SunNext.Web.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<ISolarSystemService> _mockSolarSystemService;
        private readonly Mock<IMarketService> _mockMarketService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _mockSolarSystemService = new Mock<ISolarSystemService>();
            _mockMarketService = new Mock<IMarketService>();
            _mockUserService = new Mock<IUserService>();

            _controller = new UserController(
                _mockSolarSystemService.Object,
                _mockMarketService.Object,
                _mockUserService.Object);
        }

        [Fact]
        public void Index_ReturnsView()
        {
            // Act
            var result = _controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task AdminCockpit_ReturnsViewWithDashboardData()
        {
            // Arrange
            var totalUsers = 150;
            var totalSystem = 75;
            var tradeRecords = 250;

            _mockUserService.Setup(s => s.CountAsync())
                .ReturnsAsync(totalUsers);
            _mockSolarSystemService.Setup(s => s.CountAsync())
                .ReturnsAsync(totalSystem);
            _mockMarketService.Setup(s => s.CountAsync())
                .ReturnsAsync(tradeRecords);

            // Act
            var result = await _controller.AdminCockpit();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Admin/Cockpit", viewResult.ViewName);
            
            var model = Assert.IsType<AdminDashboardViewModel>(viewResult.Model);
            Assert.Equal(totalUsers, model.TotalUsers);
            Assert.Equal(totalSystem, model.TotalSystems);
            Assert.Equal(tradeRecords, model.TradeRecords);
        }

        [Fact]
        public void AdminUsers_ReturnsCorrectView()
        {
            // Act
            var result = _controller.AdminUsers();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Admin/Users", viewResult.ViewName);
        }

        [Fact]
        public void Login_Get_ReturnsView()
        {
            // Act
            var result = _controller.Login();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_Post_WithValidCredentials_RedirectsToIndex()
        {
            // Arrange
            var username = "admin";
            var password = "password";

            // Act
            var result = _controller.Login(username, password);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(UserController.Index), redirectResult.ActionName);
        }

        [Fact]
        public void Login_Post_WithInvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var username = "invalid";
            var password = "wrong";

            // Act
            var result = _controller.Login(username, password);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey(""));
        }

        [Fact]
        public void Register_Get_ReturnsView()
        {
            // Act
            var result = _controller.Register();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Register_Post_WithMatchingPasswords_RedirectsToLogin()
        {
            // Arrange
            var username = "newuser";
            var password = "password123";
            var confirmPassword = "password123";

            // Act
            var result = _controller.Register(username, password, confirmPassword);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(nameof(UserController.Login), redirectResult.ActionName);
        }

        [Fact]
        public void Register_Post_WithMismatchedPasswords_ReturnsViewWithError()
        {
            // Arrange
            var username = "newuser";
            var password = "password123";
            var confirmPassword = "differentpassword";

            // Act
            var result = _controller.Register(username, password, confirmPassword);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey(""));
        }

        [Theory]
        [InlineData("admin", "password", true)] // Valid credentials
        [InlineData("admin", "wrong", false)] // Wrong password
        [InlineData("wrong", "password", false)] // Wrong username
        [InlineData("wrong", "wrong", false)] // Both wrong
        [InlineData("", "password", false)] // Empty username
        [InlineData("admin", "", false)] // Empty password
        [InlineData("", "", false)] // Both empty
        public void Login_Post_WithVariousCredentials_ReturnsExpectedResult(string username, string password, bool shouldSucceed)
        {
            // Act
            var result = _controller.Login(username, password);

            // Assert
            if (shouldSucceed)
            {
                var redirectResult = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal(nameof(UserController.Index), redirectResult.ActionName);
            }
            else
            {
                var viewResult = Assert.IsType<ViewResult>(result);
                Assert.False(_controller.ModelState.IsValid);
            }
        }
         
      
    }
}