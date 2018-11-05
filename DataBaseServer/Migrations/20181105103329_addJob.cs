using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace DataBaseServer.Migrations
{
    public partial class addJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    changed = table.Column<DateTime>(nullable: false),
                    GUID = table.Column<string>(maxLength: 36, nullable: false),
                    status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.GUID);
                });

            migrationBuilder.CreateTable(
                name: "FileInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    Md5 = table.Column<string>(maxLength: 32, nullable: false),
                    changed = table.Column<DateTime>(nullable: false),
                    fileLength = table.Column<long>(nullable: false),
                    Version = table.Column<DateTime>(nullable: false),
                    Path = table.Column<string>(maxLength: 250, nullable: false),
                    JobGuidFk = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileInfos_Jobs_JobGuidFk",
                        column: x => x.JobGuidFk,
                        principalTable: "Jobs",
                        principalColumn: "GUID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileInfos_JobGuidFk",
                table: "FileInfos",
                column: "JobGuidFk");

            migrationBuilder.CreateIndex(
                name: "IX_FileInfos_Md5",
                table: "FileInfos",
                column: "Md5",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileInfos");

            migrationBuilder.DropTable(
                name: "Jobs");
        }
    }
}
