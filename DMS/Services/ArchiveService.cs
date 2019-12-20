using DAS.Data;
using DAS.Models;
using DAS.Utils;
using DAS.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Services
{
    public class ArchiveService : IArchiveService
    {
        private readonly DasContext context;

        public ArchiveService(DasContext context)
        {
            this.context = context;
        }

        public async Task<FolderDetailModel> AddFolder(FolderAddModel model)
        {
            try
            {
                string repoId = model.RepositoryId;
                int? parentId = model.ParentId;

                if (string.IsNullOrEmpty(repoId))
                {
                    throw new ArgumentException("Repository id or name is required");
                }

                model.RepositoryId = repoId;

                if (parentId.HasValue)
                {
                    model.ParentId = parentId;
                }

                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new ArgumentException("Folder name is required");
                }

                if (model.Name.Contains('"') || model.Name.Contains('\'')
                    || model.Name.Contains(',') || model.Name.Contains(';')
                    || model.Name.Contains('\\') || model.Name.Contains('/')
                    || model.Name.Contains('|') || model.Name.Contains('&')
                    || model.Name.Contains('$') || model.Name == "." || model.Name == "..")
                {
                    throw new ArgumentException("Document name must not equals . or .. and not include \" \\ / , ; $ & | or ' characters");
                }

                if (string.IsNullOrEmpty(model.UserName))
                {
                    throw new ArgumentException("User name is required");
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    model.Title = model.Name;
                }

                Repository repo = null;

                if (int.TryParse(model.RepositoryId, out int idNum))
                {
                    repo = await context.Repositories
                        .Include(x => x.MetaData).ThenInclude(x => x.Field)
                        .Where(r => r.Id == idNum)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    model.RepositoryId = model.RepositoryId.ToLower();

                    repo = await context.Repositories
                        .Include(x => x.MetaData).ThenInclude(x => x.Field)
                        .Where(r => r.Name == model.RepositoryId)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                }

                if (repo == null)
                {
                    throw new Exception($"Repository '{model.RepositoryId}' not found");
                }

                // add repository meta to this folder
                if (model.Meta == null)
                {
                    model.Meta = new Dictionary<string, string>();
                }

                if (repo.MetaData != null)
                {
                    foreach (var item in repo.MetaData)
                    {
                        if (!model.Meta.ContainsKey(item.Field.Name))
                        {
                            model.Meta[item.Field.Name] = null;
                        }
                    }
                }

                Folder parent = null;

                if (model.ParentId.HasValue)
                {
                    parent = await context.Folders
                        .Include(x => x.MetaData).ThenInclude(x => x.Field)
                        .Where(f => f.Id == model.ParentId && f.RepositoryId == repo.Id)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);

                    if (parent == null)
                    {
                        throw new Exception($"Parent folder with id='{model.ParentId}' not found in repository '{repo.Name}'");
                    }
                }

                var exist = await context.Folders.CountAsync(x => x.RepositoryId == repo.Id
                    && x.ParentId == model.ParentId
                    && x.Name == model.Name)
                    .ConfigureAwait(false);

                if (exist > 0)
                {
                    throw new ArgumentException($"Folder name '{model.Name}' is already in use");
                }

                if (parent != null && parent.MetaData != null)
                {
                    foreach (var item in parent.MetaData)
                    {
                        if (!model.Meta.ContainsKey(item.Field.Name))
                        {
                            model.Meta[item.Field.Name] = null;
                        }
                    }
                }

                var date = DateTime.Now;

                var folder = new Folder()
                {
                    Name = model.Name,
                    Title = model.Title,
                    Description = model.Description,
                    Repository = repo,
                    Parent = parent,
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

                    if (folder.MetaData == null)
                    {
                        folder.MetaData = new List<FolderMeta>();
                    }

                    foreach (var field in agList)
                    {
                        var folderMeta = new FolderMeta
                        {
                            FieldId = field.Id,
                            Value = model.Meta[field.Name]
                        };

                        folder.MetaData.Add(folderMeta);
                    }

                }

                if (folder.MetaData == null)
                {
                    folder.MetaData = new List<FolderMeta>();
                }


                folder.CreatedOn = folder.UpdatedOn = DateTime.Now; ;
                repo.UpdatedOn = date;
                context.Folders.Add(folder);
                await context.SaveChangesAsync().ConfigureAwait(false);

                var dModel = new FolderDetailModel
                {
                    Id = folder.Id,
                    Name = folder.Name,
                    Title = folder.Title,
                    Description = folder.Description,
                    RepositoryId = folder.RepositoryId,
                    Repository = repo.Name,
                    ParentId = folder.ParentId,
                    Parent = parent?.Name,
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
                throw;
            }
        }

        public async Task UpdateFolder(int? id, FolderUpdateModel model)
        {
            try
            {
                if (!id.HasValue)
                {
                    throw new ArgumentException("Must provide a folder id");
                }

                if (model == null)
                {
                    throw new ArgumentException("Must provide a folder updated model");
                }

                if (string.IsNullOrEmpty(model.UserName))
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
                    throw new Exception($"Folder '{id}' not found");
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    folder.Title = folder.Name;
                }
                else
                {
                    folder.Title = model.Title;
                }

                folder.Description = model.Description;

                if (folder.ParentId != model.ParentId)
                {
                    if (model.ParentId.HasValue)
                    {
                        var parent = await context.Folders
                            .Include(x => x.MetaData).ThenInclude(m => m.Field)
                            .Where(x => x.Id == model.ParentId && x.RepositoryId == folder.RepositoryId)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

                        if (parent == null)
                        {
                            throw new Exception($"Parent folder '{model.ParentId}' not found in repository '{folder.Repository.Name}'");
                        }

                        folder.Parent = parent;
                    }

                    folder.ParentId = model.ParentId;
                }

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

                    if (folder.MetaData == null)
                    {
                        folder.MetaData = new List<FolderMeta>();
                    }

                    foreach (var field in agList)
                    {
                        var meta = folder.MetaData.FirstOrDefault(m => m.Field.Name == field.Name);
                        if (meta != null)
                        {
                            meta.Value = model.Meta[field.Name];
                        }
                        else
                        {
                            var folderMeta = new FolderMeta
                            {
                                FieldId = field.Id,
                                Value = model.Meta[field.Name]
                            };

                            folder.MetaData.Add(folderMeta);
                        }
                    }
                }

                folder.UpdatedBy = model.UserName;
                folder.UpdatedOn = DateTime.Now;

                await context.SaveChangesAsync().ConfigureAwait(false);


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DocumentDetailModel> AddDocument(DocumentAddModel model)
        {
            try
            {
                string repoId = model.RepositoryId;
                int? parentId = model.ParentId;

                if (string.IsNullOrEmpty(repoId))
                {
                    throw new ArgumentException("Repository id or name is required");
                }

                model.RepositoryId = repoId;

                if (parentId.HasValue)
                {
                    model.ParentId = parentId;
                }

                if (string.IsNullOrEmpty(model.Name))
                {
                    throw new ArgumentException("Document name is required");
                }

                if (model.Name.Contains('"') || model.Name.Contains('\'')
                    || model.Name.Contains(',') || model.Name.Contains(';')
                    || model.Name.Contains('\\') || model.Name.Contains('/')
                    || model.Name.Contains('|') || model.Name.Contains('&')
                    || model.Name.Contains('$') || model.Name == "." || model.Name == "..")
                {
                    throw new ArgumentException("Document name must not equals . or .. and not include \" \\ / , ; $ & | or ' characters");
                }

                if (string.IsNullOrEmpty(model.UserName))
                {
                    throw new ArgumentException("User name is required");
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    model.Title = model.Name;
                }

                Repository repo = null;

                if (int.TryParse(model.RepositoryId, out int idNum))
                {
                    repo = await context.Repositories
                        .Include(x => x.MetaData).ThenInclude(x => x.Field)
                        .Where(r => r.Id == idNum)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    model.RepositoryId = model.RepositoryId.ToLower();

                    repo = await context.Repositories
                        .Include(x => x.MetaData).ThenInclude(x => x.Field)
                        .Where(r => r.Name == model.RepositoryId)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);
                }

                if (repo == null)
                {
                    throw new Exception($"Repository '{model.RepositoryId}' not found");
                }

                if (model.Meta == null)
                {
                    model.Meta = new Dictionary<string, string>();
                }

                if (repo.MetaData != null)
                {
                    foreach (var item in repo.MetaData)
                    {
                        if (!model.Meta.ContainsKey(item.Field.Name))
                        {
                            model.Meta[item.Field.Name] = null;
                        }
                    }
                }

                Folder parent = null;

                if (model.ParentId.HasValue)
                {
                    parent = await context.Folders
                        .Include(x => x.MetaData).ThenInclude(x => x.Field)
                        .Where(f => f.Id == model.ParentId && f.RepositoryId == repo.Id)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);

                    if (parent == null)
                    {
                        throw new Exception($"Parent folder with id='{model.ParentId}' not found in repository '{repo.Name}'");
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.Path))
                    {
                        var folders = model.Path.Split('/');
                        int? folderParent = null;
                        Folder folderParentObj = null;
                        Folder folderToCreate = null;
                        foreach (var folderName in folders)
                        {
                            // Create Path In Database
                            folderToCreate = await context.Folders
                                .Include(x => x.MetaData).ThenInclude(x => x.Field)
                                .Where(x => x.RepositoryId == repo.Id
                                && x.ParentId == folderParent
                                && x.Name == folderName)
                                .SingleOrDefaultAsync()
                                .ConfigureAwait(false);

                            if (folderToCreate == null)
                            {
                                DateTime now = DateTime.Now;
                                folderToCreate = new Folder
                                {
                                    RepositoryId = repo.Id,
                                    ParentId = folderParent,
                                    Name = folderName,
                                    Title = folderName,
                                    CreatedOn = now,
                                    CreatedBy = model.UserName,
                                    UpdatedOn = now,
                                    UpdatedBy = model.UserName,
                                    IsDeleted = false,
                                    MetaData = new List<FolderMeta>()
                                };

                                if (repo.MetaData != null)
                                {
                                    foreach (var item in repo.MetaData)
                                    {
                                        folderToCreate.MetaData.Add(new FolderMeta
                                        {
                                            FieldId = item.FieldId,
                                            Value = null
                                        });
                                    }
                                }

                                if (folderParentObj != null && folderParentObj.MetaData != null)
                                {
                                    foreach (var item in folderParentObj.MetaData)
                                    {
                                        if (folderToCreate.MetaData.Count(x => x.FieldId == item.FieldId) == 0)
                                        {
                                            folderToCreate.MetaData.Add(new FolderMeta
                                            {
                                                FieldId = item.FieldId,
                                                Value = null
                                            });
                                        }
                                    }
                                }

                                context.Folders.Add(folderToCreate);
                                await context.SaveChangesAsync().ConfigureAwait(false);
                            }

                            folderParent = folderToCreate.Id;
                            folderParentObj = folderToCreate;
                        }

                        parent = folderToCreate;
                    }
                }

                var exist = await context.Documents.CountAsync(x => x.RepositoryId == repo.Id
                    && x.ParentId == (parent != null ? parent.Id : model.ParentId)
                    && x.Name == model.Name)
                    .ConfigureAwait(false);

                if (exist > 0)
                {
                    throw new ArgumentException($"Document name '{model.Name}' is already exist");
                }

                if (parent != null && parent.MetaData != null)
                {
                    foreach (var item in parent.MetaData)
                    {
                        if (!model.Meta.ContainsKey(item.Field.Name))
                        {
                            model.Meta[item.Field.Name] = null;
                        }
                    }
                }

                var date = DateTime.Now;

                var document = new Document()
                {
                    Name = model.Name,
                    Title = model.Title,
                    Description = model.Description,
                    Repository = repo,
                    Parent = parent,
                    ContentType = model.ContentType,
                    Version = 0,
                    Length = model.Length,
                    CheckInKey = Guid.NewGuid().ToString("D"),
                    LastOperation = DocumentOperation.CheckedOut,
                    OperationBy = model.UserName,
                    OperationDate = date,
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

                    if (document.MetaData == null)
                    {
                        document.MetaData = new List<DocumentMeta>();
                    }

                    foreach (var field in agList)
                    {
                        var docMeta = new DocumentMeta
                        {
                            FieldId = field.Id,
                            Value = model.Meta[field.Name]
                        };

                        document.MetaData.Add(docMeta);
                    }

                }

                if (document.MetaData == null)
                {
                    document.MetaData = new List<DocumentMeta>();
                }


                document.CreatedOn = document.UpdatedOn = DateTime.Now; ;
                repo.UpdatedOn = date;
                context.Documents.Add(document);
                await context.SaveChangesAsync().ConfigureAwait(false);

                var dModel = new DocumentDetailModel
                {
                    Id = document.Id,
                    Name = document.Name,
                    Title = document.Title,
                    Description = document.Description,
                    RepositoryId = document.RepositoryId,
                    Repository = repo.Name,
                    ParentId = document.ParentId,
                    Parent = parent?.Name,
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

                return dModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task UpdateDocument(int? id, DocumentUpdateModel model)
        {
            try
            {
                if (!id.HasValue)
                {
                    throw new ArgumentException("Must provide a document id");
                }

                if (model == null)
                {
                    throw new ArgumentException("Must provide a document updated model");
                }

                if (string.IsNullOrEmpty(model.UserName))
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
                    throw new Exception($"Document '{id}' not found");
                }

                if (string.IsNullOrEmpty(model.Title))
                {
                    document.Title = document.Name;
                }
                else
                {
                    document.Title = model.Title;
                }

                document.Description = model.Description;

                if (document.ParentId != model.ParentId)
                {
                    if (model.ParentId.HasValue)
                    {
                        var parent = await context.Folders
                            .Include(x => x.MetaData).ThenInclude(m => m.Field)
                            .Where(x => x.Id == model.ParentId && x.RepositoryId == document.RepositoryId)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

                        if (parent == null)
                        {
                            throw new Exception($"Parent folder '{model.ParentId}' not found in repository '{document.Repository.Name}'");
                        }

                        document.Parent = parent;
                    }

                    document.ParentId = model.ParentId;
                }

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

                    if (document.MetaData == null)
                    {
                        document.MetaData = new List<DocumentMeta>();
                    }

                    foreach (var field in agList)
                    {
                        var meta = document.MetaData.FirstOrDefault(m => m.Field.Name == field.Name);
                        if (meta != null)
                        {
                            meta.Value = model.Meta[field.Name];
                        }
                        else
                        {
                            var docMeta = new DocumentMeta
                            {
                                FieldId = field.Id,
                                Value = model.Meta[field.Name]
                            };

                            document.MetaData.Add(docMeta);
                        }
                    }
                }

                document.UpdatedBy = model.UserName;
                document.UpdatedOn = DateTime.Now;

                await context.SaveChangesAsync().ConfigureAwait(false);


            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UploadChunk(ChunkAddModel model)
        {
            bool result = false;
            try
            {
                
                var FileDataContent = model.File;
                if (FileDataContent != null && FileDataContent.Length > 0)
                {
                    // take the input stream, and save it to a temp folder using the original file.part name posted
                    var stream = FileDataContent.OpenReadStream();
                    var fileName = Path.GetFileName(FileDataContent.FileName);
                    var tempFolder = AppSettingsProvider.TempFolder;
                    var UploadPath = $"{tempFolder}\\{model.RepositoryId}\\{model.DocumentId}\\{model.CheckInKey}";
                    Directory.CreateDirectory(UploadPath);
                    string path = Path.Combine(UploadPath, fileName);
                    try
                    {
                        if (System.IO.File.Exists(path))
                            System.IO.File.Delete(path);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        // Once the file part is saved, see if we have enough to merge it
                        Merger UT = new Merger();

                        result = UT.MergeFile(path);
                        if (result)
                        {
                            var document = await context.Documents
                                .Include(x => x.Repository)
                                .Include(x => x.Parent)
                                .Where(x => x.Id == model.DocumentId
                                    && x.LastOperation == DocumentOperation.CheckedOut
                                    && x.OperationBy == model.UserName
                                    && x.CheckInKey == model.CheckInKey
                                    )
                                .SingleOrDefaultAsync().ConfigureAwait(false);

                            if (document == null)
                            {
                                throw new Exception($"Document with id={model.DocumentId} and version={model.Version} not found");
                            }

                            document.Name = model.OriginalName;
                            string sourcePath = Path.Combine(UploadPath, model.OriginalName);

                            if (document.Repository.StorageType == StorageType.Database)
                            {
                                document.Version += 1;

                                var dbCunksIds = await context.Chunks
                                    .Where(x => x.DocumentId == document.Id && x.Version == document.Version)
                                    .Select(x => x.Id)
                                    .ToListAsync()
                                    .ConfigureAwait(false);

                                foreach (var idChunk in dbCunksIds)
                                {
                                    var chunkToDelete = await context.Chunks.FindAsync(idChunk).ConfigureAwait(false);
                                    context.Chunks.Remove(chunkToDelete);
                                    await context.SaveChangesAsync().ConfigureAwait(false);
                                    chunkToDelete = null;
                                }

                                var splitter = new Splitter();
                                var splitPath = Path.Combine(UploadPath, $"split-{Guid.NewGuid().ToString("D")}");
                                Directory.CreateDirectory(splitPath);
                                splitter.TempFolder = splitPath;
                                splitter.MaxFileSizeMB = 10; // 10MB
                                splitter.FileName = sourcePath;
                                var splitResult = splitter.SplitFile();
                                int sortId = 1;
                                if (splitResult)
                                {
                                    foreach (var part in splitter.FileParts)
                                    {
                                        var dbChunk = new Chunk
                                        {
                                            DocumentId = document.Id,
                                            Version = document.Version,
                                            SortId = sortId++
                                        };

                                        dbChunk.Contents = System.IO.File.ReadAllBytes(part);
                                        dbChunk.Length = dbChunk.Contents.Length;
                                        context.Chunks.Add(dbChunk);
                                        await context.SaveChangesAsync().ConfigureAwait(false);
                                        dbChunk = null;
                                    }

                                    Directory.Delete(UploadPath, true);

                                    document.LastOperation = DocumentOperation.CheckedIn;
                                    document.OperationBy = model.UserName;
                                    document.OperationDate = DateTime.Now;
                                    document.CheckInKey = Guid.NewGuid().ToString("D");

                                    context.Histories.Add(new DocumentHistory
                                    {
                                        DocumentId = document.Id,
                                        Version = document.Version,
                                        Operation = DocumentOperation.CheckedIn,
                                        OperationBy = document.OperationBy,
                                        OperationOn = document.OperationDate
                                    });

                                    await context.SaveChangesAsync().ConfigureAwait(false);
                                }

                            }
                            else
                            {
                                // Copy File to Url
                                document.Version += 1;


                                string destPath = Path.Combine(document.Repository.Path, document.RepositoryId.ToString(), document.Id.ToString(), document.Version.ToString());
                                Directory.CreateDirectory(destPath);

                                string destFile = Path.Combine(destPath, document.Name);
                                System.IO.File.Move(sourcePath, destFile);
                                Directory.Delete(UploadPath, true);


                                document.LastOperation = DocumentOperation.CheckedIn;
                                document.OperationBy = model.UserName;
                                document.OperationDate = DateTime.Now;
                                document.CheckInKey = Guid.NewGuid().ToString("D");

                                context.Histories.Add(new DocumentHistory
                                {
                                    DocumentId = document.Id,
                                    Version = document.Version,
                                    Operation = DocumentOperation.CheckedIn,
                                    OperationBy = document.OperationBy,
                                    OperationOn = document.OperationDate
                                });
                            }

                            await context.SaveChangesAsync().ConfigureAwait(false);
                        }


                    }
                    catch (IOException ex)
                    {
                        // handle

                    }
                }


            }
            catch (Exception ex)
            {
                throw;
            }

            return result;
        }

        public async Task UploadThumbnail(DocumentThumbnail model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Invalid thumbail");
                }

                model.Id = 0;

                context.DocumentThumbnails.Add(model);
                await context.SaveChangesAsync().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<DocumentCheckoutResultModel> Checkout(DocumentCheckoutModel model)
        {
            DocumentCheckoutResultModel result = new DocumentCheckoutResultModel
            {
                DocumentId = model.DocumentId,
                UserName = model.UserName,
                IsOk = false
            };

            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Invalid checkout object");
                }

                if (model.DocumentId <= 0 || string.IsNullOrEmpty(model.UserName))
                {
                    throw new ArgumentException($"Invalid arguments id='{model.DocumentId}' and username='{model.UserName}'");
                }

                var document = await context.Documents
                    .Where(x => x.Id == model.DocumentId)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (document == null)
                {
                    throw new ArgumentException($"Document not with id='{model.DocumentId}' found");
                }

                if (document.LastOperation == DocumentOperation.CheckedOut)
                {
                    throw new ArgumentException($"Document with id='{model.DocumentId}' is checked out by user '{document.OperationBy}' on '{document.OperationDate}'");
                }

                DateTime date = DateTime.Now;

                document.LastOperation = DocumentOperation.CheckedOut;
                document.OperationBy = model.UserName;
                document.OperationDate = date;
                document.CheckInKey = Guid.NewGuid().ToString("D");
                document.UpdatedBy = model.UserName;
                document.UpdatedOn = date;


                context.Histories.Add(new DocumentHistory
                {
                    DocumentId = document.Id,
                    Operation = document.LastOperation,
                    OperationBy = document.OperationBy,
                    OperationOn = document.OperationDate
                });

                await context.SaveChangesAsync().ConfigureAwait(false);

                result.IsOk = true;
                result.CheckInKey = document.CheckInKey;

            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                return result;
            }

            return result;
        }

        public async Task<DocumentDetailModel> Checkin(DocumentCheckinModel model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentException("Invalid checkin object");
                }

                if (model.DocumentId <= 0 || string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.CheckInKey) || model.Length <= 0)
                {
                    throw new ArgumentException($"Invalid arguments id='{model.DocumentId}' and username='{model.UserName}' and checkInKey='{model.CheckInKey}' and length='{model.Length}'");
                }

                var document = await context.Documents
                    .Include(x => x.MetaData).ThenInclude(x => x.Field)
                    .Include(x => x.Repository)
                    .Include(x => x.Parent)
                    .Where(x => x.Id == model.DocumentId && x.CheckInKey == model.CheckInKey)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);

                if (document == null)
                {
                    throw new ArgumentException($"Document not with id='{model.DocumentId}' found");
                }

                if (document.LastOperation == DocumentOperation.CheckedIn)
                {
                    throw new ArgumentException($"Document with id='{model.DocumentId}' is not checked out");
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

                return dModel;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
