using System.Collections.Generic;

namespace DAS.ViewModels
{
    public class DocumentAddModel
    {
        public string RepositoryId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Path { get; set; }
        public int? ParentId { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public string UserName { get; set; }
        public Dictionary<string, string> Meta { get; set; }
    }

}
