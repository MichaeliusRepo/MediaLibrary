using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MediaLibraryApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMusicEntryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "MusicEntry",
                newName: "Tag");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "MusicEntry",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                table: "MusicEntry");

            migrationBuilder.RenameColumn(
                name: "Tag",
                table: "MusicEntry",
                newName: "Notes");
        }
    }
}
