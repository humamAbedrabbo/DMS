namespace DAS.Models
{
    public class FolderMeta
    {
        public int FolderId { get; set; }
        public Folder Folder { get; set; }
        public int FieldId { get; set; }
        public MetaField Field { get; set; }
        public string Value { get; set; }
    }
}
