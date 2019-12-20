using DAS.Models;
using DAS.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DAS.ViewModels
{
    public class SearchTerm
    {
        public SearchEntityType Type { get; set; }
        public string TypeName => Type.ToString();

        public string Name { get; set; } = "%";
        public bool IncludeTitle { get; set; } = false;
        public bool IncludeDescription { get; set; } = false;

        public List<MetaSearchTerm> MetaTerms { get; set; } = new List<MetaSearchTerm>();

    }
}
