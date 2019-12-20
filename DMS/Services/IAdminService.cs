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
        Task<Folder> GetFolderByIdAsync(int folderId);
        Task<Folder> AddFolderAsync(CreateFolderModel model);
        Task<bool> UpdateFolderAsync(UpdateFolderModel model);
        Task<Folder> DeleteFolderAsync(int folderId);
        Task<List<Folder>> GetFoldersWithChildsAsync();
        Task<List<Folder>> GetChildFoldersAsync(int? parentFolderId, int? repositoryId = null);
        #endregion

        #region Document Query And CRUD Operations
        public Task<List<Document>> GetDocumentsAsync(int folderId);
        public Task<Document> GetDocumentyByIdAsync(int documentId);
        public Task<Document> AddDocumentAsync(CreateDocumentModel model);
        public Task<bool> UpdateDocumentAsync(UpdateDocumentModel model);
        public Task<Document> DeleteDocumentAsync(int documentId); 
        #endregion

        #region MetaField Query And CRUD Operations
        public Task<List<MetaField>> GetMetaFieldsAsync();
        public Task<MetaField> GetMetaByIdAsync(int fieldId);
        public Task<List<MetaField>> GetMetaFieldsByNameAsync(string name);
        public Task<List<MetaField>> GetMetaFieldsByTitleAsync(string name);
        public Task<MetaField> AddMetaFieldAsync(MetaField model);
        public Task<bool> UpdateMetaFieldAsync(MetaField model);
        public Task<MetaField> DeleteMetaFieldAsync(int fieldId); 
        #endregion
    }
}