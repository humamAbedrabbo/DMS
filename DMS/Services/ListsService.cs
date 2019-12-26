using DAS.Data;
using DAS.Models;
using DAS.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAS.Services
{
    public class ListsService : IListsService
    {
        private readonly DasContext context;
        private readonly IConfiguration configuration;

        public ListsService(DasContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        public async Task<IEnumerable<MetaField>> GetMetaFieldsList()
        {
            try
            {
                var list = await context.MetaFields
                    .AsNoTracking()
                    .ToListAsync().ConfigureAwait(false);

                return list;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<MetaField> GetMetaFieldById(int id)
        {
            try
            {
                var field = await context.MetaFields.FindAsync(id).ConfigureAwait(false);

                if (field == null)
                {
                    throw new Exception($"Field with id='{id}' not found");
                }

                return field;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IEnumerable<TreeModel>> GetTree(int repoId, int? folderId)
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
                throw;
            }
        }

        public async Task<IEnumerable<TreeModel>> GetRepositoryFoldersTree(int repoId)
        {
            var list = await context.Folders
                .Include(x => x.MetaData).ThenInclude(y => y.Field)
                .Where(x => x.RepositoryId == repoId)
                .OrderBy(x => x.ParentId)
                .AsNoTracking()
                .ToListAsync();

            var nodes = new List<TreeModel>();
            foreach (var x in list)
            {
                var model = new TreeModel
                {
                    Id = x.Id,
                    RepositoryId = x.RepositoryId,
                    Name = x.Name,
                    Title = x.Title,
                    Type = TreeNodeType.Folder,
                    ParentId = x.ParentId

                };
                if(x.MetaData != null)
                {
                    model.Meta = x.MetaData.ToDictionary(a => a.Field.Name, b => b.Value);
                }
                nodes.Add(model);
            }

            foreach (var node in nodes)
            {
                node.Childs = nodes.Where(x => x.ParentId == node.Id).ToList();
            }

            return nodes.Where(x => !x.ParentId.HasValue).ToList();
        }

        public async Task<IEnumerable<RepoDetailModel>> GetRepositoryList()
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
                    Path = repo.Path,
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
                throw;
            }
        }

        public async Task<RepoDetailModel> GetRepositoryById(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("Repository Not Found");
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
                    throw new Exception($"Repository '{id}' not found");
                }

                var dModel = new RepoDetailModel
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    Title = repo.Title,
                    Description = repo.Description,
                    Storage = repo.StorageType,
                    Path = repo.Path,
                    CreatedBy = repo.CreatedBy,
                    CreatedOn = repo.CreatedOn,
                    UpdatedBy = repo.UpdatedBy,
                    UpdatedOn = repo.UpdatedOn,
                    Meta = repo.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value)
                };

                return dModel;
            }
            catch 
            {

                throw;
            }
        }

        public async Task<IEnumerable<FolderDetailModel>> GetFoldersList(string repoId, int? parentId)
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
                    throw new Exception($"Repository '{repoId}' not found");
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
                throw;
            }
        }

        public async Task<FolderDetailModel> GetFolderById(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    throw new Exception("Folder not found");
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
                    throw new Exception($"Folder with id='{id}' not found");
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
                    UpdatedOn = folder.UpdatedOn
                    
                };

                dModel.Meta = folder.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value);
                return dModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<IEnumerable<DocumentDetailModel>> GetDocumentsList(string repoId, int? parentId)
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
                    throw new Exception($"Repository '{repoId}' not found");
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
                throw;
            }
        }

        public async Task<DocumentDetailModel> GetDocumentById(int? id, bool withHistory = false)
        {
            try
            {
                if (!id.HasValue)
                {
                    throw new Exception("Document not found");
                }

                var document = await context.Documents
                        .Include(x => x.MetaData).ThenInclude(m => m.Field)
                        .Include(x => x.Repository)
                        .Include(x => x.Parent)
                        .Include(x => x.History)
                        .Where(x => x.Id == id)
                        .AsNoTracking()
                        .FirstOrDefaultAsync().ConfigureAwait(false);

                if (document == null)
                {
                    throw new Exception($"Document with id='{id}' not found");
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
                    CheckInKey = document.CheckInKey,
                    Meta = document.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value),
                    Thumbnail = thumbnail?.Base64Image

                };

                if(withHistory)
                {
                    dModel.History = document.History.Select(x => new DocumentHistoryDetailModel
                    {
                        Id = x.Id,
                        Operation = x.Operation,
                        OperationBy = x.OperationBy,
                        OperationOn = x.OperationOn,
                        DocumentId = x.DocumentId,
                        Version = x.Version
                    });
                }

                return dModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<FolderBreadcrumbModel> GetFolderBreadcrumb(int folderId)
        {
            var model = new FolderBreadcrumbModel();
            int? parentId = null;
            StringBuilder sb = new StringBuilder();

            var f = await context.Folders.FindAsync(folderId).ConfigureAwait(false);

            if (f != null)
            {
                model.Id = f.Id;
                model.Name = f.Name;
                model.Title = f.Title;
                model.RepositoryId = f.RepositoryId;
                model.ParentId = f.ParentId;

                parentId = f.ParentId;
                while (parentId.HasValue)
                {
                    f = await context.Folders.FindAsync(parentId).ConfigureAwait(false);
                    if (f == null)
                        break;

                    sb.Append($"{f.Name}/");
                    parentId = f.ParentId;
                }
            }
            model.Path = sb.ToString();
            return model;
        }
    }
}
