using System;
using System.Collections.Generic;
using System.Text;

namespace DAS.Models
{
    public class Repository : ITrackable, ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public StorageType StorageType { get; set; }
        public string Path { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<RepositoryMeta> MetaData { get; set; }
        public ICollection<Folder> Folders { get; set; }
        public ICollection<Document> Documents { get; set; }
    }
}
