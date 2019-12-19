using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMS.Migrations
{
    public partial class DocumentSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 259, nullable: false),
                    RepositoryId = table.Column<int>(nullable: false),
                    FolderId = table.Column<int>(nullable: false),
                    Description = table.Column<string>(maxLength: 500, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    UpdatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Folders_FolderId",
                        column: x => x.FolderId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedBy",
                table: "Documents",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_CreatedOn",
                table: "Documents",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UpdatedBy",
                table: "Documents",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UpdatedOn",
                table: "Documents",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_FolderId_Name",
                table: "Documents",
                columns: new[] { "FolderId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_RepositoryId_Name",
                table: "Documents",
                columns: new[] { "RepositoryId", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");
        }
    }
}
