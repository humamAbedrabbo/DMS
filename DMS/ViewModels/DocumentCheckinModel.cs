namespace DAS.ViewModels
{
    public class DocumentCheckinModel
    {
        public int DocumentId { get; set; }
        public long Length { get; set; }
        public string CheckInKey { get; set; }
        public string UserName { get; set; }
    }

}
