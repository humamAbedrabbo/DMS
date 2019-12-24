using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.ViewModels
{
    public class AppUser
    {
        public AppUser()
        {
            Roles = new List<string>();
            Repositories = new List<int>();
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public List<string> Roles { get; set; }
        public List<int> Repositories { get; set; }
        public string RepoToAdd { get; set; }
        public string Lang => "ar";
        public string Rtl => Lang == "ar" ? "rtl" : "";
        public bool IsAdmin => Roles?.Contains(Constants.ROLE_ADMIN) ?? false;
        public bool IsArchive => Roles?.Contains(Constants.ROLE_ARCHIVE) ?? false;
        public bool IsPublic => Roles?.Contains(Constants.ROLE_PUBLIC) ?? false;
    }

}
