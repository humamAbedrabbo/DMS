using System.Text;

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
        public string FieldFormat
        {
            get
            {
                if (FieldType == FieldType.Date)
                    return "yyyy-MM-dd";
                else
                    return null;
            }
        }
        public string FieldTag
        {
            get
            {
                switch(FieldType)
                {
                    case FieldType.List:
                        return "select";
                    case FieldType.LargeText:
                        return "textarea";
                    default:
                        return "input";
                }
            }
        }
        public string FieldTagInputType
        {
            get
            {
                switch (FieldType)
                {
                    case FieldType.Number:
                    case FieldType.Decimal:
                        return "number";
                    case FieldType.Date:
                        return "date";
                    default:
                        return "text";
                }
            }
        }
        public string FieldTagInputSelectItems
        {
            get
            {
                if (FieldType == FieldType.List && !string.IsNullOrEmpty(ListSource))
                {
                    var sb = new StringBuilder();
                    foreach (var item in ListItems)
                    {
                        sb.AppendLine($"<option value='{item}'>{item}</option>");
                    }
                    return sb.ToString();
                }

                return "";
            }
        }
        public string[] ListItems => ListSource?.Split(',');
    }

    public enum FieldType
    {
        Text,
        LargeText,
        Number,
        Decimal,
        Date,
        List
    }
}
