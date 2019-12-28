using System;

namespace DAS.ViewModels
{
    public class MetaSearchTerm
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public MetaSearchOperation Operation { get; set; }
        public string OperationName
        {
            get
            {
                switch(Operation)
                {
                    case MetaSearchOperation.EQ:
                        return "=";
                    case MetaSearchOperation.NE:
                        return "!=";
                    case MetaSearchOperation.Like:
                        return "like";
                    case MetaSearchOperation.Unlike:
                        return "unlike";
                    default:
                        return string.Empty;
                }
            }
        }
        
    }
}
