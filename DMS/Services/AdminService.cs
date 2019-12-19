﻿using DMS.Data;
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
                .Include(x => x.Childs)
                .ToListAsync().ConfigureAwait(false);

            return folders;
        }

        public async Task<Folder> GetFolderyByIdAsync(int folderId)
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

        public async Task<Folder> AddFolderyAsync(CreateFolderModel model)
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

            var folder = new Folder
            {
                Name = model.Name,
                RepositoryId = model.RepositoryId,
                ParentId = model.ParentId,
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
                await context.SaveChangesAsync().ConfigureAwait(false);

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

            Folder folder = await GetFolderyByIdAsync(model.Id).ConfigureAwait(false);
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

            Folder folder = await GetFolderyByIdAsync(folderId).ConfigureAwait(false);

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
    }
}
