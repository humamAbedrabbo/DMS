using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMS.Migrations
{
    public partial class FolderSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Folders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 248, nullable: false),
                    RepositoryId = table.Column<int>(nullable: false),
                    ParentId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    UpdatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_Folders_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Folders_Repositories_RepositoryId",
                        column: x => x.RepositoryId,
                        principalTable: "Repositories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Folders_CreatedBy",
                table: "Folders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_CreatedOn",
                table: "Folders",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ParentId",
                table: "Folders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_UpdatedBy",
                table: "Folders",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_UpdatedOn",
                table: "Folders",
                column: "UpdatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_RepositoryId_Name_ParentId",
                table: "Folders",
                columns: new[] { "RepositoryId", "Name", "ParentId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Folders");
        }
    }
}
