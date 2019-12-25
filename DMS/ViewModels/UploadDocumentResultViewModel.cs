namespace DAS.ViewModels
{
    public class UploadDocumentResultViewModel
    {
        public bool IsOk { get; set; }
        public string ErrorMessage { get; set; }
        public int DocumentId { get; set; }
        public int Version { get; set; }
        public string Path { get; set; }
    }
}
