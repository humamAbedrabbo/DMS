using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class MetaField
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public FieldType FieldType { get; set; }
        public string DefaultValue { get; set; }
        public string Group => (Name ?? string.Empty).Split('.')[0];
        public string FieldTypeName => FieldType.ToString();
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        public string GetDefaultTitle()
        {
            if (string.IsNullOrEmpty(Name))
                return null;
            if (string.IsNullOrEmpty(Group))
                return Name;
            else
                return Name.Split(".")[1];
        }
    }
}
