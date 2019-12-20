using System.Threading.Tasks;
using DAS.Models;
using DAS.ViewModels;

namespace DAS.Services
{
    public interface ISearchService
    {
        Task<SearchResult> SearchByTerm(SearchTerm term);
    }
}