using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBook.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePostImageByteColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostImage",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "PostImage",
                table: "Posts",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
