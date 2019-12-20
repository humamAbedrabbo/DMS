using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAS.Services;
using DAS.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DAS.Controllers
{
    public class ReposController : Controller
    {
        private readonly IAdminService adminService;
        private readonly IListsService listsService;
        private readonly IArchiveService arcService;

        public ReposController(IAdminService adminService, IListsService listsService, IArchiveService arcService)
        {
            this.adminService = adminService;
            this.listsService = listsService;
            this.arcService = arcService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            var repos = await listsService.GetRepositoryList().ConfigureAwait(false);
            return View(repos);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Create(RepoAddModel model)
        {
            model.UserName = "test";
            await adminService.AddRepository(model).ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Explore(int repoId, int? folderId = null)
        {

            var treeNodes = await listsService.GetTree(repoId, folderId).ConfigureAwait(false);
            FolderBreadcrumbModel breadcrumb = folderId.HasValue ? await listsService.GetFolderBreadcrumb(folderId.Value).ConfigureAwait(false) : null;
            ViewData["Path"] = breadcrumb?.Path;
            ViewData["RepositoryId"] = repoId;
            ViewData["FolderId"] = folderId;
            ViewData["FolderName"] = breadcrumb?.Name;
            ViewData["ParentId"] = breadcrumb?.ParentId;
            

            return View(treeNodes);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateFolder(FolderAddModel model)
        {
            model.UserName = "test";
            await arcService.AddFolder(model).ConfigureAwait(false);
            return RedirectToAction(nameof(Explore), new { repoId = model.RepositoryId, folderId = model.ParentId });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> SetRepoMeta(int repositoryId, string name, string value)
        {
            await adminService.SetRepoMetaValue(repositoryId, name, value).ConfigureAwait(false);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Upload(string repoId, int? folderId)
        {
            var model = new UploadViewModel();
            FolderDetailModel folder = null;
            FolderBreadcrumbModel breadcrumb = null;
            RepoDetailModel repo = null;

            if(folderId.HasValue)
            {
                folder = await listsService.GetFolderById(folderId).ConfigureAwait(false);
                
            }

            if(folder == null)
            {
                repo = await listsService.GetRepositoryById(repoId).ConfigureAwait(false);
                model.RepositoryId = repo.Id;
                model.RepositoryName = repo.Name;
                model.Path = "/";
                model.Meta = repo.Meta;
            }
            else
            {
                breadcrumb = await listsService.GetFolderBreadcrumb(folder.Id).ConfigureAwait(false);
                model.RepositoryId = folder.RepositoryId;
                model.RepositoryName = folder.Repository;
                model.ParentId = folder.Id;
                model.ParentName = folder.Name;
                model.Path = breadcrumb?.Path;
                model.Meta = folder.Meta;
            }

            return View(model);
        }
    }
}
