using System.ComponentModel.DataAnnotations;

namespace DMS.ViewModels
{
    public class UpdateRepositoryModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(Constants.REPO_NAME_MAX_LENGTH)]
        [Display(Name = "Repository Name")]
        public string Name { get; set; }

        [StringLength(Constants.REPO_DESC_MAX_LENGTH)]
        public string Description { get; set; }
    }
}
