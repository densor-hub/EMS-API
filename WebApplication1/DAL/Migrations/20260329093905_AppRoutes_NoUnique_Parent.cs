using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AppRoutes_NoUnique_Parent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationRoutes_ParentId",
                table: "ApplicationRoutes");

            migrationBuilder.CreateTable(
                name: "AppUserRoutes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserRoutes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRoutes_ParentId",
                table: "ApplicationRoutes",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUserRoutes");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationRoutes_ParentId",
                table: "ApplicationRoutes");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationRoutes_ParentId",
                table: "ApplicationRoutes",
                column: "ParentId",
                unique: true);
        }
    }
}
