using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryAPI6.Migrations
{
    public partial class test7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookSubCategories_Categories_CategoryId",
                table: "BookSubCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookSubCategories",
                table: "BookSubCategories");

            migrationBuilder.DropIndex(
                name: "IX_BookSubCategories_BooksId",
                table: "BookSubCategories");

            migrationBuilder.DropIndex(
                name: "IX_BookSubCategories_CategoryId",
                table: "BookSubCategories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "BookSubCategories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookSubCategories",
                table: "BookSubCategories",
                columns: new[] { "BooksId", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_BookSubCategories_Id",
                table: "BookSubCategories",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BookSubCategories",
                table: "BookSubCategories");

            migrationBuilder.DropIndex(
                name: "IX_BookSubCategories_Id",
                table: "BookSubCategories");

            migrationBuilder.AddColumn<short>(
                name: "CategoryId",
                table: "BookSubCategories",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookSubCategories",
                table: "BookSubCategories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BookSubCategories_BooksId",
                table: "BookSubCategories",
                column: "BooksId");

            migrationBuilder.CreateIndex(
                name: "IX_BookSubCategories_CategoryId",
                table: "BookSubCategories",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookSubCategories_Categories_CategoryId",
                table: "BookSubCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}
