using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.ViewModels
{
    public class TreeModel
    {
        public int RepositoryId { get; set; }
        public string Repository { get; set; }
        public TreeNodeType Type { get; set; }
        public string TypeName => Type.ToString();
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public Dictionary<string,string> Meta { get; set; }
    }
}
