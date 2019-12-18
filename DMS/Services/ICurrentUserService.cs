using DMS.ViewModels;
using System.Threading.Tasks;

namespace DMS.Services
{
    public interface ICurrentUserService
    {
        Task<AppUser> GetCurrentUserAsync();
    }
}