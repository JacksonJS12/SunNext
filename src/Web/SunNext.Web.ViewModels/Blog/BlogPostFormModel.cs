using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SunNext.Web.ViewModels.Blog;

public class BlogPostFormModel
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public string Content { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsPublished { get; set; }
    public string ImagesUrls { get; set; } 
}
