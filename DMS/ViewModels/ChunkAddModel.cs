using Microsoft.AspNetCore.Http;

namespace DAS.ViewModels
{
    public class ChunkAddModel
    {
        public int RepositoryId { get; set; }
        public int DocumentId { get; set; }
        public int Version { get; set; }
        public int SortId { get; set; }
        public string OriginalName { get; set; }
        public string CheckInKey { get; set; }
        public string UserName { get; set; }
        public IFormFile File { get; set; }
    }

}
