using AutoMapper;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Web.ViewModels.SolarAssets;
using SolarAsset = SunNext.Data.Models.SolarAsset;

namespace SunNext.Web;

public class SolarAssetProfile : Profile
{
    public SolarAssetProfile()
    {
        CreateMap<SolarAssetPrototype, SolarAsset>().ReverseMap();
        CreateMap<SolarAssetPrototype, SolarAssetViewModel>().ReverseMap();
        CreateMap<SolarAsset, SolarAssetPrototype>().ReverseMap();
        CreateMap<SolarAssetFormModel, SolarAssetPrototype>();
        CreateMap<SolarAssetPrototype, SolarAssetFormModel>();
    }
}