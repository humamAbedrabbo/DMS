using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Models
{
    public class Folder
    {
        public static char[] ForbiddenCharactersInName =
        {
            '"',
            '\'',
            ',',
            ';',
            '\\',
            '/',
            '|',
            '&',
            '$',
            '>',
            '<',
            '*',
            '?',
            ':'
        };

        public static string[] ForbiddenNames =
        {
            ".",
            ".."
        };

        public Folder()
        {
            CreatedOn = UpdatedOn = DateTime.Now;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int RepositoryId { get; set; }
        public Repository Repository { get; set; }
        public int? ParentId { get; set; }
        public Folder Parent { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool IsDeleted { get; set; }

        public List<Folder> Childs { get; set; }
        public List<Document> Documents { get; set; }

        public void AddChildFolder(Folder child)
        {
            if(Childs == null)
            {
                Childs = new List<Folder>();
            }

            if(child.ParentId != this.Id && this.Id > 0)
            {
                child.ParentId = this.Id;
            }

            child.Parent = this;
            Childs.Add(child);
        }

        public bool Validate()
        {
            return ValidateName() && ValidateDescription();
        }

        private bool ValidateName()
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            if (Name.Length > Constants.FOLDER_NAME_MAX_LENGTH)
                return false;

            foreach (var forbiddenChar in ForbiddenCharactersInName)
            {
                if (Name.Contains(forbiddenChar))
                    return false;
            }

            foreach (var forbiddenName in ForbiddenNames)
            {
                if (Name == forbiddenName)
                    return false;
            }
            return true;
        }

        private bool ValidateDescription()
        {
            if (string.IsNullOrEmpty(Description))
                return true;

            if (Description.Length > Constants.FOLDER_DESC_MAX_LENGTH)
                return false;
            
            return true;
        }
    }
}
