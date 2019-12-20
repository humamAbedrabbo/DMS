using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.ViewModels
{
    public class UploadViewModel
    {
        public int RepositoryId { get; set; }
        public string RepositoryName { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
    }
}
