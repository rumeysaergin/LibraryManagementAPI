using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLoanLibraryBookRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Books_BookId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "BookId",
                table: "Loans",
                newName: "LibraryBookId");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_BookId",
                table: "Loans",
                newName: "IX_Loans_LibraryBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_LibraryBooks_LibraryBookId",
                table: "Loans",
                column: "LibraryBookId",
                principalTable: "LibraryBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_LibraryBooks_LibraryBookId",
                table: "Loans");

            migrationBuilder.RenameColumn(
                name: "LibraryBookId",
                table: "Loans",
                newName: "BookId");

            migrationBuilder.RenameIndex(
                name: "IX_Loans_LibraryBookId",
                table: "Loans",
                newName: "IX_Loans_BookId");

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Books_BookId",
                table: "Loans",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
