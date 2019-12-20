namespace DAS.Models
{
    public class MetaField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public FieldType FieldType { get; set; } = FieldType.Text;
        public string ListSource { get; set; }
        public string DefaultValue { get; set; }
        public string Group => (Name ?? string.Empty).Split('.')[0];
        public string FieldTypeName => FieldType.ToString();
    }

    public enum FieldType
    {
        Text,
        LargeText,
        Number,
        Decimal,
        Date,
        DateTime,
        Boolean,
        List
    }
}
