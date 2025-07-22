using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunNext.Common;
using SunNext.Services.BlogPost;
using SunNext.Services.Data.Blog;
using SunNext.Services.Mapping;
using SunNext.Web.ViewModels.Blog;

namespace SunNext.Web.Controllers
{
    public class BlogController : Controller
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
        public async Task<IActionResult> All()
        {
            var posts = await this._blogPostService.GetAllAsync();
            var model = this._mapper.Map<IList<BlogPostViewModel>>(posts);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Details(string id)
        {
            var post = await this._blogPostService.GetByIdAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
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
            return View();
        }

        [HttpPost]
        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> Create(BlogPostFormModel model)
        {
            if (!this.ModelState.IsValid)
            {
                return View(model);
            }

            var prototype = this._mapper.Map<BlogPostPrototype>(model);
            await this._blogPostService.CreateAsync(prototype);
            return RedirectToAction(nameof(Index));
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
                return View( model);
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
    }
}