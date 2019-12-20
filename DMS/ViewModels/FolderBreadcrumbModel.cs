using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.ViewModels
{
    public class FolderBreadcrumbModel
    {
        public int Id { get; set; }
        public int RepositoryId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int? ParentId { get; set; }
        public string Path { get; set; }

        public override string ToString()
        {
            return Path;
        }
    }
}
