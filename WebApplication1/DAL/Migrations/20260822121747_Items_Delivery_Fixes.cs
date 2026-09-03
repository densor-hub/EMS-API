using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Items_Delivery_Fixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliveryRequests_Trans~",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropTable(
                name: "TransactionDeliveryRequests");

            migrationBuilder.RenameColumn(
                name: "TransactionDeliveryRequestId",
                table: "TransactionItemsDelivered",
                newName: "SaleTransDeliveryRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionItemsDelivered_TransactionDeliveryRequestId",
                table: "TransactionItemsDelivered",
                newName: "IX_TransactionItemsDelivered_SaleTransDeliveryRequestId");

            migrationBuilder.AddColumn<Guid>(
                name: "BatchId",
                table: "TransactionItemsDelivered",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsDelivered",
                table: "TransactionItemsDelivered",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SaleTransDeliveryRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SaleId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDelivered = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<string>(type: "text", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleTransDeliveryRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleTransDeliveryRequests_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleTransDeliveryRequests_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleTransDeliveryRequests_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleTransDeliveryRequests_CreatedById",
                table: "SaleTransDeliveryRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SaleTransDeliveryRequests_SaleId",
                table: "SaleTransDeliveryRequests",
                column: "SaleId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleTransDeliveryRequests_UpdatedById",
                table: "SaleTransDeliveryRequests",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemsDelivered_SaleTransDeliveryRequests_SaleTra~",
                table: "TransactionItemsDelivered",
                column: "SaleTransDeliveryRequestId",
                principalTable: "SaleTransDeliveryRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_SaleTransDeliveryRequests_SaleTra~",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropTable(
                name: "SaleTransDeliveryRequests");

            migrationBuilder.DropColumn(
                name: "BatchId",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropColumn(
                name: "IsDelivered",
                table: "TransactionItemsDelivered");

            migrationBuilder.RenameColumn(
                name: "SaleTransDeliveryRequestId",
                table: "TransactionItemsDelivered",
                newName: "TransactionDeliveryRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionItemsDelivered_SaleTransDeliveryRequestId",
                table: "TransactionItemsDelivered",
                newName: "IX_TransactionItemsDelivered_TransactionDeliveryRequestId");

            migrationBuilder.CreateTable(
                name: "TransactionDeliveryRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedById = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDelivered = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionDeliveryRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionDeliveryRequests_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionDeliveryRequests_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionDeliveryRequests_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDeliveryRequests_CreatedById",
                table: "TransactionDeliveryRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDeliveryRequests_TransactionId",
                table: "TransactionDeliveryRequests",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDeliveryRequests_UpdatedById",
                table: "TransactionDeliveryRequests",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliveryRequests_Trans~",
                table: "TransactionItemsDelivered",
                column: "TransactionDeliveryRequestId",
                principalTable: "TransactionDeliveryRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
