using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SunNext.Web.Controllers;
using Xunit;

namespace SunNext.Web.Tests.Controllers
{
    public class BaseControllerTests
    {
        private readonly TestableBaseController _controller;

        public BaseControllerTests()
        {
            _controller = new TestableBaseController();
        }

        [Fact]
        public void GetUserId_WithAuthenticatedUser_ReturnsUserId()
        {
            // Arrange
            var expectedUserId = "test-user-id-123";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, expectedUserId)
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

            // Act
            var result = _controller.GetUserIdPublic();

            // Assert
            Assert.Equal(expectedUserId, result);
        }

      
        [Fact]
        public void GetUserId_WithEmptyNameIdentifierClaim_ReturnsEmptyString()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, ""), // Empty claim value
                new Claim(ClaimTypes.Name, "testuser")
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

            // Act
            var result = _controller.GetUserIdPublic();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetUserId_WithNullHttpContext_ReturnsEmptyString()
        {
            // Arrange
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = null
            };

            // Act
            var result = _controller.GetUserIdPublic();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void GetUserId_WithMultipleNameIdentifierClaims_ReturnsFirstOne()
        {
            // Arrange
            var expectedUserId = "first-user-id";
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, expectedUserId),
                new Claim(ClaimTypes.NameIdentifier, "second-user-id"), // This should be ignored
                new Claim(ClaimTypes.Name, "testuser")
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

            // Act
            var result = _controller.GetUserIdPublic();

            // Assert
            Assert.Equal(expectedUserId, result);
        }
    }

    // Test helper class to expose protected method
    public class TestableBaseController : BaseController
    {
        public string GetUserIdPublic()
        {
            return GetUserId();
        }
    }
}