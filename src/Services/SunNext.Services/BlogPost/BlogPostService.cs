using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SunNext.Data.Common.Repositories;
using SunNext.Data.Models;
using SunNext.Services.BlogPost;
using SunNext.Services.Data.Blog;

namespace SunNext.Services.Data
{
   public class BlogPostService : IBlogPostService
    {
        private readonly IDeletableEntityRepository<SunNext.Data.Models.BlogPost> _blogRepository;
        private readonly IMapper _mapper;

        public BlogPostService(
            IDeletableEntityRepository<SunNext.Data.Models.BlogPost> blogRepository,
            IMapper mapper)
        {
            this._blogRepository = blogRepository;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<BlogPostPrototype>> GetAllAsync()
        {
            var posts = await this._blogRepository
                .AllAsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();

            return this._mapper.Map<IEnumerable<BlogPostPrototype>>(posts);
        }

        public async Task<BlogPostPrototype> GetByIdAsync(string id)
        {
            var post = await this._blogRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return this._mapper.Map<BlogPostPrototype>(post);
        }

        public async Task CreateAsync(BlogPostPrototype input)
        {
            var entity = this._mapper.Map<SunNext.Data.Models.BlogPost>(input);
            entity.Id = Guid.NewGuid().ToString();
            await this._blogRepository.AddAsync(entity);
            await this._blogRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(string id, BlogPostPrototype input)
        {
            var entity = await this._blogRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            this._mapper.Map(input, entity);
            await this._blogRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await this._blogRepository
                .All()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return false;

            this._blogRepository.Delete(entity);
            await this._blogRepository.SaveChangesAsync();
            return true;
        }
        public async Task<IList<BlogPostPrototype>> GetAllPublishedAsync()
        {
            var posts =  await this._blogRepository.AllAsNoTracking()
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreatedOn)
                .ToListAsync();
            
            return this._mapper.Map<List<BlogPostPrototype>>(posts);
        }

    }
}