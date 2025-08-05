using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using SunNext.Data.Common.Repositories;
using SunNext.Data.Models;
using SunNext.Services.Data;
using SunNext.Services.Data.Blog;

namespace SunNext.Services.Data.Tests
{
    public class BlogPostServiceTests
    {
        private readonly Mock<IDeletableEntityRepository<SunNext.Data.Models.BlogPost>> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BlogPostService _service;

        public BlogPostServiceTests()
        {
            _mockRepository = new Mock<IDeletableEntityRepository<SunNext.Data.Models.BlogPost>>();
            _mockMapper = new Mock<IMapper>();
            _service = new BlogPostService(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateBlogPostWithNewId()
        {
            // Arrange
            var input = new BlogPostPrototype { Title = "Test Post" };
            var entity = new SunNext.Data.Models.BlogPost { Title = "Test Post" };

            _mockMapper.Setup(m => m.Map<SunNext.Data.Models.BlogPost>(input))
                .Returns(entity);

            _mockRepository.Setup(r => r.AddAsync(It.IsAny<SunNext.Data.Models.BlogPost>()))
                .Returns(Task.CompletedTask);

            _mockRepository.Setup(r => r.SaveChangesAsync())
                .Returns(Task.FromResult(1));

            // Act
            await _service.CreateAsync(input);

            // Assert
            Assert.NotNull(entity.Id);
            Assert.NotEqual(Guid.Empty.ToString(), entity.Id);
            _mockMapper.Verify(m => m.Map<SunNext.Data.Models.BlogPost>(input), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<SunNext.Data.Models.BlogPost>()), Times.Once);
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }


    }
}
