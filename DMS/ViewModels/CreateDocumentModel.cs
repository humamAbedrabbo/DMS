using System.ComponentModel.DataAnnotations;

namespace DMS.ViewModels
{
    public class CreateDocumentModel
    {
        [Required]
        [StringLength(Constants.DOCUMENT_NAME_MAX_LENGTH)]
        [Display(Name = "Document Name")]
        public string Name { get; set; }

        [StringLength(Constants.DOCUMENT_DESC_MAX_LENGTH)]
        public string Description { get; set; }

        public int RepositoryId { get; set; }

        public int FolderId { get; set; }
    }
}
