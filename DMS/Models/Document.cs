using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class Document
    {
        public Document()
        {
            CreatedOn = UpdatedOn = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int RepositoryId { get; set; }
        public Repository Repository { get; set; }
        public int FolderId { get; set; }
        public Folder Folder { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

    }
}
