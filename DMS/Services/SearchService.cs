using DAS.Data;
using DAS.Models;
using DAS.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Services
{
    public class SearchService : ISearchService
    {
        private readonly DasContext context;
        private readonly ICurrentUserService currentUserService;
        private AppUser currentUser;

        public SearchService(DasContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<SearchResult> SearchByTerm(SearchTerm term)
        {
            if (term == null)
            {
                throw new ArgumentException("Search term is required");
            }

            currentUser = await currentUserService.GetCurrentUserAsync();

            SearchResult result = new SearchResult();
            result.Term = term;

            if(term.Type == SearchEntityType.Repository)
                await SearchReposByTerm(term, result);
            else if (term.Type == SearchEntityType.Folder)
                await SearchFoldersByTerm(term, result);
            else if (term.Type == SearchEntityType.Document)
                await SearchDocumentsByTerm(term, result);

            return result;
        }

        private async Task SearchReposByTerm(SearchTerm term, SearchResult result)
        {
            var query = context.RepositoryMetaData
                .Include(x => x.Repository)
                .Where(x => currentUser != null && (currentUser.IsAdmin || currentUser.Repositories.Contains(x.RepositoryId)))
                .AsQueryable();

            if (term.MetaTerms != null)
            {
                foreach (var cond in term.MetaTerms)
                {
                    if (cond.Operation == MetaSearchOperation.EQ)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && x.Value == cond.Value
                            );
                    }
                    else if (cond.Operation == MetaSearchOperation.NE)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && x.Value != cond.Value
                            );
                    }
                    else if (cond.Operation == MetaSearchOperation.Like)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && EF.Functions.Like(x.Value, cond.Value)
                            );
                    }
                    else if (cond.Operation == MetaSearchOperation.Unlike)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && !EF.Functions.Like(x.Value, cond.Value)
                            );
                    }

                }
            }

            if (term.Name != null)
            {
                query = query.Where
                    (x =>
                    EF.Functions.Like(x.Repository.Name, term.Name)
                    ||
                    (term.IncludeTitle && EF.Functions.Like(x.Repository.Title, term.Name))
                    ||
                    (term.IncludeDescription && EF.Functions.Like(x.Repository.Description, term.Name))
                    );
            }

            var list = await query.Select(x => x.Repository)
                .Distinct()
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            result.Results.AddRange(list.Select(x => new SearchResultModel
            {
                Id = x.Id,
                Name = x.Name,
                TypeName = "Repository"
            }));
        }

        private async Task SearchFoldersByTerm(SearchTerm term, SearchResult result)
        {

            var query = context.FolderMetaData
                .Include(x => x.Folder)
                .Where(x => currentUser != null && (currentUser.IsAdmin || currentUser.Repositories.Contains(x.Folder.RepositoryId)))
                .AsQueryable();

            if (term.MetaTerms != null)
            {
                foreach (var cond in term.MetaTerms)
                {
                    if (cond.Operation == MetaSearchOperation.EQ)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && x.Value == cond.Value
                            );
                    }
                    else if (cond.Operation == MetaSearchOperation.NE)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && x.Value != cond.Value
                            );
                    }
                    else if (cond.Operation == MetaSearchOperation.Like)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && EF.Functions.Like(x.Value, cond.Value)
                            );
                    }
                    else if (cond.Operation == MetaSearchOperation.Unlike)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && !EF.Functions.Like(x.Value, cond.Value)
                            );
                    }

                }
            }

            if (term.Name != null)
            {
                query = query.Where
                    (x =>
                    EF.Functions.Like(x.Folder.Name, term.Name)
                    ||
                    (term.IncludeTitle && EF.Functions.Like(x.Folder.Title, term.Name))
                    ||
                    (term.IncludeDescription && EF.Functions.Like(x.Folder.Description, term.Name))
                    );
            }

            var list = await query.Select(x => x.Folder)
                .Distinct()
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            result.Results.AddRange(list.Select(x => new SearchResultModel
            {
                Id = x.Id,
                Name = x.Name,
                TypeName = "Folder"
            }));
        }

        private async Task SearchDocumentsByTerm(SearchTerm term, SearchResult result)
        {
            var query = context.DocumentMetaData
                .Include(x => x.Document)
                .Where(x => currentUser != null && (currentUser.IsAdmin || currentUser.Repositories.Contains(x.Document.RepositoryId)))
                .AsQueryable();

            if (term.MetaTerms != null)
            {
                foreach (var cond in term.MetaTerms)
                {
                    if (cond.Operation == MetaSearchOperation.EQ)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && x.Value == cond.Value
                            );
                    }
                    else if (cond.Operation == MetaSearchOperation.NE)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && x.Value != cond.Value
                            );
                    }
                    else if (cond.Operation == MetaSearchOperation.Like)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && EF.Functions.Like(x.Value, cond.Value)
                            );
                    }
                    else if (cond.Operation == MetaSearchOperation.Unlike)
                    {
                        query = query.Where
                            (x =>
                                x.Field.Name == cond.Name && !EF.Functions.Like(x.Value, cond.Value)
                            );
                    }

                }
            }

            if (term.Name != null)
            {
                query = query.Where
                    (x =>
                    EF.Functions.Like(x.Document.Name, term.Name)
                    ||
                    (term.IncludeTitle && EF.Functions.Like(x.Document.Title, term.Name))
                    ||
                    (term.IncludeDescription && EF.Functions.Like(x.Document.Description, term.Name))
                    );
            }

            var list = await query.Select(x => x.Document)
                .Distinct()
                .AsNoTracking()
                .ToListAsync()
                .ConfigureAwait(false);

            result.Results.AddRange(list.Select(x => new SearchResultModel
            {
                Id = x.Id,
                Name = x.Name,
                TypeName = "Document"
            }));
        }
    }
}
