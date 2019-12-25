using DAS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.ViewModels
{
    public class RepoAddModel
    {
        [Required]
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public StorageType Storage { get; set; }
        public string Path { get; set; }
        public string UserName { get; set; }
        public Dictionary<string,string> Meta { get; set; }
    }
}
