using System.Threading.Tasks;

namespace SunNext.Services.User;

public interface IUserService
{
    Task<int> CountAsync();
}