using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using SunNext.Web.Controllers;
using SunNext.Web.ViewModels;
using SunNext.Web.ViewModels.Home;
using Xunit;

namespace SunNext.Web.Tests.Controllers
{
    public class HomeControllerTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly HomeController _controller;

        public HomeControllerTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _controller = new HomeController(_mockConfiguration.Object);

            // Setup TempData
            var httpContext = new DefaultHttpContext();
            var tempDataProvider = new Mock<ITempDataProvider>();
            var tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            var tempData = new TempDataDictionary(httpContext, tempDataProvider.Object);
            tempDataDictionaryFactory.Setup(f => f.GetTempData(It.IsAny<HttpContext>())).Returns(tempData);
            _controller.TempData = tempData;

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };
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
        public void Contact_Get_ReturnsViewWithEmptyModel()
        {
            // Act
            var result = _controller.Contact();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ContactFormViewModel>(viewResult.Model);
            Assert.NotNull(model);
        }

        [Fact]
        public async Task Contact_Post_WithInvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new ContactFormViewModel
            {
                Name = "", // Invalid - empty name
                Email = "invalid-email", // Invalid email format
                Message = "Test message"
            };

            _controller.ModelState.AddModelError("Name", "Name is required");
            _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.Contact(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
        }

        [Fact]
        public async Task Contact_Post_WithValidModel_ButEmailFails_ReturnsViewWithError()
        {
            // Arrange
            var model = new ContactFormViewModel
            {
                Name = "John Doe",
                Email = "john@example.com",
                Message = "Test message"
            };

            // Setup configuration to return null/invalid SMTP settings to simulate email failure
            var mockConfigSection = new Mock<IConfigurationSection>();
            mockConfigSection.Setup(x => x["Host"]).Returns((string)null);
            _mockConfiguration.Setup(x => x.GetSection("SmtpSettings")).Returns(mockConfigSection.Object);

            // Act
            var result = await _controller.Contact(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
            Assert.True(_controller.ModelState.ContainsKey(""));
        }

        [Fact]
        public void About_ReturnsView()
        {
            // Act
            var result = _controller.About();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Privacy_ReturnsView()
        {
            // Act
            var result = _controller.Privacy();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Error404_ReturnsCorrectView()
        {
            // Act
            var result = _controller.Error404();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error404", viewResult.ViewName);
        }

        [Fact]
        public void Error500_ReturnsCorrectView()
        {
            // Act
            var result = _controller.Error500();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Error500", viewResult.ViewName);
        }

        [Fact]
        public void Error_ReturnsViewWithErrorModel()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "test-trace-id";
            
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ErrorViewModel>(viewResult.Model);
            Assert.Equal("test-trace-id", model.RequestId);
        }

        [Fact]
        public void TriggerError_ThrowsException()
        {
            // Act & Assert
            var exception = Assert.Throws<Exception>(() => _controller.TriggerError());
            Assert.Equal("Simulated internal server error.", exception.Message);
        }

        [Theory]
        [InlineData("", "test@email.com", "message")] // Empty name
        [InlineData("John", "", "message")] // Empty email
        [InlineData("John", "test@email.com", "")] // Empty message
        [InlineData("John", "invalid-email", "message")] // Invalid email format
        public async Task Contact_Post_WithVariousInvalidInputs_ReturnsViewWithErrors(string name, string email, string message)
        {
            // Arrange
            var model = new ContactFormViewModel
            {
                Name = name,
                Email = email,
                Message = message
            };

            // Simulate model validation errors
            if (string.IsNullOrEmpty(name))
                _controller.ModelState.AddModelError("Name", "Name is required");
            if (string.IsNullOrEmpty(email))
                _controller.ModelState.AddModelError("Email", "Email is required");
            if (string.IsNullOrEmpty(message))
                _controller.ModelState.AddModelError("Message", "Message is required");
            if (!string.IsNullOrEmpty(email) && !email.Contains("@"))
                _controller.ModelState.AddModelError("Email", "Invalid email format");

            // Act
            var result = await _controller.Contact(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
            Assert.False(_controller.ModelState.IsValid);
        }
    }
}