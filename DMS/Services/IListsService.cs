using DAS.Models;
using DAS.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAS.Services
{
    public interface IListsService
    {
        Task<DocumentDetailModel> GetDocumentById(int? id);
        Task<IEnumerable<DocumentDetailModel>> GetDocumentsList(string repoId, int? parentId);
        Task<FolderDetailModel> GetFolderById(int? id);
        Task<IEnumerable<FolderDetailModel>> GetFoldersList(string repoId, int? parentId);
        Task<MetaField> GetMetaFieldById(int id);
        Task<IEnumerable<MetaField>> GetMetaFieldsList();
        Task<RepoDetailModel> GetRepositoryById(string id);
        Task<IEnumerable<RepoDetailModel>> GetRepositoryList();
        Task<IEnumerable<TreeModel>> GetTree(int repoId, int? folderId);
        Task<FolderBreadcrumbModel> GetFolderBreadcrumb(int folderId);
    }
}