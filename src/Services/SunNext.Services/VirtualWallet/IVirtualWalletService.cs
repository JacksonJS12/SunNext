using System.Threading.Tasks;
using SunNext.Services.Data.Prototypes.VirtualWallet;

namespace SunNext.Services.VirtualWallet;

public interface IVirtualWalletService
{
    Task FillWalletForUserAsync(string userId, double totalGeneratedKWh, string simulatedGeneration);
    Task<VirtualWalletPrototype> GetWalletByUserIdAsync(string userId);
}