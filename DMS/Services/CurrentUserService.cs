using DAS.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
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
        private readonly IMemoryCache mem;

        public CurrentUserService(IHttpContextAccessor http, IIdentityService identService, IMemoryCache mem)
        {
            this.http = http;
            this.identService = identService;
            this.mem = mem;
        }

        public async Task<AppUser> GetCurrentUserAsync()
        {
            AppUser appUser;

            if (http.HttpContext.User.Identity.IsAuthenticated)
            {
                var userName = http.HttpContext.User.Identity.Name;
                
                appUser = mem.Get<AppUser>($"AppUser_{userName}");
                if (appUser == null)
                {
                    var user = await identService.GetUserAsync(userName).ConfigureAwait(false);
                    if (user != null)
                    {
                        appUser = user;
                        mem.Set<AppUser>($"AppUser_{userName}", appUser,new TimeSpan(0,10,0));
                    }
                    else
                    {
                        appUser = new AppUser
                        {
                            UserName = "Anonymous",
                            IsAnonymous = true
                        };
                    }
                }

            }
            else
            {
                appUser = new AppUser();
            }

            return appUser;
        }
    }
}
