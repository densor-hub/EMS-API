using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SaleDeliveryCounterOnSaleDeliveryCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockAdjustments_Locations_LocationId",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_StockAdjustments_LocationId",
                table: "StockAdjustments");

            migrationBuilder.DropIndex(
                name: "IX_Sales_Location_Date_Incremental",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "IncrementalId",
                table: "Sales");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOnly",
                table: "SaleTransDeliveryRequests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<int>(
                name: "IncrementalId",
                table: "SaleTransDeliveryRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "SaleTransDeliveryRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleTransDeliveryRequests_LocationId_DateOnly_IncrementalId",
                table: "SaleTransDeliveryRequests",
                columns: new[] { "LocationId", "DateOnly", "IncrementalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_LocationId",
                table: "Sales",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleTransDeliveryRequests_Locations_LocationId",
                table: "SaleTransDeliveryRequests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleTransDeliveryRequests_Locations_LocationId",
                table: "SaleTransDeliveryRequests");

            migrationBuilder.DropIndex(
                name: "IX_SaleTransDeliveryRequests_LocationId_DateOnly_IncrementalId",
                table: "SaleTransDeliveryRequests");

            migrationBuilder.DropIndex(
                name: "IX_Sales_LocationId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DateOnly",
                table: "SaleTransDeliveryRequests");

            migrationBuilder.DropColumn(
                name: "IncrementalId",
                table: "SaleTransDeliveryRequests");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "SaleTransDeliveryRequests");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "StockAdjustments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IncrementalId",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StockAdjustments_LocationId",
                table: "StockAdjustments",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Location_Date_Incremental",
                table: "Sales",
                columns: new[] { "LocationId", "SaleDate", "IncrementalId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAdjustments_Locations_LocationId",
                table: "StockAdjustments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");
        }
    }
}
