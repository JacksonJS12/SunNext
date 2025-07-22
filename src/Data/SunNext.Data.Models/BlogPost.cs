using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SunNext.Data.Common.Models;

namespace SunNext.Data.Models;

public class BlogPost : BaseDeletableModel<string>
{
    [Required]
    public string Title { get; set; } = null!;

    public string? Summary { get; set; }

    [Required]
    public string Content { get; set; } = null!;

    public bool IsPublished { get; set; }

    public string ImagesUrls { get; set; } 
}