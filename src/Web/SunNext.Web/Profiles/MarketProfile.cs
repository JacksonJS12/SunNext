using System;
using AutoMapper;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.Market;
using SunNext.Web.ViewModels.Market;

namespace SunNext.Web;

public class MarketProfile : Profile
{
    public MarketProfile()
    {
        CreateMap<MarketPricePrototype, MarketPriceViewModel>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => $"{src.Date:dd/MM/yyyy}"))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PricePerMWh));


    }
}