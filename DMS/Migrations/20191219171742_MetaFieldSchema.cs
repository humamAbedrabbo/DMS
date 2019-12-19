using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DMS.Migrations
{
    public partial class MetaFieldSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetaFields",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: true),
                    FieldType = table.Column<int>(nullable: false),
                    DefaultValue = table.Column<string>(nullable: true),
                    CreatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    UpdatedBy = table.Column<string>(maxLength: 256, nullable: false),
                    CreatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    UpdatedOn = table.Column<DateTime>(nullable: false, defaultValueSql: "getdate()"),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetaFields", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MetaFields_CreatedBy",
                table: "MetaFields",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MetaFields_CreatedOn",
                table: "MetaFields",
                column: "CreatedOn");

            migrationBuilder.CreateIndex(
                name: "IX_MetaFields_Name",
                table: "MetaFields",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MetaFields_Title",
                table: "MetaFields",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_MetaFields_UpdatedBy",
                table: "MetaFields",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MetaFields_UpdatedOn",
                table: "MetaFields",
                column: "UpdatedOn");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetaFields");
        }
    }
}
