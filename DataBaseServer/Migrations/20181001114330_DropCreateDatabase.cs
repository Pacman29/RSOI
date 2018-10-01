using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBaseServer.Migrations
{
    public partial class DropCreateDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "changed",
                table: "FileInfos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "changed",
                table: "FileInfos");
        }
    }
}
