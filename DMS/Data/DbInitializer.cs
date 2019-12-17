using DMS.Services;
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
        }
    }
}
