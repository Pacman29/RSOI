using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBaseServer.Migrations
{
    public partial class addPageNo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PageNo",
                table: "FileInfos",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageNo",
                table: "FileInfos");
        }
    }
}
