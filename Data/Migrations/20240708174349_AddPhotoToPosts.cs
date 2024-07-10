using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace DevBook.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoToPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
            name: "PostImage",
            table: "Posts",
            nullable: true);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostImage",
                table: "Posts");
        }
    }
}
