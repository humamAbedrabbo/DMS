using System;

namespace DAS.ViewModels
{
    public class MetaSearchTerm
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public MetaSearchOperation Operation { get; set; }
        public string OperationName => Operation.ToString();
        
    }
}
