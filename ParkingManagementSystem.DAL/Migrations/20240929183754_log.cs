using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingManagementSystem.DAL.Migrations
{
    public partial class log : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogLevelId = table.Column<byte>(type: "tinyint", nullable: false),
                    ShortMessage = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    FullMessage = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    IsActive = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedById = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_IsDeleted",
                table: "Logs",
                column: "IsDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
