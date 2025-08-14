using AutoMapper;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.SolarSystem;
using SunNext.Web.ViewModels.SolarSystem;

namespace SunNext.Web;

public class SolarSystemProfile : Profile
{
    public SolarSystemProfile()
    {
        CreateMap<SolarSystem, SolarSystemPrototype>().ReverseMap();

        CreateMap<SolarSystemViewModel, SolarSystemPrototype>().ReverseMap();

        CreateMap<SolarSystemFormModel, SolarSystemPrototype>().ReverseMap();

        CreateMap<AllSolarSystemQueryModel, AllSolarSystemsQueryPrototype>().ReverseMap();

        CreateMap<SolarSystem, AllSolarSystemsQueryPrototype>();

        CreateMap<AllSolarSystemsQueryPrototype, SolarSystemViewModel>();
        
        CreateMap<SolarSystem, SolarSystemListItemPrototype>();
        
        CreateMap<SolarSystemListItemPrototype, SolarSystemViewModel>();
        
        CreateMap<SolarSystemListItemPrototype, SolarSystemListItemViewModel>();

    }
}