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
        private readonly AppUser currentUser;

        public AdminService(DmsContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
            currentUser = currentUserService.GetCurrentUserAsync().Result;
        }

        public async Task<List<Repository>> GetRepositoriesAsync()
        {
            var repositories = await context.Repositories.ToListAsync().ConfigureAwait(false);
            return repositories;
        }

        public async Task<Repository> GetRepositoryByIdAsync(int repositoryId)
        {
            var repository = await context.Repositories.FindAsync(repositoryId).ConfigureAwait(false);
            if (repository == null)
                throw new EntityNotFoundException("Repository", $"with id='{repositoryId}'");

            return repository;
        }

        public async Task<Repository> GetRepositoryByNameAsync(string name)
        {
            var repository = await context.Repositories.SingleOrDefaultAsync(x => x.Name == name).ConfigureAwait(false);
            if (repository == null)
                throw new EntityNotFoundException("Repository", $"with name='{name}'");

            return repository;
        }

        public async Task<Repository> AddRepositoryAsync(CreateRepositoryModel model)
        {
            if ((await GetRepositoryByNameAsync(model.Name).ConfigureAwait(false)) != null)
            {
                throw new EntityExistException("Repository", $"with name = '{model.Name}'");
            }

            Repository repository = new Repository
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
    }
}
