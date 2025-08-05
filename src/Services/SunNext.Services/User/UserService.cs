using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SunNext.Data.Common.Repositories;
using SunNext.Data.Models;

namespace SunNext.Services.User;

public class UserService : IUserService
{
    private readonly IDeletableEntityRepository<ApplicationUser> _userRepository;

    public UserService(IDeletableEntityRepository<ApplicationUser> userRepository)
    {
        this._userRepository = userRepository;
    }

    public async Task<int> CountAsync()
        => await this._userRepository.AllWithDeleted().CountAsync();
}