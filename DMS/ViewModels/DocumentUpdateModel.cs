using System.Collections.Generic;

namespace DAS.ViewModels
{
    public class DocumentUpdateModel
    {
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public Dictionary<string, string> Meta { get; set; }
    }

}
