using DAS.Models;
using DAS.Services;
using DAS.Utils;
using DAS.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Data
{
    public class DbInitializer
    {
        public static void Initialize(DasContext context, IConfiguration config, IServiceProvider serviceProvider)
        {
            AppSettingsProvider.DefaultUploadFolder = config["DefaultUploadFolder"];
            AppSettingsProvider.TempFolder = config["TempFolder"];
            Directory.CreateDirectory(AppSettingsProvider.DefaultUploadFolder);
            Directory.CreateDirectory(AppSettingsProvider.TempFolder);

            var initializer = new DbInitializer();
            initializer.Seed(context, serviceProvider);
        }

        private void Seed(DasContext context, IServiceProvider serviceProvider)
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

            if ((context.Repositories.Count()) == 0)
            {
                var repo = new Repository
                {
                    Name = "Default",
                    Title = "Default",
                    StorageType = StorageType.Directory,
                    Path = AppSettingsProvider.DefaultUploadFolder,
                    CreatedBy = "admin",
                    CreatedOn = DateTime.Now,
                    UpdatedBy = "admin",
                    UpdatedOn = DateTime.Now,
                    IsDeleted = false
                };
                context.Repositories.Add(repo);
                context.SaveChanges();
            }
        }
    }
}
