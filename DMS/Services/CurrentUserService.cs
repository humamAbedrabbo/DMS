using DAS.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor http;
        private readonly IIdentityService identService;

        public CurrentUserService(IHttpContextAccessor http, IIdentityService identService)
        {
            this.http = http;
            this.identService = identService;
        }

        public async Task<AppUser> GetCurrentUserAsync()
        {
            var appUser = new AppUser
            {
                UserName = "Anonymous"
            };

            if (http.HttpContext.User.Identity.IsAuthenticated)
            {
                var userName = http.HttpContext.User.Identity.Name;
                var user = await identService.GetUserAsync(userName).ConfigureAwait(false);
                if (user != null)
                {
                    appUser = user;
                }
            }

            return appUser;
        }
    }
}
