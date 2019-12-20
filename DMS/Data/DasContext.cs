using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DAS.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAS.Data
{
    public class DasContext : IdentityDbContext
    {
        public DasContext(DbContextOptions<DasContext> options)
            : base(options)
        {
        }

        public DbSet<MetaField> MetaFields { get; set; }
        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Chunk> Chunks { get; set; }
        public DbSet<DocumentHistory> Histories { get; set; }
        public DbSet<RepositoryMeta> RepositoryMetaData { get; set; }
        public DbSet<FolderMeta> FolderMetaData { get; set; }
        public DbSet<DocumentMeta> DocumentMetaData { get; set; }
        public DbSet<DocumentThumbnail> DocumentThumbnails { get; set; }

        public async static Task AddMetaToFolder(DasContext context, int folderId, int fieldId)
        {
            var meta = await context.FolderMetaData
                .Where(x => x.FolderId == folderId && x.FieldId == fieldId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (meta == null)
            {
                context.FolderMetaData.Add(new FolderMeta
                {
                    FolderId = folderId,
                    FieldId = fieldId
                });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

            var childFolders = await context.Folders
                .Where(x => x.ParentId == folderId)
                .Select(x => x.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var child in childFolders)
            {
                await DasContext.AddMetaToFolder(context, child, fieldId).ConfigureAwait(false);
            }

            var childDocuments = await context.Documents
                .Where(x => x.ParentId == folderId)
                .Select(x => x.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var child in childDocuments)
            {
                await DasContext.AddMetaToDocument(context, child, fieldId).ConfigureAwait(false);
            }
        }

        public async static Task AddMetaToDocument(DasContext context, int documentId, int fieldId)
        {
            var meta = await context.DocumentMetaData
                .Where(x => x.DocumentId == documentId && x.FieldId == fieldId)
                .SingleOrDefaultAsync()
                .ConfigureAwait(false);

            if (meta == null)
            {
                context.DocumentMetaData.Add(new DocumentMeta
                {
                    DocumentId = documentId,
                    FieldId = fieldId
                });
                await context.SaveChangesAsync().ConfigureAwait(false);
            }

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Identity Configuration
            builder.Entity<IdentityUser>().ToTable("Users");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

            builder.Entity<MetaField>().Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Entity<MetaField>().Property(p => p.Title).HasMaxLength(200);
            builder.Entity<MetaField>().HasAlternateKey(p => p.Name);

            builder.Entity<Repository>().Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Entity<Repository>().Property(p => p.Title).HasMaxLength(200);
            builder.Entity<Repository>().HasAlternateKey(p => p.Name);

            builder.Entity<Folder>().Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Entity<Folder>().Property(p => p.Title).HasMaxLength(200);
            builder.Entity<Folder>().HasIndex(p => new { p.ParentId, p.Name });

            builder.Entity<Document>().Property(p => p.Name).HasMaxLength(100).IsRequired();
            builder.Entity<Document>().Property(p => p.Title).HasMaxLength(200);
            builder.Entity<Document>().HasIndex(p => new { p.ParentId, p.Name });

            builder.Entity<RepositoryMeta>().HasKey(p => new { p.RepositoryId, p.FieldId });
            builder.Entity<FolderMeta>().HasKey(p => new { p.FolderId, p.FieldId });
            builder.Entity<DocumentMeta>().HasKey(p => new { p.DocumentId, p.FieldId });

            builder.Entity<Repository>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Folder>().HasQueryFilter(p => !p.IsDeleted);
            builder.Entity<Document>().HasQueryFilter(p => !p.IsDeleted);
        }
    }
}
