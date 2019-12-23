using DAS.Data;
using DAS.ViewModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly DasContext context;
        private readonly UserManager<IdentityUser> um;
        private readonly RoleManager<IdentityRole> rm;

        public IdentityService(DasContext context, UserManager<IdentityUser> um, RoleManager<IdentityRole> rm)
        {
            this.context = context;
            this.um = um;
            this.rm = rm;
        }

        public async Task<List<AppUser>> GetUsersAsync()
        {
            var users = new List<AppUser>();

            var usersList = um.Users.ToList();
            foreach (var u in usersList)
            {
                var user = new AppUser { Id = u.Id, UserName = u.UserName };
                user.Roles = (await um.GetRolesAsync(u).ConfigureAwait(false)).ToList();
                
                user.Repositories = (await um.GetClaimsAsync(u))
                    .Where(x => x.Type == "RepositoryId")
                    .Select(x => Convert.ToInt32(x.Value))
                    .ToList();

                users.Add(user);
            }

            return users;
        }

        public async Task<IdentityUser> CreateUserAsync(string userName, string email, string password = Constants.USER_DEFAULT_PWD)
        {
            var user = new IdentityUser
            {
                UserName = userName,
                Email = email
            };

            var result = await um.CreateAsync(user, password).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task<IdentityRole> CreateRoleAsync(string roleName)
        {
            var role = new IdentityRole
            {
                Name = roleName
            };

            var result = await rm.CreateAsync(role).ConfigureAwait(false);
            if (result.Succeeded)
            {
                return role;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> AddUserRoleAsync(string userName, string roleName)
        {
            var user = await um.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null)
                return false;

            var result = await um.AddToRoleAsync(user, roleName).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserRoleAsync(string userName, string roleName)
        {
            var user = await um.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null)
                return false;

            var result = await um.RemoveFromRoleAsync(user, roleName).ConfigureAwait(false);
            return result.Succeeded;
        }

        public async Task<bool> IsUserInRoleAsync(string userName, string roleName)
        {
            var user = await um.FindByNameAsync(userName).ConfigureAwait(false);
            if (user == null)
                return false;

            return await um.IsInRoleAsync(user, roleName).ConfigureAwait(false);
        }

        public async Task<AppUser> GetUserAsync(string userName)
        {
            var user = await um.FindByNameAsync(userName).ConfigureAwait(false);
            if (user != null)
            {
                var appUser = new AppUser
                {
                    Id = user.Id,
                    UserName = user.UserName
                };
                appUser.Roles = await GetUserRolesAsync(user).ConfigureAwait(false);
                appUser.Repositories = (await um.GetClaimsAsync(user))
                    .Where(x => x.Type == "RepositoryId")
                    .Select(x => Convert.ToInt32(x.Value))
                    .ToList();
                return appUser;
            }

            return null;
        }

        private async Task<List<string>> GetUserRolesAsync(IdentityUser user)
        {
            return (await um.GetRolesAsync(user).ConfigureAwait(false)).ToList();
        }
    }
}
