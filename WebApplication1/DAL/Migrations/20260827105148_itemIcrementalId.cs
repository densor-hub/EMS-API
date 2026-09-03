using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class itemIcrementalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_Items_ItemId",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropIndex(
                name: "IX_TransactionItemsDelivered_ItemId",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropIndex(
                name: "IX_Items_CreatedAtLocationId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "TransactionItemsDelivered");

            migrationBuilder.AddColumn<int>(
                name: "IncrementalId",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncrementalId",
                table: "Companies",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedAtLocationId",
                table: "Items",
                column: "CreatedAtLocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Items_CreatedAtLocationId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IncrementalId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IncrementalId",
                table: "Companies");

            migrationBuilder.AddColumn<Guid>(
                name: "ItemId",
                table: "TransactionItemsDelivered",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItemsDelivered_ItemId",
                table: "TransactionItemsDelivered",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CreatedAtLocationId",
                table: "Items",
                column: "CreatedAtLocationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemsDelivered_Items_ItemId",
                table: "TransactionItemsDelivered",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");
        }
    }
}
