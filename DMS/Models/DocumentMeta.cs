namespace DAS.Models
{
    public class DocumentMeta
    {
        public int DocumentId { get; set; }
        public Document Document { get; set; }
        public int FieldId { get; set; }
        public MetaField Field { get; set; }
        public string Value { get; set; }
    }
}
