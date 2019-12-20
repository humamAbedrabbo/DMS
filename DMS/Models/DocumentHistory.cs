using System;

namespace DAS.Models
{
    public class DocumentHistory
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Document Document { get; set; }
        public int Version { get; set; }
        public DocumentOperation Operation { get; set; }
        public string OperationBy { get; set; }
        public DateTime OperationOn { get; set; }
    }
}
