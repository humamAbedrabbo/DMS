using DAS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.ViewModels
{
    public class RepoAddModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public StorageType Storage { get; set; }
        public string Path { get; set; }
        public string UserName { get; set; }
        public Dictionary<string,string> Meta { get; set; }
    }
}
