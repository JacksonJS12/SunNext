using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using SunNext.Data.Models;
using SunNext.Services.Data.Blog;
using SunNext.Web.ViewModels.Blog;

namespace SunNext.Web;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        CreateMap<BlogPostFormModel, BlogPost>();
        CreateMap<BlogPost, BlogPostViewModel>();
        CreateMap<BlogPostFormModel, BlogPostPrototype>();
        CreateMap<BlogPostPrototype, BlogPost>();
        CreateMap<BlogPost, BlogPostPrototype>();
        CreateMap<BlogPostPrototype, BlogPostViewModel>();
        CreateMap<BlogPostPrototype, BlogPostFormModel>();
        CreateMap<BlogPostFormModel, BlogPostPrototype>();
        CreateMap<BlogPostPrototype, BlogPost>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

    }
}