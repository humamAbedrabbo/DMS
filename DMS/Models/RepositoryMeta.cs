namespace DAS.Models
{
    public class RepositoryMeta
    {
        public int RepositoryId { get; set; }
        public Repository Repository { get; set; }
        public int FieldId { get; set; }
        public MetaField Field { get; set; }
        public string Value { get; set; }
    }
}
