using DAS.Models;
using DAS.ViewModels;
using System.Threading.Tasks;

namespace DAS.Services
{
    public interface IAdminService
    {
        Task<DocumentMeta> AddDocumentMetaValue(int id, string name, string value = null);
        Task<FolderMeta> AddFolderMetaValue(int id, string name, string value = null);
        Task<MetaField> AddMetaField(MetaField model);
        Task<RepositoryMeta> AddRepoMetaValue(int id, string name, string value = null);
        Task<DocumentDetailModel> DeleteDocument(int? id, string username);
        Task<DocumentMeta> DeleteDocumentMetaValue(int id, string name);
        Task<FolderDetailModel> DeleteFolder(int? id, string username);
        Task<FolderMeta> DeleteFolderMetaValue(int id, string name);
        Task<MetaField> DeleteMetaField(int? id, string force = null);
        Task<RepositoryMeta> DeleteRepoMetaValue(int id, string name);
        Task<RepoDetailModel> DeleteRepository(int? id, string username);
        Task<RepoDetailModel> AddRepository(RepoAddModel model);
        Task UpdateRepository(string id, RepoUpdateModel model);
        Task<DocumentMeta> SetDocumentMetaValue(int id, string name, string value = null);
        Task<FolderMeta> SetFolderMetaValue(int id, string name, string value = null);
        Task<RepositoryMeta> SetRepoMetaValue(int id, string name, string value = null);
        Task UpdateMetaField(int? id, MetaField model);
    }
}