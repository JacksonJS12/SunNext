using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SunNext.Services.Data.Blog;

namespace SunNext.Services.BlogPost;

public interface IBlogPostService
{
        Task<IEnumerable<BlogPostPrototype>> GetAllAsync();
        Task<BlogPostPrototype> GetByIdAsync(string id);
        Task CreateAsync(BlogPostPrototype input);
        Task<bool> UpdateAsync(string id, BlogPostPrototype input);
        Task<bool> DeleteAsync(string id);
    

}