using System.Collections.Generic;

namespace DAS.ViewModels
{
    public class SearchResult
    {
        public SearchTerm Term { get; set; } = new SearchTerm();
        public List<SearchResultModel> Results { get; set; } = new List<SearchResultModel>();
    }
}
