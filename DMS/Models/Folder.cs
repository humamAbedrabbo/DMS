using System;
using System.Collections.Generic;

namespace DAS.Models
{
    public class Folder : ITrackable, ISoftDelete
    {
        public int Id { get; set; }
        public int RepositoryId { get; set; }
        public Repository Repository { get; set; }
        public int? ParentId { get; set; }
        public Folder Parent { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<FolderMeta> MetaData { get; set; }
        public ICollection<Folder> Folders { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}
