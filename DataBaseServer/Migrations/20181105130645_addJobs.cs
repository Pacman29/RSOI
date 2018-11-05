using Microsoft.EntityFrameworkCore.Migrations;

namespace DataBaseServer.Migrations
{
    public partial class addJobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FileInfos_Md5",
                table: "FileInfos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FileInfos_Md5",
                table: "FileInfos",
                column: "Md5",
                unique: true);
        }
    }
}
