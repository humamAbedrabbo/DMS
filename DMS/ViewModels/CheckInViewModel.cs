using System.Collections.Generic;

namespace DAS.ViewModels
{
    public class CheckInViewModel
    {
        public int DocumentId { get; set; }
        public string CheckInKey { get; set; }
        public int RepositoryId { get; set; }
        public string RepositoryName { get; set; }
        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public string Path { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public Dictionary<string, string> Meta { get; set; } = new Dictionary<string, string>();
    }
}
