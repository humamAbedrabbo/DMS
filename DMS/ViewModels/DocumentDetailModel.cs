using DAS.Models;
using System;
using System.Collections.Generic;

namespace DAS.ViewModels
{
    public class DocumentDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int RepositoryId { get; set; }
        public string Repository { get; set; }
        public int? ParentId { get; set; }
        public string Parent { get; set; }
        public int Version { get; set; }
        public string ContentType { get; set; }
        public long Length { get; set; }
        public string CheckInKey { get; set; }
        public DocumentOperation LastOperation { get; set; }
        public string OperationBy { get; set; }
        public DateTime OperationDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public Dictionary<string, string> Meta { get; set; }
        public string Thumbnail { get; set; }
    }

}
