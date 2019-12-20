using DAS.ViewModels;
using System.Threading.Tasks;

namespace DAS.Services
{
    public interface ICurrentUserService
    {
        Task<AppUser> GetCurrentUserAsync();
    }
}