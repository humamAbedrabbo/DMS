using DMS.Models;
using DMS.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services
{
    public interface IAdminService
    {
        Task<Repository> AddRepositoryAsync(CreateRepositoryModel model);
        Task<Repository> DeleteRepositoryAsync(int repositoryId);
        Task<List<Repository>> GetRepositoriesAsync();
        Task<Repository> GetRepositoryByIdAsync(int repositoryId);
        Task<Repository> GetRepositoryByNameAsync(string name);
        Task<bool> UpdateRepositoryAsync(UpdateRepositoryModel model);
    }
}