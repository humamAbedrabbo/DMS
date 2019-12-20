using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DAS.Data;
using DAS.Models;
using DAS.Utils;
using DAS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DAS.Controllers
{
    public class AdminController : Controller
    {
        private readonly DasContext context;
        private readonly IConfiguration configuration;

        public AdminController(DasContext context, IConfiguration configuration)
        {
            this.context = context;
            this.configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult<MetaField>> PostMetaField(MetaField model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new ArgumentException("Field name is required");
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    model.Title = model.Name;
                }

                var exist = await context.MetaFields
                    .Where(x => x.Name == model.Name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (exist != null)
                {
                    return BadRequest($"Field with name='{model.Name}' is already existed");
                }

                context.MetaFields.Add(model);
                await context.SaveChangesAsync().ConfigureAwait(false);

                return model;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutMetaField(int? id, MetaField model)
        {
            try
            {
                if (!id.HasValue)
                {
                    throw new ArgumentException("Invalid field id");
                }

                if (model == null)
                {
                    throw new ArgumentException("Invalid field model");
                }

                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new ArgumentException("Field name is required");
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    model.Title = model.Name;
                }

                var exist = await context.MetaFields
                    .FindAsync(id.Value)
                    .ConfigureAwait(false);

                if (exist == null)
                {
                    return NotFound($"Field with id='{id}' not found");
                }


                exist.Title = model.Title;

                await context.SaveChangesAsync().ConfigureAwait(false);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        [HttpDelete]
        public async Task<ActionResult<MetaField>> DeleteMetaField(int? id, string force = null)
        {
            try
            {
                if (!id.HasValue)
                {
                    throw new ArgumentException("Invalid field id");
                }

                var exist = await context.MetaFields
                    .FindAsync(id.Value)
                    .ConfigureAwait(false);

                if (exist == null)
                {
                    return NotFound($"Field with id='{id}' not found");
                }

                if ((force ?? "").ToLower() == "force")
                {
                    var repoFields = await context.RepositoryMetaData
                        .Where(x => x.FieldId == id)
                        .ToListAsync()
                        .ConfigureAwait(false);
                    context.RepositoryMetaData.RemoveRange(repoFields);

                    var folderFields = await context.FolderMetaData
                        .Where(x => x.FieldId == id)
                        .ToListAsync()
                        .ConfigureAwait(false);
                    context.FolderMetaData.RemoveRange(folderFields);

                    var docFields = await context.DocumentMetaData
                        .Where(x => x.FieldId == id)
                        .ToListAsync()
                        .ConfigureAwait(false);
                    context.DocumentMetaData.RemoveRange(docFields);
                }

                context.Remove(exist);

                await context.SaveChangesAsync().ConfigureAwait(false);

                return exist;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }

        [HttpPost]
        public async Task<ActionResult<RepositoryMeta>> AddRepoMetaValue(int id, string name, string value = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Meta field name is required");
                }

                var field = await context.MetaFields
                    .Where(x => x.Name == name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (field == null)
                {
                    field = new MetaField
                    {
                        Name = name,
                        Title = name
                    };
                    context.MetaFields.Add(field);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                var repo = await context.Repositories
                    .Include(x => x.MetaData).ThenInclude(x => x.Field)
                    .Where(x => x.Id == id)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (repo == null)
                {
                    return NotFound($"Repository with id='{id}' not found");
                }

                if (repo.MetaData == null)
                {
                    repo.MetaData = new List<RepositoryMeta>();
                }

                RepositoryMeta meta = repo.MetaData.FirstOrDefault(x => x.FieldId == field.Id);
                if (meta == null)
                {
                    meta = new RepositoryMeta
                    {
                        FieldId = field.Id,
                        Value = value
                    };
                    repo.MetaData.Add(meta);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    throw new Exception($"Repository '{repo.Name}' already have field '{name}' with value '{meta.Value}'");
                }



                var childFolders = await context.Folders
                    .Where(x => x.RepositoryId == repo.Id || !x.ParentId.HasValue)
                    .Select(x => x.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (var child in childFolders)
                {
                    await DasContext.AddMetaToFolder(context, child, meta.FieldId).ConfigureAwait(false);
                }

                var childDocuments = await context.Documents
               .Where(x => x.RepositoryId == repo.Id && !x.ParentId.HasValue)
               .Select(x => x.Id)
               .ToListAsync()
               .ConfigureAwait(false);

                foreach (var child in childDocuments)
                {
                    await DasContext.AddMetaToDocument(context, child, meta.FieldId).ConfigureAwait(false);
                }

                return meta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<FolderMeta>> AddFolderMetaValue(int id, string name, string value = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Meta field name is required");
                }

                var field = await context.MetaFields
                    .Where(x => x.Name == name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (field == null)
                {
                    field = new MetaField
                    {
                        Name = name,
                        Title = name
                    };
                    context.MetaFields.Add(field);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                var folder = await context.Folders
                    .Include(x => x.MetaData).ThenInclude(x => x.Field)
                    .Where(x => x.Id == id)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (folder == null)
                {
                    return NotFound($"Folder with id='{id}' not found");
                }

                if (folder.MetaData == null)
                {
                    folder.MetaData = new List<FolderMeta>();
                }

                FolderMeta meta = folder.MetaData.FirstOrDefault(x => x.FieldId == field.Id);
                if (meta == null)
                {
                    meta = new FolderMeta
                    {
                        FieldId = field.Id,
                        Value = value
                    };
                    folder.MetaData.Add(meta);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    throw new Exception($"Folder '{folder.Name}' already have field '{name}' with value '{meta.Value}'");
                }

                var childFolders = await context.Folders
                    .Where(x => x.ParentId == folder.Id)
                    .Select(x => x.Id)
                    .ToListAsync()
                    .ConfigureAwait(false);

                foreach (var child in childFolders)
                {
                    await DasContext.AddMetaToFolder(context, child, meta.FieldId).ConfigureAwait(false);
                }

                var childDocuments = await context.Documents
                .Where(x => x.ParentId == folder.Id)
                .Select(x => x.Id)
                .ToListAsync()
                .ConfigureAwait(false);

                foreach (var child in childDocuments)
                {
                    await DasContext.AddMetaToDocument(context, child, meta.FieldId).ConfigureAwait(false);
                }

                return meta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<DocumentMeta>> AddDocumentMetaValue(int id, string name, string value = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Meta field name is required");
                }

                var field = await context.MetaFields
                    .Where(x => x.Name == name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (field == null)
                {
                    field = new MetaField
                    {
                        Name = name,
                        Title = name
                    };
                    context.MetaFields.Add(field);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                var doc = await context.Documents
                    .Include(x => x.MetaData).ThenInclude(x => x.Field)
                    .Where(x => x.Id == id)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (doc == null)
                {
                    return NotFound($"Document with id='{id}' not found");
                }

                if (doc.MetaData == null)
                {
                    doc.MetaData = new List<DocumentMeta>();
                }

                DocumentMeta meta = doc.MetaData.FirstOrDefault(x => x.FieldId == field.Id);
                if (meta == null)
                {
                    meta = new DocumentMeta
                    {
                        FieldId = field.Id,
                        Value = value
                    };
                    doc.MetaData.Add(meta);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }
                else
                {
                    throw new Exception($"Document '{doc.Name}' already have field '{name}' with value '{meta.Value}'");
                }

                return meta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<RepositoryMeta>> SetRepoMetaValue(int id, string name, string value = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Meta field name is required");
                }

                var field = await context.MetaFields
                    .Where(x => x.Name == name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (field == null)
                {
                    field = new MetaField
                    {
                        Name = name,
                        Title = name
                    };
                    context.MetaFields.Add(field);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                var repo = await context.Repositories
                    .Include(x => x.MetaData).ThenInclude(x => x.Field)
                    .Where(x => x.Id == id)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (repo == null)
                {
                    return NotFound($"Repository with id='{id}' not found");
                }

                if (repo.MetaData == null)
                {
                    repo.MetaData = new List<RepositoryMeta>();
                }

                RepositoryMeta meta = repo.MetaData.FirstOrDefault(x => x.FieldId == field.Id);
                if (meta == null)
                {
                    meta = new RepositoryMeta
                    {
                        FieldId = field.Id,
                        Value = value
                    };
                    repo.MetaData.Add(meta);
                }
                else
                {
                    meta.Value = value;
                }
                await context.SaveChangesAsync().ConfigureAwait(false);

                return meta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<FolderMeta>> SetFolderMetaValue(int id, string name, string value = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Meta field name is required");
                }

                var field = await context.MetaFields
                    .Where(x => x.Name == name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (field == null)
                {
                    field = new MetaField
                    {
                        Name = name,
                        Title = name
                    };
                    context.MetaFields.Add(field);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                var folder = await context.Folders
                    .Include(x => x.MetaData).ThenInclude(x => x.Field)
                    .Where(x => x.Id == id)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (folder == null)
                {
                    return NotFound($"Folder with id='{id}' not found");
                }

                if (folder.MetaData == null)
                {
                    folder.MetaData = new List<FolderMeta>();
                }

                FolderMeta meta = folder.MetaData.FirstOrDefault(x => x.FieldId == field.Id);
                if (meta == null)
                {
                    meta = new FolderMeta
                    {
                        FieldId = field.Id,
                        Value = value
                    };
                    folder.MetaData.Add(meta);
                }
                else
                {
                    meta.Value = value;
                }
                await context.SaveChangesAsync().ConfigureAwait(false);

                return meta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult<DocumentMeta>> SetDocumentMetaValue(int id, string name, string value = null)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Meta field name is required");
                }

                var field = await context.MetaFields
                    .Where(x => x.Name == name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (field == null)
                {
                    field = new MetaField
                    {
                        Name = name,
                        Title = name
                    };
                    context.MetaFields.Add(field);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                var doc = await context.Documents
                    .Include(x => x.MetaData).ThenInclude(x => x.Field)
                    .Where(x => x.Id == id)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (doc == null)
                {
                    return NotFound($"Document with id='{id}' not found");
                }

                if (doc.MetaData == null)
                {
                    doc.MetaData = new List<DocumentMeta>();
                }

                DocumentMeta meta = doc.MetaData.FirstOrDefault(x => x.FieldId == field.Id);
                if (meta == null)
                {
                    meta = new DocumentMeta
                    {
                        FieldId = field.Id,
                        Value = value
                    };
                    doc.MetaData.Add(meta);
                }
                else
                {
                    meta.Value = value;
                }
                await context.SaveChangesAsync().ConfigureAwait(false);

                return meta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<RepositoryMeta>> DeleteRepoMetaValue(int id, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Meta field name is required");
                }

                var meta = await context.RepositoryMetaData
                    .Where(x => x.RepositoryId == id && x.Field.Name == name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (meta != null)
                {
                    context.RepositoryMetaData.Remove(meta);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                return meta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<FolderMeta>> DeleteFolderMetaValue(int id, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Meta field name is required");
                }

                var meta = await context.FolderMetaData
                    .Where(x => x.FolderId == id && x.Field.Name == name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (meta != null)
                {
                    context.FolderMetaData.Remove(meta);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                return meta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<DocumentMeta>> DeleteDocumentMetaValue(int id, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                {
                    throw new ArgumentException("Meta field name is required");
                }

                var meta = await context.DocumentMetaData
                    .Where(x => x.DocumentId == id && x.Field.Name == name)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (meta != null)
                {
                    context.DocumentMetaData.Remove(meta);
                    await context.SaveChangesAsync().ConfigureAwait(false);
                }

                return meta;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<RepoDetailModel>> PostRepository(RepoAddModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new ArgumentException("Repository name is required");
                }

                if (int.TryParse(model.Name, out int idNum))
                {
                    throw new ArgumentException("Repository name must have alphabet characters");
                }

                if (model.Name.Contains('"') || model.Name.Contains('\'')
                    || model.Name.Contains(',') || model.Name.Contains(';')
                    || model.Name.Contains('\\') || model.Name.Contains('/')
                    || model.Name.Contains('.') || model.Name.Contains('|')
                    || model.Name.Contains('&') || model.Name.Contains('$'))
                {
                    throw new ArgumentException("Repository name must not include \" \\ / , . ; $ & | or ' characters");
                }

                model.Name = model.Name.Trim().ToLower().Replace(' ', '-');

                if (string.IsNullOrEmpty(model.UserName))
                {
                    throw new ArgumentException("User name is required");
                }

                if (model.Storage == StorageType.Directory)
                {
                    if (string.IsNullOrEmpty(model.Path))
                    {
                        model.Path = AppSettingsProvider.DefaultUploadFolder;
                    }
                    else
                    {
                        if (!Directory.Exists(model.Path))
                        {
                            Directory.CreateDirectory(model.Path);
                        }
                    }
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    model.Title = model.Name;
                }

                var exist = await context.Repositories.CountAsync(r => r.Name == model.Name).ConfigureAwait(false);

                if (exist > 0)
                {
                    throw new ArgumentException("Repository name already used");
                }

                var date = DateTime.Now;

                var repo = new Repository()
                {
                    Name = model.Name,
                    Title = model.Title,
                    Description = model.Description,
                    StorageType = model.Storage,
                    Path = model.Path,
                    CreatedBy = model.UserName,
                    CreatedOn = date,
                    UpdatedBy = model.UserName,
                    UpdatedOn = date,
                    IsDeleted = false
                };

                if (model.Meta != null && model.Meta.Count > 0)
                {
                    var metaFields = await context.MetaFields.Where(mf => model.Meta.Keys.Contains(mf.Name)).ToListAsync().ConfigureAwait(false);
                    var newFields = new List<MetaField>();
                    var newFieldNames = model.Meta.Keys.Except(metaFields.Select(mf => mf.Name));

                    foreach (string newFieldName in newFieldNames)
                    {
                        var newField = new MetaField
                        {
                            Name = newFieldName,
                            Title = newFieldName,
                        };

                        newFields.Add(newField);
                    }

                    context.MetaFields.AddRange(newFields);
                    await context.SaveChangesAsync().ConfigureAwait(false);

                    var agList = metaFields.Union(newFields);

                    if (repo.MetaData == null)
                    {
                        repo.MetaData = new List<RepositoryMeta>();
                    }

                    foreach (var field in agList)
                    {
                        var repoMeta = new RepositoryMeta
                        {
                            FieldId = field.Id,
                            Value = model.Meta[field.Name]
                        };

                        repo.MetaData.Add(repoMeta);
                    }
                }

                context.Repositories.Add(repo);
                await context.SaveChangesAsync().ConfigureAwait(false);

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

                return CreatedAtAction("GetRepositoryById", "Lists", new { id = repo.Name }, dModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<ActionResult> PutRepository(string id, RepoUpdateModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new ArgumentException("Must provide a repository id or name");
                }

                if (model == null)
                {
                    throw new ArgumentException("Must provide a repository updated model");
                }

                if (string.IsNullOrEmpty(model.UserName))
                {
                    throw new ArgumentException("User name is required");
                }

                Repository repo = null;

                if (int.TryParse(id, out int idNum))
                {
                    repo = await context.Repositories
                    .Include(x => x.MetaData).ThenInclude(m => m.Field)
                    .Where(x => x.Id == idNum)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                }
                else
                {
                    id = id.ToLower();

                    repo = await context.Repositories
                    .Include(x => x.MetaData).ThenInclude(m => m.Field)
                    .Where(x => x.Name == id)
                    .FirstOrDefaultAsync().ConfigureAwait(false);
                }

                if (repo == null)
                {
                    return NotFound($"Repository '{id}' not found");
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    repo.Title = repo.Name;
                }
                else
                {
                    repo.Title = model.Title;
                }

                repo.Description = model.Description;

                if (model.Meta != null && model.Meta.Count > 0)
                {
                    var metaFields = await context.MetaFields.Where(mf => model.Meta.Keys.Contains(mf.Name)).ToListAsync().ConfigureAwait(false);
                    var newFields = new List<MetaField>();
                    var newFieldNames = model.Meta.Keys.Except(metaFields.Select(mf => mf.Name));

                    foreach (string newFieldName in newFieldNames)
                    {
                        var newField = new MetaField
                        {
                            Name = newFieldName,
                            Title = newFieldName,
                        };

                        newFields.Add(newField);
                    }

                    context.MetaFields.AddRange(newFields);
                    await context.SaveChangesAsync().ConfigureAwait(false);

                    var agList = metaFields.Union(newFields);

                    if (repo.MetaData == null)
                    {
                        repo.MetaData = new List<RepositoryMeta>();
                    }

                    foreach (var field in agList)
                    {
                        var meta = repo.MetaData.FirstOrDefault(m => m.Field.Name == field.Name);
                        if (meta != null)
                        {
                            meta.Value = model.Meta[field.Name];
                        }
                        else
                        {
                            var repoMeta = new RepositoryMeta
                            {
                                FieldId = field.Id,
                                Value = model.Meta[field.Name]
                            };

                            repo.MetaData.Add(repoMeta);
                        }
                    }
                }

                repo.UpdatedBy = model.UserName;
                repo.UpdatedOn = DateTime.Now;

                await context.SaveChangesAsync().ConfigureAwait(false);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<RepoDetailModel>> DeleteRepository(int? id, [FromHeader] string username)
        {
            try
            {
                if (!id.HasValue)
                {
                    throw new ArgumentException("Must provide a repository id");
                }

                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("User name is required");
                }

                var repo = await context.Repositories
                    .Include(x => x.MetaData).ThenInclude(m => m.Field)
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync().ConfigureAwait(false);

                if (repo == null)
                {
                    return NotFound($"Repository with id={id} not found");
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

                repo.IsDeleted = true;
                await context.SaveChangesAsync().ConfigureAwait(false);

                return CreatedAtAction("GetRepositoryById", "Lists", new { id = repo.Id }, dModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<FolderDetailModel>> DeleteFolder(int? id, [FromHeader] string username)
        {
            try
            {
                if (!id.HasValue)
                {
                    throw new ArgumentException("Must provide a folder id");
                }

                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("User name is required");
                }

                var folder = await context.Folders
                    .Include(x => x.MetaData).ThenInclude(m => m.Field)
                    .Include(x => x.Repository)
                    .Include(x => x.Parent)
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync().ConfigureAwait(false);

                if (folder == null)
                {
                    return NotFound($"Folder with id={id} not found");
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

                folder.IsDeleted = true;
                await context.SaveChangesAsync().ConfigureAwait(false);

                return CreatedAtAction("GetFolderById", "Lists", new { id = folder.Id }, dModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<ActionResult<DocumentDetailModel>> DeleteDocument(int? id, [FromHeader] string username)
        {
            try
            {
                if (!id.HasValue)
                {
                    throw new ArgumentException("Must provide a document id");
                }

                if (string.IsNullOrEmpty(username))
                {
                    throw new ArgumentException("User name is required");
                }

                var document = await context.Documents
                    .Include(x => x.MetaData).ThenInclude(m => m.Field)
                    .Include(x => x.Repository)
                    .Include(x => x.Parent)
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync().ConfigureAwait(false);

                if (document == null)
                {
                    return NotFound($"Document with id={id} not found");
                }

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
                    CheckInKey = document.CheckInKey,
                    Length = document.Length,
                    CreatedBy = document.CreatedBy,
                    CreatedOn = document.CreatedOn,
                    UpdatedBy = document.UpdatedBy,
                    UpdatedOn = document.UpdatedOn,
                    Meta = document.MetaData?.ToDictionary(k => k.Field.Name, v => v.Value)
                };

                document.IsDeleted = true;
                await context.SaveChangesAsync().ConfigureAwait(false);

                return CreatedAtAction("GetDocumentById", "Lists", new { id = document.Id }, dModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
