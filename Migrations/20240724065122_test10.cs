using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryAPI6.Migrations
{
    public partial class test10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_BookCopy_BookCopyId",
                table: "Ratings");

            migrationBuilder.RenameColumn(
                name: "BookCopyId",
                table: "Ratings",
                newName: "BookId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_BookCopyId",
                table: "Ratings",
                newName: "IX_Ratings_BookId");

            migrationBuilder.AddColumn<int>(
                name: "VoteCount",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "VoteSum",
                table: "Books",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Books_BookId",
                table: "Ratings",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Books_BookId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "VoteCount",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "VoteSum",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Ratings",
                newName: "BookCopyId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_BookId",
                table: "Ratings",
                newName: "IX_Ratings_BookCopyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_BookCopy_BookCopyId",
                table: "Ratings",
                column: "BookCopyId",
                principalTable: "BookCopy",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
