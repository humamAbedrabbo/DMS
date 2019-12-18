using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMS.Migrations
{
    public partial class RepositorySchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Repositories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    StorageType = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    UpdatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repositories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_CreatedBy",
                table: "Repositories",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_CreatedOn",
                table: "Repositories",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_Name",
                table: "Repositories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_UpdatedBy",
                table: "Repositories",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Repositories_UpdatedOn",
                table: "Repositories",
                column: "UpdatedOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Repositories");
        }
    }
}
