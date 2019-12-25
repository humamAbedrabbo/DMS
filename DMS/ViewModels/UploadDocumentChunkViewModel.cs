using DAS.Models;

namespace DAS.ViewModels
{
    public class UploadDocumentChunkViewModel
    {
        public int DocumentId { get; set; }
        public int Version { get; set; }
        public int RepositoryId { get; set; }
        public string UploadPath { get; set; }
        public int? ParentId { get; set; }
        public StorageType Storage { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public int ChunkNo { get; set; }
        public int ChunkSize { get; set; }
        public byte[] Bytes { get; set; }
        public string UserName { get; set; }
        public string Path { get; set; }

    }
}
