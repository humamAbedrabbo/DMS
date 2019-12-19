using DMS.Models;
using DMS.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DMS.Services
{
    public interface IAdminService
    {
        #region Repository Query And CRUD Operations
        Task<Repository> AddRepositoryAsync(CreateRepositoryModel model);
        Task<Repository> DeleteRepositoryAsync(int repositoryId);
        Task<List<Repository>> GetRepositoriesAsync();
        Task<Repository> GetRepositoryByIdAsync(int repositoryId);
        Task<Repository> GetRepositoryByIdWithFolderTreeAsync(int repositoryId);
        Task<Repository> GetRepositoryByNameAsync(string name);
        Task<bool> UpdateRepositoryAsync(UpdateRepositoryModel model);
        #endregion

        #region Folder Query And CRUD Operations
        Task<List<Folder>> GetFoldersAsync();
        Task<Folder> GetFolderyByIdAsync(int folderId);
        Task<Folder> AddFolderyAsync(CreateFolderModel model);
        Task<bool> UpdateFolderAsync(UpdateFolderModel model);
        Task<Folder> DeleteFolderAsync(int folderId);
        Task<List<Folder>> GetFoldersWithChildsAsync();
        Task<List<Folder>> GetChildFoldersAsync(int? parentFolderId, int? repositoryId = null);
        #endregion
    }
}