using DAS.Models;
using DAS.ViewModels;
using System.Threading.Tasks;

namespace DAS.Services
{
    public interface IArchiveService
    {
        Task<DocumentDetailModel> AddDocument(DocumentAddModel model);
        Task<FolderDetailModel> AddFolder(FolderAddModel model);
        Task<DocumentDetailModel> Checkin(DocumentCheckinModel model);
        Task<DocumentCheckoutResultModel> Checkout(DocumentCheckoutModel model);
        Task UpdateDocument(int? id, DocumentUpdateModel model);
        Task UpdateFolder(int? id, FolderUpdateModel model);
        Task<bool> UploadChunk(ChunkAddModel model);
        Task<UploadDocumentResultViewModel> UploadDocument(UploadDocumentViewModel uploadDocument);
        Task<UploadDocumentChunkResultViewModel> UploadDocumentChunk(UploadDocumentChunkViewModel uploadChunk);
        Task<UploadDocumentResultViewModel> UploadDocumentForCheckIn(UploadDocumentViewModel uploadDocument);
        Task UploadThumbnail(DocumentThumbnail model);
    }
}