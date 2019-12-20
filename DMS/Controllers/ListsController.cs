using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAS.Data;
using DAS.Models;
using DAS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAS.Controllers
{
    public class ListsController : Controller
    {
        private readonly DasContext context;
        private readonly IConfiguration configuration;

        public ListsController(DasContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MetaField>>> GetMetaFieldsList()
        {
            try
            {
                var list = await context.MetaFields
                    .AsNoTracking()
                    .ToListAsync().ConfigureAwait(false);

                return list;
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<MetaField>> GetMetaFieldById(int id)
        {
            try
            {
                var field = await context.MetaFields.FindAsync(id).ConfigureAwait(false);

                if (field == null)
                {
                    return NotFound($"Field with id='{id}' not found");
                }

                return field;
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TreeModel>>> GetTree(int repoId, int? folderId)
        {
            try
            {
                List<TreeModel> tree = new List<TreeModel>();

                var folders = await context.Folders
                    .Include(x => x.Repository)
                    .Include(x => x.MetaData).ThenInclude(x => x.Field)
                    .Where(x => x.RepositoryId == repoId && x.ParentId == folderId)
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false);

                tree.AddRange(folders.Select(x => new TreeModel
                {
                    Id = x.Id,
                    Type = TreeNodeType.Folder,
                    Name = x.Name,
                    Title = x.Title,
                    RepositoryId = x.RepositoryId,
                    Repository = x.Repository.Name,
                    Meta = x.MetaData.ToDictionary(a => a.Field.Name, b => b.Value)
                }));

                var docs = await context.Documents
                    .Include(x => x.Repository)
                    .Include(x => x.MetaData).ThenInclude(x => x.Field)
                    .Where(x => x.RepositoryId == repoId && x.ParentId == folderId)
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false);

                tree.AddRange(docs.Select(x => new TreeModel
                {
                    Id = x.Id,
                    Type = TreeNodeType.Document,
                    Name = x.Name,
                    Title = x.Title,
                    RepositoryId = x.RepositoryId,
                    Repository = x.Repository.Name,
                    Meta = x.MetaData.ToDictionary(a => a.Field.Name, b => b.Value)
                }));

                return tree;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RepoDetailModel>>> GetRepositoryList()
        {
            try
            {
                var list = await context.Repositories
                    .Include(x => x.MetaData).ThenInclude(m => m.Field)
                    .AsNoTracking()
                    .ToListAsync().ConfigureAwait(false);

                var dList = list.Select(repo => new RepoDetailModel
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    Title = repo.Title,
                    Description = repo.Description,
                    Storage = repo.StorageType,
                    CreatedBy = repo.CreatedBy,
                    CreatedOn = repo.CreatedOn,
                    UpdatedBy = repo.UpdatedBy,
                    UpdatedOn = repo.UpdatedOn,
                    Meta = repo.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value)
                }).ToList();

                return dList;
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public async Task<ActionResult<RepoDetailModel>> GetRepositoryById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return NotFound();
                }

                Repository repo = null;

                if (int.TryParse(id, out int idNum))
                {
                    repo = await context.Repositories
                        .Include(x => x.MetaData).ThenInclude(m => m.Field)
                        .Where(x => x.Id == idNum)
                        .AsNoTracking()
                        .FirstOrDefaultAsync().ConfigureAwait(false);
                }
                else
                {
                    id = id.ToLower();

                    repo = await context.Repositories
                        .Include(x => x.MetaData).ThenInclude(m => m.Field)
                        .Where(x => x.Name == id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync().ConfigureAwait(false);
                }

                if (repo == null)
                {
                    return NotFound($"Repository '{id}' not found");
                }

                var dModel = new RepoDetailModel
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    Title = repo.Title,
                    Description = repo.Description,
                    Storage = repo.StorageType,
                    CreatedBy = repo.CreatedBy,
                    CreatedOn = repo.CreatedOn,
                    UpdatedBy = repo.UpdatedBy,
                    UpdatedOn = repo.UpdatedOn,
                    Meta = repo.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value)
                };

                return dModel;
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FolderDetailModel>>> GetFoldersList(string repoId, int? parentId)
        {
            try
            {
                if (string.IsNullOrEmpty(repoId))
                {
                    throw new ArgumentException("Repository id or name is required");
                }

                int? existId = null;

                if (int.TryParse(repoId, out int idNum))
                {
                    existId = await context.Repositories.Where(x => x.Id == idNum)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    repoId = repoId.ToLower();

                    existId = await context.Repositories.Where(x => x.Name == repoId)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                }

                if (!existId.HasValue || existId == 0)
                {
                    return NotFound($"Repository '{repoId}' not found");
                }

                var list = await context.Folders
                    .Include(x => x.MetaData).ThenInclude(m => m.Field)
                    .Include(x => x.Repository)
                    .Include(x => x.Parent)
                    .Where(x => x.RepositoryId == existId && x.ParentId == parentId)
                    .AsNoTracking()
                    .ToListAsync().ConfigureAwait(false);

                var dList = list.Select(folder => new FolderDetailModel
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    Title = folder.Title,
                    Description = folder.Description,
                    RepositoryId = folder.RepositoryId,
                    Repository = folder.Repository?.Name,
                    ParentId = folder.ParentId,
                    Parent = folder.Parent?.Name,
                    CreatedBy = folder.CreatedBy,
                    CreatedOn = folder.CreatedOn,
                    UpdatedBy = folder.UpdatedBy,
                    UpdatedOn = folder.UpdatedOn,
                    Meta = folder.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value)
                }).ToList();

                return dList;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<FolderDetailModel>> GetFolderById(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return NotFound();
                }

                var folder = await context.Folders
                        .Include(x => x.MetaData).ThenInclude(m => m.Field)
                        .Include(x => x.Repository)
                        .Include(x => x.Parent)
                        .Where(x => x.Id == id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync().ConfigureAwait(false);

                if (folder == null)
                {
                    return NotFound($"Folder with id='{id}' not found");
                }

                var dModel = new FolderDetailModel
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    Title = folder.Title,
                    Description = folder.Description,
                    RepositoryId = folder.RepositoryId,
                    Repository = folder.Repository.Name,
                    ParentId = folder.ParentId,
                    Parent = folder.Parent?.Name,
                    CreatedBy = folder.CreatedBy,
                    CreatedOn = folder.CreatedOn,
                    UpdatedBy = folder.UpdatedBy,
                    UpdatedOn = folder.UpdatedOn,
                    Meta = folder.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value)
                };

                return dModel;
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDetailModel>>> GetDocumentsList(string repoId, int? parentId)
        {
            try
            {
                if (string.IsNullOrEmpty(repoId))
                {
                    throw new ArgumentException("Repository id or name is required");
                }

                int? existId = null;

                if (int.TryParse(repoId, out int idNum))
                {
                    existId = await context.Repositories.Where(x => x.Id == idNum)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    repoId = repoId.ToLower();

                    existId = await context.Repositories.Where(x => x.Name == repoId)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                }

                if (!existId.HasValue || existId == 0)
                {
                    return NotFound($"Repository '{repoId}' not found");
                }

                var list = await context.Documents
                    .Include(x => x.MetaData).ThenInclude(m => m.Field)
                    .Include(x => x.Repository)
                    .Include(x => x.Parent)
                    .Where(x => x.RepositoryId == existId && x.ParentId == parentId)
                    .AsNoTracking()
                    .ToListAsync().ConfigureAwait(false);

                var dList = list.Select(document => new DocumentDetailModel
                {
                    Id = document.Id,
                    Name = document.Name,
                    Title = document.Title,
                    Description = document.Description,
                    RepositoryId = document.RepositoryId,
                    Repository = document.Repository?.Name,
                    ParentId = document.ParentId,
                    Parent = document.Parent?.Name,
                    ContentType = document.ContentType,
                    Version = document.Version,
                    Length = document.Length,
                    LastOperation = document.LastOperation,
                    OperationBy = document.OperationBy,
                    OperationDate = document.OperationDate,
                    CreatedBy = document.CreatedBy,
                    CreatedOn = document.CreatedOn,
                    UpdatedBy = document.UpdatedBy,
                    UpdatedOn = document.UpdatedOn,
                    Meta = document.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value)
                }).ToList();

                return dList;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<DocumentDetailModel>> GetDocumentById(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return NotFound();
                }

                var document = await context.Documents
                        .Include(x => x.MetaData).ThenInclude(m => m.Field)
                        .Include(x => x.Repository)
                        .Include(x => x.Parent)
                        .Where(x => x.Id == id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync().ConfigureAwait(false);

                if (document == null)
                {
                    return NotFound($"Document with id='{id}' not found");
                }

                var thumbnail = await context.DocumentThumbnails
                    .Where(x => x.DocumentId == document.Id)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefaultAsync()
                    .ConfigureAwait(false);

                var dModel = new DocumentDetailModel
                {
                    Id = document.Id,
                    Name = document.Name,
                    Title = document.Title,
                    Description = document.Description,
                    RepositoryId = document.RepositoryId,
                    Repository = document.Repository.Name,
                    ParentId = document.ParentId,
                    Parent = document.Parent?.Name,
                    ContentType = document.ContentType,
                    Version = document.Version,
                    Length = document.Length,
                    LastOperation = document.LastOperation,
                    OperationBy = document.OperationBy,
                    OperationDate = document.OperationDate,
                    CreatedBy = document.CreatedBy,
                    CreatedOn = document.CreatedOn,
                    UpdatedBy = document.UpdatedBy,
                    UpdatedOn = document.UpdatedOn,
                    Meta = document.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value),
                    Thumbnail = thumbnail?.Base64Image
                };

                return dModel;
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
