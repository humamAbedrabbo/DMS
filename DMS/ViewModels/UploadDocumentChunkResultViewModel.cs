namespace DAS.ViewModels
{
    public class UploadDocumentChunkResultViewModel
    {
        public bool IsOk { get; set; }
        public string ErrorMessage { get; set; }
        public int DocumentId { get; set; }
        public int Version { get; set; }
        public int Id { get; set; }
    }
}
