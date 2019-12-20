using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAS.Data;
using DAS.Models;
using DAS.Services;
using DAS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAS.Controllers
{
    public class SearchController : Controller
    {
        private readonly DasContext context;
        private readonly ISearchService searchService;

        public SearchController(DasContext context, ISearchService searchService)
        {
            this.context = context;
            this.searchService = searchService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SearchTerm(SearchTerm term)
        {
            try
            {
                
                var results = new List<SearchResult>();
                if(term.MetaTerms != null)
                {
                    term.MetaTerms.RemoveAll(x => string.IsNullOrEmpty(x.Name));
                }

                results.Add(await searchService.SearchByTerm(term).ConfigureAwait(false));

                return View(results);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepoDetailModel>>> FindRepositoriesBy(string field, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(field))
                {
                    return NotFound();
                }

                List<Repository> list = new List<Repository>();

                var query = context.RepositoryMetaData
                    .Include(x => x.Field)
                    .Include(x => x.Repository)
                    .AsNoTracking()
                    .AsQueryable();

                switch (field.ToLower())
                {
                    case "id":
                        if (int.TryParse(value, out int num))
                        {
                            query = query.Where(x => x.RepositoryId == num);
                        }
                        else
                        {
                            query = query.Where(x => x.RepositoryId == 0);
                        }
                        break;
                    case "name":
                    case "title":
                        query = query.Where(x => EF.Functions.Like(x.Repository.Name, $"%{value}%")
                            || EF.Functions.Like(x.Repository.Title, $"%{value}%")
                        );
                        break;
                    case "description":
                        query = query.Where(x => EF.Functions.Like(x.Repository.Description, $"%{value}%"));
                        break;
                    default:
                        query = query.Where(x => EF.Functions.Like(x.Value, $"%{value}%"));
                        break;
                }

                list = await query.Select(x => x.Repository).Distinct().ToListAsync().ConfigureAwait(false);
                var result = new List<RepoDetailModel>();
                foreach (var item in list)
                {
                    result.Add(new RepoDetailModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Title = item.Title,
                        Storage = item.StorageType,
                        Description = item.Description,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = item.CreatedOn,
                        UpdatedBy = item.UpdatedBy,
                        UpdatedOn = item.UpdatedOn,
                        Meta = item.MetaData?.ToDictionary(x => x.Field.Name, y => y.Value)
                    });
                }

                return result;
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FolderDetailModel>>> FindFoldersBy(string field, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(field))
                {
                    return NotFound();
                }

                List<Folder> list = new List<Folder>();

                var query = context.FolderMetaData
                    .Include(x => x.Field)
                    .Include(x => x.Folder).ThenInclude(x => x.Repository)
                    .Include(x => x.Folder).ThenInclude(x => x.Parent)
                    .AsNoTracking()
                    .AsQueryable();

                switch (field.ToLower())
                {
                    case "id":
                        if (int.TryParse(value, out int num))
                        {
                            query = query.Where(x => x.FolderId == num);
                        }
                        else
                        {
                            query = query.Where(x => x.FolderId == 0);
                        }
                        break;
                    case "repositoryId":
                        if (int.TryParse(value, out int rid))
                        {
                            query = query.Where(x => x.Folder.RepositoryId == rid);
                        }
                        else
                        {
                            query = query.Where(x => x.Folder.RepositoryId == 0);
                        }
                        break;
                    case "parentId":
                        if (int.TryParse(value, out int pid))
                        {
                            query = query.Where(x => x.Folder.ParentId == pid);
                        }
                        else
                        {
                            query = query.Where(x => x.Folder.ParentId == 0);
                        }
                        break;
                    case "parent":
                        query = query.Where(x => x.Folder.ParentId.HasValue
                        && (EF.Functions.Like(x.Folder.Parent.Name, $"%{value}%") || EF.Functions.Like(x.Folder.Parent.Title, $"%{value}%"))
                        );
                        break;
                    case "name":
                    case "title":
                        query = query.Where(x => EF.Functions.Like(x.Folder.Name, $"%{value}%")
                            || EF.Functions.Like(x.Folder.Title, $"%{value}%")
                        );
                        break;
                    case "description":
                        query = query.Where(x => EF.Functions.Like(x.Folder.Description, $"%{value}%"));
                        break;
                    default:
                        query = query.Where(x => EF.Functions.Like(x.Value, $"%{value}%"));
                        break;
                }

                list = await query.Select(x => x.Folder).Distinct().ToListAsync().ConfigureAwait(false);
                var result = new List<FolderDetailModel>();
                foreach (var item in list)
                {
                    result.Add(new FolderDetailModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Title = item.Title,
                        RepositoryId = item.RepositoryId,
                        Repository = item.Repository.Name,
                        ParentId = item.ParentId,
                        Parent = item.Parent?.Name,
                        Description = item.Description,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = item.CreatedOn,
                        UpdatedBy = item.UpdatedBy,
                        UpdatedOn = item.UpdatedOn,
                        Meta = item.MetaData?.ToDictionary(x => x.Field.Name, y => y.Value)
                    });
                }

                return result;
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDetailModel>>> FindDocumentsBy(string field, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(field))
                {
                    return NotFound();
                }

                List<Document> list = new List<Document>();

                var query = context.DocumentMetaData
                    .Include(x => x.Field)
                    .Include(x => x.Document).ThenInclude(x => x.Repository)
                    .Include(x => x.Document).ThenInclude(x => x.Parent)
                    .AsNoTracking()
                    .AsQueryable();

                switch (field.ToLower())
                {
                    case "id":
                        if (int.TryParse(value, out int num))
                        {
                            query = query.Where(x => x.DocumentId == num);
                        }
                        else
                        {
                            query = query.Where(x => x.DocumentId == 0);
                        }
                        break;
                    case "repositoryId":
                        if (int.TryParse(value, out int rid))
                        {
                            query = query.Where(x => x.Document.RepositoryId == rid);
                        }
                        else
                        {
                            query = query.Where(x => x.Document.RepositoryId == 0);
                        }
                        break;
                    case "parentId":
                        if (int.TryParse(value, out int pid))
                        {
                            query = query.Where(x => x.Document.ParentId == pid);
                        }
                        else
                        {
                            query = query.Where(x => x.Document.ParentId == 0);
                        }
                        break;
                    case "parent":
                        query = query.Where(x => x.Document.ParentId.HasValue
                        && (EF.Functions.Like(x.Document.Parent.Name, $"%{value}%") || EF.Functions.Like(x.Document.Parent.Title, $"%{value}%"))
                        );
                        break;
                    case "name":
                    case "title":
                        query = query.Where(x => EF.Functions.Like(x.Document.Name, $"%{value}%")
                            || EF.Functions.Like(x.Document.Title, $"%{value}%")
                        );
                        break;
                    case "description":
                        query = query.Where(x => EF.Functions.Like(x.Document.Description, $"%{value}%"));
                        break;
                    default:
                        query = query.Where(x => EF.Functions.Like(x.Value, $"%{value}%"));
                        break;
                }

                list = await query.Select(x => x.Document).Distinct().ToListAsync().ConfigureAwait(false);
                var result = new List<DocumentDetailModel>();
                foreach (var item in list)
                {
                    result.Add(new DocumentDetailModel
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Title = item.Title,
                        RepositoryId = item.RepositoryId,
                        Repository = item.Repository.Name,
                        ParentId = item.ParentId,
                        Parent = item.Parent?.Name,
                        Description = item.Description,
                        CheckInKey = item.CheckInKey,
                        ContentType = item.ContentType,
                        LastOperation = item.LastOperation,
                        Length = item.Length,
                        OperationBy = item.OperationBy,
                        OperationDate = item.OperationDate,
                        Version = item.Version,
                        CreatedBy = item.CreatedBy,
                        CreatedOn = item.CreatedOn,
                        UpdatedBy = item.UpdatedBy,
                        UpdatedOn = item.UpdatedOn,
                        Meta = item.MetaData?.ToDictionary(x => x.Field.Name, y => y.Value)
                    });
                }

                return result;
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<SearchResult>>> Search(List<SearchTerm> terms)
        {
            try
            {
                if (terms == null || terms.Count == 0)
                    return NotFound();

                var results = new List<SearchResult>();

                foreach (var term in terms)
                {
                    results.Add(await searchService.SearchByTerm(term).ConfigureAwait(false));
                }

                return results;

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

    }
}
