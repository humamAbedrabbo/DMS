using DMS.Data;
using DMS.Exceptions;
using DMS.Models;
using DMS.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Services
{
    public class AdminService : IAdminService
    {
        private readonly DmsContext context;
        private readonly ICurrentUserService currentUserService;
        private AppUser currentUser;

        public AdminService(DmsContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            this.currentUserService = currentUserService;
        }

        #region Repository Query And CRUD Operations
        public async Task<List<Repository>> GetRepositoriesAsync()
        {
            var repositories = await context.Repositories.ToListAsync().ConfigureAwait(false);
            return repositories;
        }

        public async Task<Repository> GetRepositoryByIdAsync(int repositoryId)
        {
            var repository = await context.Repositories.FindAsync(repositoryId).ConfigureAwait(false);

            return repository;
        }

        public async Task<Repository> GetRepositoryByIdWithFolderTreeAsync(int repositoryId)
        {
            var repository = await GetRepositoryByIdAsync(repositoryId);

            var folders = await context.Folders
                .Include(x => x.Parent)
                .Include(x => x.Childs)
                .Where(x => x.RepositoryId == repository.Id)
                .ToListAsync().ConfigureAwait(false);

            repository.Folders = folders.Where(x => !x.ParentId.HasValue).ToList();

            return repository;
        }

        public async Task<Repository> GetRepositoryByNameAsync(string name)
        {
            var repository = await context.Repositories.FirstOrDefaultAsync(x => x.Name == name).ConfigureAwait(false);

            return repository;
        }

        public async Task<Repository> AddRepositoryAsync(CreateRepositoryModel model)
        {
            if(currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            Repository repository = await GetRepositoryByNameAsync(model.Name).ConfigureAwait(false);

            if (repository != null)
            {
                throw new EntityExistException("Repository", $"with name = '{model.Name}'");
            }

            repository = new Repository
            {
                Name = model.Name,
                Description = model.Description,
                StorageType = model.StorageType,
                CreatedBy = currentUser.UserName,
                UpdatedBy = currentUser.UserName
            };

            context.Repositories.Add(repository);
            try
            {
                await context.SaveChangesAsync().ConfigureAwait(false);

                return repository;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateRepositoryAsync(UpdateRepositoryModel model)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            Repository repository = await GetRepositoryByIdAsync(model.Id).ConfigureAwait(false);
            bool isModified = false;

            if (repository == null)
            {
                throw new EntityNotFoundException("Repository", $"with name = '{model.Name}'");
            }

            if (repository.Name != model.Name)
            {
                repository.Name = model.Name;
                isModified = true;
            }

            if (repository.Description != model.Description)
            {
                repository.Description = model.Description;
                isModified = true;
            }

            if (isModified)
            {
                repository.UpdatedBy = currentUser.UserName;

                try
                {
                    await context.SaveChangesAsync().ConfigureAwait(false);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<Repository> DeleteRepositoryAsync(int repositoryId)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            Repository repository = await GetRepositoryByIdAsync(repositoryId).ConfigureAwait(false);

            if (repository == null)
            {
                throw new EntityNotFoundException("Repository", $"with id = '{repositoryId}'");
            }

            try
            {
                repository.IsDeleted = true;
                repository.UpdatedBy = currentUser.UserName;

                await context.SaveChangesAsync().ConfigureAwait(false);

                return repository;
            }
            catch (Exception ex)
            {
                return repository;
            }

        }

        #endregion

        #region Folder Query And CRUD Operations
        public async Task<List<Folder>> GetFoldersAsync()
        {
            var folders = 
                await context.Folders
                .Include(x => x.Repository)
                .Include(x => x.Parent)
                .ToListAsync().ConfigureAwait(false);

            return folders;
        }

        public async Task<List<Folder>> GetFoldersWithChildsAsync()
        {
            var folders =
                await context.Folders
                .Include(x => x.Repository)
                .Include(x => x.Parent)
                .Include(x => x.Childs)
                .ToListAsync().ConfigureAwait(false);

            return folders;
        }

        public async Task<List<Folder>> GetChildFoldersAsync(int? parentFolderId ,int? repositoryId = null)
        {
            var folders =
                await context.Folders
                .Include(x => x.Repository)
                .Include(x => x.Parent)
                .Where(x => (!repositoryId.HasValue || x.RepositoryId == repositoryId) && x.ParentId == parentFolderId)
                .ToListAsync().ConfigureAwait(false);

            return folders;
        }

        public async Task<Folder> GetFolderByIdAsync(int folderId)
        {
            var folder = 
                await context.Folders
                .Include(x => x.Repository)
                .Include(x => x.Parent)
                .Include(x => x.Childs)
                .Where(x => x.Id == folderId)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return folder;
        }

        public async Task<Folder> AddFolderAsync(CreateFolderModel model)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            Folder parent = null;

            if (model.ParentId.HasValue)
            {
                parent = await GetFolderByIdAsync(model.ParentId.Value).ConfigureAwait(false);
            }

            var folder = new Folder
            {
                Name = model.Name,
                RepositoryId = model.RepositoryId,
                Parent = parent,
                Description = model.Description,
                CreatedBy = currentUser.UserName,
                UpdatedBy = currentUser.UserName
            };

            if(!folder.Validate())
            {
                throw new InvalidEntityException("Folder", $"with name = '{model.Name}'");
            }

            context.Folders.Add(folder);
            try
            {
                await context.SaveChangesAsync();

                return folder;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateFolderAsync(UpdateFolderModel model)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            Folder folder = await GetFolderByIdAsync(model.Id).ConfigureAwait(false);
            bool isModified = false;

            if (folder == null)
            {
                throw new EntityNotFoundException("Folder", $"with name = '{model.Name}'");
            }

            if (folder.Name != model.Name)
            {
                folder.Name = model.Name;
                isModified = true;
            }

            if (folder.Description != model.Description)
            {
                folder.Description = model.Description;
                isModified = true;
            }

            if (!folder.Validate())
            {
                throw new InvalidEntityException("Folder", $"with name = '{model.Name}'");
            }

            if (isModified)
            {
                folder.UpdatedBy = currentUser.UserName;

                try
                {
                    await context.SaveChangesAsync().ConfigureAwait(false);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<Folder> DeleteFolderAsync(int folderId)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            Folder folder = await GetFolderByIdAsync(folderId).ConfigureAwait(false);

            if (folder == null)
            {
                throw new EntityNotFoundException("Folder", $"with id = '{folderId}'");
            }

            try
            {
                folder.IsDeleted = true;
                folder.UpdatedBy = currentUser.UserName;

                if (folder.Childs != null)
                {
                    foreach (var childFolder in folder.Childs)
                    {
                        await DeleteFolderAsync(childFolder.Id).ConfigureAwait(false);
                    }
                }

                await context.SaveChangesAsync().ConfigureAwait(false);

                return folder;
            }
            catch (Exception ex)
            {
                return folder;
            }

        }

        #endregion

        #region MetaField Query And CRUD Operations
        public async Task<List<MetaField>> GetMetaFieldsAsync()
        {
            var metaFields = await context.MetaFields.ToListAsync().ConfigureAwait(false);
            return metaFields;
        }

        public async Task<MetaField> GetMetaByIdAsync(int fieldId)
        {
            var metaField = await context.MetaFields.FindAsync(fieldId).ConfigureAwait(false);

            return metaField;
        }

        public async Task<List<MetaField>> GetMetaFieldsByNameAsync(string name)
        {
            var metaFields = await context.MetaFields.Where(x => x.Name == name).ToListAsync().ConfigureAwait(false);

            return metaFields;
        }

        public async Task<List<MetaField>> GetMetaFieldsByTitleAsync(string name)
        {
            var metaFields = await context.MetaFields.Where(x => x.Title == name).ToListAsync().ConfigureAwait(false);

            return metaFields;
        }

        public async Task<MetaField> AddMetaFieldAsync(MetaField model)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            if(string.IsNullOrEmpty(model.Title))
            {
                model.Title = model.GetDefaultTitle();
            }

            model.CreatedBy = currentUser.UserName;
            model.UpdatedBy = currentUser.UserName;

            context.MetaFields.Add(model);
            try
            {
                await context.SaveChangesAsync().ConfigureAwait(false);

                return model;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateMetaFieldAsync(MetaField model)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            if (string.IsNullOrEmpty(model.Title))
            {
                model.Title = model.GetDefaultTitle();
            }

            model.UpdatedBy = currentUser.UserName;
            context.Entry(model).State = EntityState.Modified;

            try
            {
                await context.SaveChangesAsync().ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<MetaField> DeleteMetaFieldAsync(int fieldId)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            MetaField metaField = await GetMetaByIdAsync(fieldId).ConfigureAwait(false);

            if (metaField == null)
            {
                throw new EntityNotFoundException("Repository", $"with id = '{fieldId}'");
            }

            try
            {
                metaField.IsDeleted = true;
                metaField.UpdatedBy = currentUser.UserName;

                await context.SaveChangesAsync().ConfigureAwait(false);

                return metaField;
            }
            catch (Exception ex)
            {
                return metaField;
            }

        }
        #endregion

        #region Document Query And CRUD Operations
        public async Task<List<Document>> GetDocumentsAsync(int folderId)
        {
            var docs =
                await context.Documents
                .Include(x => x.Folder)
                .Where(x => x.FolderId == folderId)
                .ToListAsync().ConfigureAwait(false);

            return docs;
        }

        public async Task<Document> GetDocumentyByIdAsync(int documentId)
        {
            var doc =
                await context.Documents
                .Include(x => x.Folder)
                .Where(x => x.Id == documentId)
                .FirstOrDefaultAsync().ConfigureAwait(false);

            return doc;
        }

        public async Task<Document> AddDocumentAsync(CreateDocumentModel model)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            //Repository repository = await GetRepositoryByIdAsync(model.RepositoryId).ConfigureAwait(false);

            //if (repository == null)
            //{
            //    throw new EntityNotFoundException("Repository", $"with id = '{model.RepositoryId}'");
            //}

            var doc = new Document
            {
                Name = model.Name,
                FolderId = model.FolderId,
                Description = model.Description,
                CreatedBy = currentUser.UserName,
                UpdatedBy = currentUser.UserName
            };

            context.Documents.Add(doc);
            try
            {
                await context.SaveChangesAsync().ConfigureAwait(false);

                return doc;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateDocumentAsync(UpdateDocumentModel model)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            Document doc = await GetDocumentyByIdAsync(model.Id).ConfigureAwait(false);
            bool isModified = false;

            if (doc == null)
            {
                throw new EntityNotFoundException("Document", $"with name = '{model.Name}'");
            }

            if (doc.Name != model.Name)
            {
                doc.Name = model.Name;
                isModified = true;
            }

            if (doc.Description != model.Description)
            {
                doc.Description = model.Description;
                isModified = true;
            }

            if (isModified)
            {
                doc.UpdatedBy = currentUser.UserName;

                try
                {
                    await context.SaveChangesAsync().ConfigureAwait(false);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<Document> DeleteDocumentAsync(int documentId)
        {
            if (currentUser == null)
            {
                currentUser = await currentUserService.GetCurrentUserAsync().ConfigureAwait(false);
            }

            Document doc = await GetDocumentyByIdAsync(documentId).ConfigureAwait(false);

            if (doc == null)
            {
                throw new EntityNotFoundException("Document", $"with id = '{documentId}'");
            }

            try
            {
                doc.IsDeleted = true;
                doc.UpdatedBy = currentUser.UserName;

                await context.SaveChangesAsync().ConfigureAwait(false);

                return doc;
            }
            catch (Exception ex)
            {
                return doc;
            }

        }

        #endregion
    }
}
