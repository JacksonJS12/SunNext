using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SunNext.Data.Common.Models;

namespace SunNext.Data.Models;

public class BlogPost : BaseDeletableModel<string>
{
    public BlogPost()
    {
        this.Id = Guid.NewGuid().ToString();
    }
    [Required]
    public string Title { get; set; }

    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } 

    public bool IsPublished { get; set; }

    public string ImagesUrls { get; set; } 
}