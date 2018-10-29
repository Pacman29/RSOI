using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBaseServer.Migrations
{
    public partial class AddPdfFilePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "FileInfos",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Path",
                table: "FileInfos");
        }
    }
}
