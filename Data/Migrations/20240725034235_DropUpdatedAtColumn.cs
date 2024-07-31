using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevBook.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropUpdatedAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Posts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
           name: "UpdatedAt",
           table: "Posts",
           nullable: true);
        }
    }
}




