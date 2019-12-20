using System;
using System.Collections.Generic;

namespace DAS.Models
{
    public class Document : ITrackable, ISoftDelete
    {
        public int Id { get; set; }
        public int RepositoryId { get; set; }
        public Repository Repository { get; set; }
        public int? ParentId { get; set; }
        public Folder Parent { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Version { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public string CheckInKey { get; set; } = Guid.NewGuid().ToString("D");
        public DocumentOperation LastOperation { get; set; }
        public string OperationBy { get; set; }
        public DateTime OperationDate { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<DocumentMeta> MetaData { get; set; }
        public ICollection<Chunk> Chunks { get; set; }
        public ICollection<DocumentHistory> History { get; set; }
        public ICollection<DocumentThumbnail> Thumbnails { get; set; }
    }
}
