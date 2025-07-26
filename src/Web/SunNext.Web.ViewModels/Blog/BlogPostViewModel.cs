using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SunNext.Web.ViewModels.Blog;

public class BlogPostViewModel
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Content { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsPublished { get; set; }
    public string ImagesUrls { get; set; }
}