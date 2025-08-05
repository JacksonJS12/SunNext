using System;
using AutoMapper;
using SunNext.Services.Data.Prototypes.Market;
using SunNext.Web.ViewModels.Market;

namespace SunNext.Web;

public class MarketMappingProfile : Profile
{
    public MarketMappingProfile()
    {
        CreateMap<MarketPricePrototype, MarketPriceViewModel>()
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => $"{src.Date:dd/MM/yyyy}"))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PricePerMWh));

        CreateMap<TradePositionInputModel, TradePositionPrototype>()
            .ForMember(dest => dest.TradeDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.TradeDate)));
    }
}