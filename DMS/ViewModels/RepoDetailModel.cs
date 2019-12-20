using DAS.Models;
using System;
using System.Collections.Generic;

namespace DAS.ViewModels
{
    public class RepoDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public StorageType Storage { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Dictionary<string, string> Meta { get; set; }
    }

}
