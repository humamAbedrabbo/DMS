using DAS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.ViewModels
{
    public class DocumentHistoryDetailModel
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int Version { get; set; }
        public DocumentOperation Operation { get; set; }
        public string OperationBy { get; set; }
        public DateTime OperationOn { get; set; }
    }
}
