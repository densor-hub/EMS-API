using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StockTransferItemsDelivered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivedQuantity",
                table: "StockTransferItems");

            migrationBuilder.DropColumn(
                name: "Variance",
                table: "StockTransferItems");

            migrationBuilder.CreateTable(
                name: "StockTransferItemsDelivered",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockTransferItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quanity = table.Column<int>(type: "integer", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransferItemsDelivered", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransferItemsDelivered_StockTransferItems_StockTransfe~",
                        column: x => x.StockTransferItemId,
                        principalTable: "StockTransferItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItemsDelivered_StockTransferItemId",
                table: "StockTransferItemsDelivered",
                column: "StockTransferItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockTransferItemsDelivered");

            migrationBuilder.AddColumn<int>(
                name: "ReceivedQuantity",
                table: "StockTransferItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Variance",
                table: "StockTransferItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
