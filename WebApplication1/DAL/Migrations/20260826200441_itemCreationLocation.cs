using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class itemCreationLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedAtLocationId",
                table: "Items",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedAtLocationId",
                table: "Items",
                column: "CreatedAtLocationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Locations_CreatedAtLocationId",
                table: "Items",
                column: "CreatedAtLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Locations_CreatedAtLocationId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_CreatedAtLocationId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CreatedAtLocationId",
                table: "Items");
        }
    }
}
