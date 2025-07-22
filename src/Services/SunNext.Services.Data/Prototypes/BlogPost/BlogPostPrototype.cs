using System;
using System.Collections.Generic;

namespace SunNext.Services.Data.Blog;

public class BlogPostPrototype
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Content { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsPublished { get; set; }
    public string ImagesUrls { get; set; } 

}