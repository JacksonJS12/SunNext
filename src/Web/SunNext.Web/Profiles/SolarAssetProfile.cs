using AutoMapper;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Web.ViewModels.SolarAssets;
using SolarAsset = SunNext.Data.Models.SolarAsset;

namespace SunNext.Web;

public class SolarAssetProfile : Profile
{
    public SolarAssetProfile()
    {
        CreateMap<SolarAsset, SolarAssetPrototype>().ReverseMap();

        CreateMap<SolarAssetViewModel, SolarAssetPrototype>().ReverseMap();

        CreateMap<SolarAssetFormModel, SolarAssetPrototype>().ReverseMap();

        CreateMap<AllSolarAssetsQueryModel, AllSolarAssetsQueryPrototype>().ReverseMap();

        CreateMap<SolarAsset, AllSolarAssetsQueryPrototype>();

        CreateMap<AllSolarAssetsQueryPrototype, SolarAssetViewModel>();
        
        CreateMap<SolarAsset, SolarAssetListItemPrototype>();
        
        CreateMap<SolarAssetListItemPrototype, SolarAssetViewModel>();
        
        CreateMap<SolarAssetListItemPrototype, SolarAssetListItemViewModel>();

    }
}