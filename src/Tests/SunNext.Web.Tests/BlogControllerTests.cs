using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SunNext.Services.BlogPost;
using SunNext.Services.Data.Blog;
using SunNext.Web.Controllers;
using SunNext.Web.ViewModels.Blog;
using Xunit;

namespace SunNext.Web.Tests.Controllers
{
    public class BlogControllerTests
    {
        private readonly Mock<IBlogPostService> _mockBlogPostService;
        private readonly Mock<IMapper> _mockMapper;
        private readonly BlogController _controller;

        public BlogControllerTests()
        {
            _mockBlogPostService = new Mock<IBlogPostService>();
            _mockMapper = new Mock<IMapper>();
            _controller = new BlogController(_mockBlogPostService.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task All_WithoutFilters_ReturnsAllPublishedPosts()
        {
            // Arrange
            var blogPosts = new List<BlogPostPrototype>
            {
                new BlogPostPrototype 
                { 
                    Id = "1", 
                    Title = "First Post", 
                    CreatedOn = DateTime.Now.AddDays(-1) 
                },
                new BlogPostPrototype 
                { 
                    Id = "2", 
                    Title = "Second Post", 
                    CreatedOn = DateTime.Now 
                }
            };

            var viewModels = new List<BlogPostViewModel>
            {
                new BlogPostViewModel { Id = "1", Title = "First Post" },
                new BlogPostViewModel { Id = "2", Title = "Second Post" }
            };

            _mockBlogPostService.Setup(s => s.GetAllPublishedAsync())
                .ReturnsAsync(blogPosts);
            _mockMapper.Setup(m => m.Map<IList<BlogPostViewModel>>(It.IsAny<IEnumerable<BlogPostPrototype>>()))
                .Returns(viewModels);

            // Act
            var result = await _controller.All(null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<BlogPostViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public async Task All_WithSearchFilter_ReturnsFilteredPosts()
        {
            // Arrange
            var searchTerm = "First";
            var blogPosts = new List<BlogPostPrototype>
            {
                new BlogPostPrototype 
                { 
                    Id = "1", 
                    Title = "First Post", 
                    CreatedOn = DateTime.Now.AddDays(-1) 
                },
                new BlogPostPrototype 
                { 
                    Id = "2", 
                    Title = "Second Post", 
                    CreatedOn = DateTime.Now 
                }
            };

            var filteredViewModels = new List<BlogPostViewModel>
            {
                new BlogPostViewModel { Id = "1", Title = "First Post" }
            };

            _mockBlogPostService.Setup(s => s.GetAllPublishedAsync())
                .ReturnsAsync(blogPosts);
            _mockMapper.Setup(m => m.Map<IList<BlogPostViewModel>>(It.Is<IEnumerable<BlogPostPrototype>>(
                posts => posts.Count() == 1 && posts.First().Title == "First Post")))
                .Returns(filteredViewModels);

            // Act
            var result = await _controller.All(searchTerm, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<BlogPostViewModel>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("First Post", model.First().Title);
        }

        [Fact]
        public async Task All_WithDateFilter_ReturnsFilteredPosts()
        {
            // Arrange
            var filterDate = DateTime.Today;
            var blogPosts = new List<BlogPostPrototype>
            {
                new BlogPostPrototype 
                { 
                    Id = "1", 
                    Title = "Old Post", 
                    CreatedOn = DateTime.Today.AddDays(-2) 
                },
                new BlogPostPrototype 
                { 
                    Id = "2", 
                    Title = "Recent Post", 
                    CreatedOn = DateTime.Today 
                }
            };

            var filteredViewModels = new List<BlogPostViewModel>
            {
                new BlogPostViewModel { Id = "2", Title = "Recent Post" }
            };

            _mockBlogPostService.Setup(s => s.GetAllPublishedAsync())
                .ReturnsAsync(blogPosts);
            _mockMapper.Setup(m => m.Map<IList<BlogPostViewModel>>(It.Is<IEnumerable<BlogPostPrototype>>(
                posts => posts.Count() == 1 && posts.First().Title == "Recent Post")))
                .Returns(filteredViewModels);

            // Act
            var result = await _controller.All(null, filterDate);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<BlogPostViewModel>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("Recent Post", model.First().Title);
        }

        [Fact]
        public async Task Index_ReturnsAllBlogPosts()
        {
            // Arrange
            var blogPostPrototypes = new List<BlogPostPrototype>
            {
                new BlogPostPrototype { Id = "1", Title = "Post 1" },
                new BlogPostPrototype { Id = "2", Title = "Post 2" }
            };

            var viewModels = new List<BlogPostViewModel>
            {
                new BlogPostViewModel { Id = "1", Title = "Post 1" },
                new BlogPostViewModel { Id = "2", Title = "Post 2" }
            };

            _mockBlogPostService.Setup(s => s.GetAllAsync())
                .ReturnsAsync(blogPostPrototypes);
            _mockMapper.Setup(m => m.Map<List<BlogPostViewModel>>(blogPostPrototypes))
                .Returns(viewModels);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<List<BlogPostViewModel>>(viewResult.Model);
            Assert.Equal(2, model.Count);
        }

        [Fact]
        public void Create_Get_ReturnsViewWithModel()
        {
            // Act
            var result = _controller.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<BlogPostFormModel>(viewResult.Model);
            Assert.NotNull(model);
            Assert.Equal(string.Empty, model.Content);
            Assert.True(model.CreatedOn <= DateTime.Now);
        }

        [Fact]
        public async Task Create_Post_WithValidModel_RedirectsToIndex()
        {
            // Arrange
            var formModel = new BlogPostFormModel
            {
                Title = "Test Post",
                Content = "Test Content"
            };

            var prototype = new BlogPostPrototype
            {
                Title = "Test Post",
                Content = "Test Content"
            };

            _mockMapper.Setup(m => m.Map<BlogPostPrototype>(formModel))
                .Returns(prototype);
            _mockBlogPostService.Setup(s => s.CreateAsync(prototype))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Create(formModel);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
        }

        [Fact]
        public async Task Create_Post_WithInvalidModel_ReturnsView()
        {
            // Arrange
            var formModel = new BlogPostFormModel();
            _controller.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = await _controller.Create(formModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(formModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_WithValidId_ReturnsViewWithModel()
        {
            // Arrange
            var postId = "test-id";
            var blogPost = new BlogPostPrototype
            {
                Id = postId,
                Title = "Test Post",
                Content = "Test Content"
            };

            var formModel = new BlogPostFormModel
            {
                Title = "Test Post",
                Content = "Test Content"
            };

            _mockBlogPostService.Setup(s => s.GetByIdAsync(postId))
                .ReturnsAsync(blogPost);
            _mockMapper.Setup(m => m.Map<BlogPostFormModel>(blogPost))
                .Returns(formModel);

            // Act
            var result = await _controller.Edit(postId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(formModel, viewResult.Model);
        }

        [Fact]
        public async Task Edit_Get_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var postId = "invalid-id";
            _mockBlogPostService.Setup(s => s.GetByIdAsync(postId))
                .ReturnsAsync((BlogPostPrototype)null);

            // Act
            var result = await _controller.Edit(postId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_WithValidId_ReturnsViewWithModel()
        {
            // Arrange
            var postId = "test-id";
            var blogPost = new BlogPostPrototype
            {
                Id = postId,
                Title = "Test Post",
                Content = "Test Content"
            };

            var viewModel = new BlogPostViewModel
            {
                Id = postId,
                Title = "Test Post",
                Content = "Test Content"
            };

            _mockBlogPostService.Setup(s => s.GetByIdAsync(postId))
                .ReturnsAsync(blogPost);
            _mockMapper.Setup(m => m.Map<BlogPostViewModel>(blogPost))
                .Returns(viewModel);

            // Act
            var result = await _controller.Details(postId);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(viewModel, viewResult.Model);
        }

        [Fact]
        public async Task Details_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var postId = "invalid-id";
            _mockBlogPostService.Setup(s => s.GetByIdAsync(postId))
                .ReturnsAsync((BlogPostPrototype)null);

            // Act
            var result = await _controller.Details(postId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}