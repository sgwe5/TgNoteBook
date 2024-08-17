using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TgNoteBook.Migrations
{
    /// <inheritdoc />
    public partial class AddChatId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TgChatId",
                table: "NoteBooks",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TgChatId",
                table: "NoteBooks");
        }
    }
}
