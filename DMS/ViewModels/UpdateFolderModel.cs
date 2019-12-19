using System.ComponentModel.DataAnnotations;

namespace DMS.ViewModels
{
    public class UpdateFolderModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(Constants.FOLDER_NAME_MAX_LENGTH)]
        [Display(Name = "Folder Name")]
        public string Name { get; set; }

        [StringLength(Constants.FOLDER_DESC_MAX_LENGTH)]
        public string Description { get; set; }
    }
}
