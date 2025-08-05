using System.Net.Mime;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SunNext.Data.Common.Repositories;
using SunNext.Data.Models;
using SunNext.Services.Data.Prototypes.SolarAsset;
using SunNext.Services.Data.Prototypes.VirtualWallet;
using SunNext.Services.VirtualWallet;

namespace SunNext.Services.VirtualWallet;

public class VirtualWalletService : IVirtualWalletService
{
    private readonly IDeletableEntityRepository<SunNext.Data.Models.VirtualWallet> _walletRepository;
    private readonly IMapper _mapper;

    public VirtualWalletService(IDeletableEntityRepository<SunNext.Data.Models.VirtualWallet> walletRepository, IMapper mapper)
    {
        this._walletRepository = walletRepository;
        this._mapper = mapper;
    }
    public async Task FillWalletForUserAsync(string userId, double amountKWh, string source)
    {
        var wallet = await _walletRepository
            .All()
            .FirstOrDefaultAsync(w => w.OwnerId == userId);

        if (wallet == null) return;

        wallet.AddEnergy(amountKWh, source);
        await _walletRepository.SaveChangesAsync();
    }
    public async Task<VirtualWalletPrototype> GetWalletByUserIdAsync(string userId)
    {
        var entity = await this._walletRepository
            .AllAsNoTracking()
            .FirstOrDefaultAsync(x => x.OwnerId == userId);

        return this._mapper.Map<VirtualWalletPrototype>(entity);
    }
}