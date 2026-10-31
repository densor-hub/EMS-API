using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StockAdjstmntUPD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustments_AspNetUsers_CreatedById",
                table: "StockAdjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustments_Items_ItemId",
                table: "StockAdjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustments_Locations_LocationId",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustment_Item_Location_TransactionDate",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustments_CreatedById",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustments_GeneralStatus",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustments_ItemId",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustments_TransactionDate",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "TransactionDate",
                table: "StockAdjustments");

            migrationBuilder.RenameColumn(
                name: "ItemId",
                table: "StockAdjustments",
                newName: "TransactionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "StockAdjustments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_TransactionId",
                table: "StockAdjustments",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_TransactionId_SupplierId_ApprovedById",
                table: "StockAdjustments",
                columns: new[] { "TransactionId", "SupplierId", "ApprovedById" });

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustments_Locations_LocationId",
                table: "StockAdjustments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustments_Transactions_TransactionId",
                table: "StockAdjustments",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustments_Locations_LocationId",
                table: "StockAdjustments");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustments_Transactions_TransactionId",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustments_TransactionId",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustments_TransactionId_SupplierId_ApprovedById",
                table: "StockAdjustments");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "StockAdjustments",
                newName: "ItemId");

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "StockAdjustments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StockAdjustments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "StockAdjustments",
                type: "character varying(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "StockAdjustments",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "StockAdjustments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "TransactionDate",
                table: "StockAdjustments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustment_Item_Location_TransactionDate",
                table: "StockAdjustments",
                columns: new[] { "ItemId", "LocationId", "TransactionDate" });

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_CreatedById",
                table: "StockAdjustments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_GeneralStatus",
                table: "StockAdjustments",
                column: "GeneralStatus");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_ItemId",
                table: "StockAdjustments",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_TransactionDate",
                table: "StockAdjustments",
                column: "TransactionDate");

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustments_AspNetUsers_CreatedById",
                table: "StockAdjustments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustments_Items_ItemId",
                table: "StockAdjustments",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustments_Locations_LocationId",
                table: "StockAdjustments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
