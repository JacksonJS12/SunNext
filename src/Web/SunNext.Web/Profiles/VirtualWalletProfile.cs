using AutoMapper;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.VirtualWallet;

namespace SunNext.Web;

public class VirtualWalletProfile : Profile
{
    public VirtualWalletProfile()
    {
        CreateMap<VirtualWallet, VirtualWalletPrototype>();
    }
}