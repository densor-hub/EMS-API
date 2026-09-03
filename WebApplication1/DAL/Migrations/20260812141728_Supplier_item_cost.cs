using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Supplier_item_cost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Transactions");

            migrationBuilder.CreateTable(
                name: "SupplierItemCostPrices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupplierId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierItemCostPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierItemCostPrices_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierItemCostPrices_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierItemCostPrices_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItemCostPrices_CreatedAt",
                table: "SupplierItemCostPrices",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItemCostPrices_CreatedById",
                table: "SupplierItemCostPrices",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItemCostPrices_ItemId",
                table: "SupplierItemCostPrices",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierItemCostPrices_SupplierId",
                table: "SupplierItemCostPrices",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupplierItemCostPrices");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Transactions",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
