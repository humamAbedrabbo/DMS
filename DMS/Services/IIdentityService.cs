using DMS.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services
{
    public interface IIdentityService
    {
        Task<bool> AddUserRoleAsync(string userName, string roleName);
        Task<IdentityRole> CreateRoleAsync(string roleName);
        Task<IdentityUser> CreateUserAsync(string userName, string email, string password = Constants.USER_DEFAULT_PWD);
        Task<AppUser> GetUserAsync(string userName);
        Task<List<AppUser>> GetUsersAsync();
        Task<bool> IsUserInRoleAsync(string userName, string roleName);
        Task<bool> RemoveUserRoleAsync(string userName, string roleName);
    }
}