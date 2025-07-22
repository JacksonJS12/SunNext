using System;

namespace SunNext.Services.Data.BlogPost
{
    public class BlogPostFormPrototype
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsPublished { get; set; }
        public string ImagesUrls { get; set; } 
    }
}
