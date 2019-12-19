using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DMS.Models;

namespace DMS.Data
{
    public class DmsContext : IdentityDbContext
    {
        public DmsContext(DbContextOptions<DmsContext> options)
            : base(options)
        {
        }

        public DbSet<Repository> Repositories { get; set; }
        public DbSet<Folder> Folders { get; set; }

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

            // Repository
            builder.Entity<Repository>(e =>
            {
                e.Property(p => p.Name).HasMaxLength(Constants.REPO_NAME_MAX_LENGTH).IsRequired();
                e.Property(p => p.Description).HasMaxLength(Constants.REPO_DESC_MAX_LENGTH);
                e.Property(p => p.CreatedBy).HasMaxLength(Constants.USERNAME_MAX_LENGTH).IsRequired();
                e.Property(p => p.UpdatedBy).HasMaxLength(Constants.USERNAME_MAX_LENGTH).IsRequired();
                e.Property(p => p.CreatedOn).HasDefaultValueSql("getdate()").ValueGeneratedOnAdd();
                e.Property(p => p.UpdatedOn).HasDefaultValueSql("getdate()").ValueGeneratedOnAddOrUpdate();

                e.HasQueryFilter(p => !p.IsDeleted);

                e.HasIndex(p => p.Name).IsUnique();
                e.HasIndex(p => p.CreatedBy);
                e.HasIndex(p => p.UpdatedBy);
                e.HasIndex(p => p.CreatedOn);
                e.HasIndex(p => p.UpdatedOn);
            });

            // Folder
            builder.Entity<Folder>(e =>
            {
                e.Property(p => p.Name).HasMaxLength(Constants.FOLDER_NAME_MAX_LENGTH).IsRequired();
                e.Property(p => p.Description).HasMaxLength(Constants.FOLDER_DESC_MAX_LENGTH);
                e.Property(p => p.CreatedBy).HasMaxLength(Constants.USERNAME_MAX_LENGTH).IsRequired();
                e.Property(p => p.UpdatedBy).HasMaxLength(Constants.USERNAME_MAX_LENGTH).IsRequired();
                e.Property(p => p.CreatedOn).HasDefaultValueSql("getdate()").ValueGeneratedOnAdd();
                e.Property(p => p.UpdatedOn).HasDefaultValueSql("getdate()").ValueGeneratedOnAddOrUpdate();

                e.HasOne(p => p.Repository)
                    .WithMany(p => p.Folders)
                    .HasForeignKey(p => p.RepositoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(p => p.Parent)
                    .WithMany(p => p.Childs)
                    .HasForeignKey(p => p.ParentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasQueryFilter(p => !p.IsDeleted);

                e.HasIndex(p => new { p.RepositoryId, p.Name, p.ParentId });
                e.HasIndex(p => p.CreatedBy);
                e.HasIndex(p => p.UpdatedBy);
                e.HasIndex(p => p.CreatedOn);
                e.HasIndex(p => p.UpdatedOn);
            });
        }
    }
}
