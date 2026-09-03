using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class stockLEVELfixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApprovedByUser",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_AspNetUsers_InitiatedById",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Locations_FromLocationId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Locations_ToLocationId",
                table: "StockTransfers");

            migrationBuilder.DropTable(
                name: "StockTransferItemsDelivered");

            migrationBuilder.DropTable(
                name: "StockTransferItems");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_InitiatedById",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_ToLocationId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockLevels_ItemId",
                table: "StockLevels");

            migrationBuilder.DropColumn(
                name: "InitiatedById",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "TransactionCode",
                table: "StockTransfers");

            migrationBuilder.RenameColumn(
                name: "ToLocationId",
                table: "StockTransfers",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "FromLocationId",
                table: "StockTransfers",
                newName: "ResponderId");

            migrationBuilder.RenameColumn(
                name: "ApprovedByUser",
                table: "StockTransfers",
                newName: "ApplicationUserId1");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_FromLocationId",
                table: "StockTransfers",
                newName: "IX_StockTransfers_ResponderId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_ApprovedByUser",
                table: "StockTransfers",
                newName: "IX_StockTransfers_ApplicationUserId1");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionItemReceivedId",
                table: "TransactionItemReversals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "StockTransfers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequesterId",
                table: "StockTransfers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "stockTransferApprovalLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockTransferId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stockTransferApprovalLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_stockTransferApprovalLogs_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stockTransferApprovalLogs_StockTransfers_StockTransferId",
                        column: x => x.StockTransferId,
                        principalTable: "StockTransfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionItemReceived",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    DateReceived = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionItemReceived", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionItemReceived_TransactionItems_TransactionItemId",
                        column: x => x.TransactionItemId,
                        principalTable: "TransactionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItemReversals_TransactionItemReceivedId",
                table: "TransactionItemReversals",
                column: "TransactionItemReceivedId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ApplicationUserId",
                table: "StockTransfers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_CreatedAt",
                table: "StockTransfers",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_RequesterId",
                table: "StockTransfers",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_TransactionId",
                table: "StockTransfers",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockLevels_ItemId_LocationId",
                table: "StockLevels",
                columns: new[] { "ItemId", "LocationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stockTransferApprovalLogs_CreatedById",
                table: "stockTransferApprovalLogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_stockTransferApprovalLogs_StockTransferId",
                table: "stockTransferApprovalLogs",
                column: "StockTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItemReceived_TransactionItemId",
                table: "TransactionItemReceived",
                column: "TransactionItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApplicationUserId",
                table: "StockTransfers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApplicationUserId1",
                table: "StockTransfers",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Locations_RequesterId",
                table: "StockTransfers",
                column: "RequesterId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Locations_ResponderId",
                table: "StockTransfers",
                column: "ResponderId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Transactions_TransactionId",
                table: "StockTransfers",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemReversals_TransactionItemReceived_Transactio~",
                table: "TransactionItemReversals",
                column: "TransactionItemReceivedId",
                principalTable: "TransactionItemReceived",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApplicationUserId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApplicationUserId1",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Locations_RequesterId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Locations_ResponderId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Transactions_TransactionId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemReversals_TransactionItemReceived_Transactio~",
                table: "TransactionItemReversals");

            migrationBuilder.DropTable(
                name: "stockTransferApprovalLogs");

            migrationBuilder.DropTable(
                name: "TransactionItemReceived");

            migrationBuilder.DropIndex(
                name: "IX_TransactionItemReversals_TransactionItemReceivedId",
                table: "TransactionItemReversals");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_ApplicationUserId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_CreatedAt",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_RequesterId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_TransactionId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockLevels_ItemId_LocationId",
                table: "StockLevels");

            migrationBuilder.DropColumn(
                name: "TransactionItemReceivedId",
                table: "TransactionItemReversals");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "RequesterId",
                table: "StockTransfers");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "StockTransfers",
                newName: "ToLocationId");

            migrationBuilder.RenameColumn(
                name: "ResponderId",
                table: "StockTransfers",
                newName: "FromLocationId");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId1",
                table: "StockTransfers",
                newName: "ApprovedByUser");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_ResponderId",
                table: "StockTransfers",
                newName: "IX_StockTransfers_FromLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_ApplicationUserId1",
                table: "StockTransfers",
                newName: "IX_StockTransfers_ApprovedByUser");

            migrationBuilder.AddColumn<string>(
                name: "InitiatedById",
                table: "StockTransfers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TransactionCode",
                table: "StockTransfers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StockTransferItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    StockTransferId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    RequestedQuantity = table.Column<int>(type: "integer", nullable: false),
                    TransferedQuantity = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTransferItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTransferItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTransferItems_StockTransfers_StockTransferId",
                        column: x => x.StockTransferId,
                        principalTable: "StockTransfers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTransferItemsDelivered",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockTransferItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Quanity = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
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
                name: "IX_StockTransfers_InitiatedById",
                table: "StockTransfers",
                column: "InitiatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ToLocationId",
                table: "StockTransfers",
                column: "ToLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLevels_ItemId",
                table: "StockLevels",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_ItemId",
                table: "StockTransferItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItems_StockTransferId",
                table: "StockTransferItems",
                column: "StockTransferId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransferItemsDelivered_StockTransferItemId",
                table: "StockTransferItemsDelivered",
                column: "StockTransferItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApprovedByUser",
                table: "StockTransfers",
                column: "ApprovedByUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_AspNetUsers_InitiatedById",
                table: "StockTransfers",
                column: "InitiatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Locations_FromLocationId",
                table: "StockTransfers",
                column: "FromLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Locations_ToLocationId",
                table: "StockTransfers",
                column: "ToLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
