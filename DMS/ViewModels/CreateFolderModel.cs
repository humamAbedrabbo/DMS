using System.ComponentModel.DataAnnotations;

namespace DMS.ViewModels
{
    public class CreateFolderModel
    {
        [Required]
        [StringLength(Constants.FOLDER_NAME_MAX_LENGTH)]
        [Display(Name = "Folder Name")]
        public string Name { get; set; }

        [StringLength(Constants.FOLDER_DESC_MAX_LENGTH)]
        public string Description { get; set; }

        public int RepositoryId { get; set; }

        public int? ParentId { get; set; }
    }
}
