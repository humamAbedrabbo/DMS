namespace DAS.ViewModels
{
    public class DocumentCheckoutResultModel
    {
        public bool IsOk { get; set; } = false;
        public string ErrorMessage { get; set; }
        public int DocumentId { get; set; }
        public string CheckInKey { get; set; }
        public string UserName { get; set; }
    }

}
