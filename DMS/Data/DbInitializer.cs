using DMS.Models;
using DMS.Services;
using DMS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Data
{
    public class DbInitializer
    {
        public static void Initialize(DmsContext context, IConfiguration config, IServiceProvider serviceProvider)
        {
            var initializer = new DbInitializer();
            initializer.Seed(context, serviceProvider);
        }

        private void Seed(DmsContext context, IServiceProvider serviceProvider)
        {
            context.Database.Migrate();

            var identService = serviceProvider.GetRequiredService<IIdentityService>();
            AppUser adminUser;

            if (!context.Roles.Any())
            {
                _ = identService.CreateRoleAsync(Constants.ROLE_ADMIN).Result;
                _ = identService.CreateRoleAsync(Constants.ROLE_ARCHIVE).Result;
                _ = identService.CreateRoleAsync(Constants.ROLE_PUBLIC).Result;
            }

            if (!context.Users.Any())
            {
                _ = identService.CreateUserAsync(Constants.USER_ADMIN, Constants.USER_ADMIN_EMAIL, Constants.USER_DEFAULT_PWD).Result;
                _ = identService.AddUserRoleAsync(Constants.USER_ADMIN, Constants.ROLE_ADMIN).Result;
            }

            adminUser = identService.GetUserAsync(Constants.USER_ADMIN).Result;
            
            // Add Default Repository
            if(!context.Repositories.Any())
            {
                var repository = new Repository
                {
                    Name = "Default",
                    CreatedBy = adminUser.UserName,
                    UpdatedBy = adminUser.UserName
                };

                context.Repositories.Add(repository);
                context.SaveChanges();
            }
        }
    }
}
