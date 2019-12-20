namespace DAS.Models
{
    public class DocumentThumbnail
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; }
        public string Base64Image { get; set; }
    }
}
