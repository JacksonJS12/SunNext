using Microsoft.AspNetCore.Mvc;
using SunNext.Web.Controllers;
using Xunit;

namespace SunNext.Web.Tests.Controllers
{
    public class MonitoringControllerTests
    {
        private readonly MonitoringController _controller;

        public MonitoringControllerTests()
        {
            _controller = new MonitoringController();
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