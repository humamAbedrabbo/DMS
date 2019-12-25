using DAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.ViewModels
{
    public class UploadDocumentViewModel
    {
        public int DocumentId { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string Description { get; set; }
        public int RepositoryId { get; set; }
        public string UploadPath { get; set; }
        public int? ParentId { get; set; }
        public StorageType Storage { get; set; }
        public string UserName { get; set; }
    }
}
