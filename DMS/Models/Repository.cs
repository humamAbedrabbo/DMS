using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class Repository
    {
        public Repository()
        {
            CreatedOn = UpdatedOn = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public StorageType StorageType { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        public List<Folder> Folders { get; set; }
    }
}
