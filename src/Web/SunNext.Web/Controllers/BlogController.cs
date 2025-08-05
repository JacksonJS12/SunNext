using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunNext.Common;
using SunNext.Services.BlogPost;
using SunNext.Services.Data.Blog;
using SunNext.Web.ViewModels.Blog;

namespace SunNext.Web.Controllers
{
    public class BlogController : BaseController
    {
        private readonly IBlogPostService _blogPostService;
        private readonly IMapper _mapper;

        public BlogController(IBlogPostService blogPostService, IMapper mapper)
        {
            this._blogPostService = blogPostService;
            this._mapper = mapper;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> All(string search, DateTime? fromDate)
        {
            var posts = await this._blogPostService.GetAllPublishedAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                posts = posts
                    .Where(p => p.Title != null && p.Title.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            if (fromDate.HasValue)
            {
                posts = posts
                    .Where(p => p.CreatedOn.Date >= fromDate.Value.Date)
                    .ToList();
            }

            var model = this._mapper.Map<IList<BlogPostViewModel>>(posts);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Index()
        {
            var postsPrototypes = await this._blogPostService.GetAllAsync();
            var posts = this._mapper.Map<List<BlogPostViewModel>>(postsPrototypes);
            return View(posts);
        }

        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public IActionResult Create()
        {
            var model = new BlogPostFormModel
            {
                CreatedOn = DateTime.Now,
                Content = string.Empty 
            };

            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Create(BlogPostFormModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var entity = this._mapper.Map<BlogPostPrototype>(model);
            await this._blogPostService.CreateAsync(entity);
            return RedirectToAction("Index");
        }


        [HttpGet]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(string id)
        {
            var post = await this._blogPostService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var model = this._mapper.Map<BlogPostFormModel>(post);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Edit(string id, BlogPostFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            var prototype = this._mapper.Map<BlogPostPrototype>(model);
            await this._blogPostService.UpdateAsync(id, prototype);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Delete(string id)
        {
            await this._blogPostService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var blogPost = await this._blogPostService.GetByIdAsync(id);
            if (blogPost == null)
                return NotFound();

            var viewModel = this._mapper.Map<BlogPostViewModel>(blogPost);
            return View(viewModel);
        }
    }
}