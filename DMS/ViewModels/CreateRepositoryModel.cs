using DMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.ViewModels
{
    public class CreateRepositoryModel
    {
        [Required]
        [StringLength(Constants.REPO_NAME_MAX_LENGTH)]
        [Display(Name = "Repository Name")]
        public string Name { get; set; }

        [StringLength(Constants.REPO_DESC_MAX_LENGTH)]
        public string Description { get; set; }

        [Display(Name = "Storage Type")]
        public StorageType StorageType { get; set; }
    }
}
