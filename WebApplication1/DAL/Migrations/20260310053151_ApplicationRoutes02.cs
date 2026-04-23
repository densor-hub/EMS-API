using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationRoutes02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationRoutes",
                table: "ApplicationRoutes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ApplicationRoutes");

            migrationBuilder.RenameTable(
                name: "ApplicationRoutes",
                newName: "applicationroutes");

            migrationBuilder.RenameColumn(
                name: "Route",
                table: "applicationroutes",
                newName: "Title");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "applicationroutes",
                type: "boolean",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "boolean");

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "applicationroutes",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "applicationroutes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "applicationroutes",
                type: "character varying(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_applicationroutes",
                table: "applicationroutes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_applicationroutes_ParentId",
                table: "applicationroutes",
                column: "ParentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_applicationroutes_applicationroutes_ParentId",
                table: "applicationroutes",
                column: "ParentId",
                principalTable: "applicationroutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicationroutes_applicationroutes_ParentId",
                table: "applicationroutes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_applicationroutes",
                table: "applicationroutes");

            migrationBuilder.DropIndex(
                name: "IX_applicationroutes_ParentId",
                table: "applicationroutes");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "applicationroutes");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "applicationroutes");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "applicationroutes");

            migrationBuilder.RenameTable(
                name: "applicationroutes",
                newName: "ApplicationRoutes");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ApplicationRoutes",
                newName: "Route");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "ApplicationRoutes",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldDefaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ApplicationRoutes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationRoutes",
                table: "ApplicationRoutes",
                column: "Id");
        }
    }
}
