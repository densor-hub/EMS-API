using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Resumed00 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_purchases_suppliers_SuppplierId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_transaction_TrasactionId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_transaction_TrasactionId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktaking_aspnetusers_ConductedBy",
                table: "stocktaking");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktaking_aspnetusers_VerifiedBy",
                table: "stocktaking");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktaking_locations_LocationId",
                table: "stocktaking");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakingitems_stocktaking_StockTakingId",
                table: "stocktakingitems");

            migrationBuilder.DropForeignKey(
                name: "FK_transactiondeliveries_transaction_TransactionId",
                table: "transactiondeliveries");

            migrationBuilder.DropForeignKey(
                name: "FK_transactionitemsdelivered_items_ItemId",
                table: "transactionitemsdelivered");

            migrationBuilder.DropForeignKey(
                name: "FK_transactionitemsdelivered_transactiondeliveries_Transaction~",
                table: "transactionitemsdelivered");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transactiondeliveries",
                table: "transactiondeliveries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stocktaking",
                table: "stocktaking");

            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "transaction");

            migrationBuilder.DropColumn(
                name: "SubTotal",
                table: "transaction");

            migrationBuilder.RenameTable(
                name: "transactiondeliveries",
                newName: "TransactionDelivery");

            migrationBuilder.RenameTable(
                name: "stocktaking",
                newName: "stocktakings");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "transactionpayments",
                newName: "GeneralStatus");

            migrationBuilder.RenameColumn(
                name: "TransactionDeliveryId",
                table: "transactionitemsdelivered",
                newName: "TransactionItemId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "transactionitemsdelivered",
                newName: "GeneralStatus");

            migrationBuilder.RenameIndex(
                name: "IX_transactionitemsdelivered_TransactionDeliveryId",
                table: "transactionitemsdelivered",
                newName: "IX_transactionitemsdelivered_TransactionItemId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "transactionitems",
                newName: "GeneralStatus");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "transaction",
                newName: "TransactionType");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "transaction",
                newName: "GeneralStatus");

            migrationBuilder.RenameColumn(
                name: "PaymentMethod",
                table: "transaction",
                newName: "TransactionCode");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "transaction",
                newName: "CancellationReason");

            migrationBuilder.RenameColumn(
                name: "TransferCode",
                table: "stocktransfers",
                newName: "TransactionCode");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "stocktransfers",
                newName: "CancellationReason");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "stocktransferitems",
                newName: "Variance");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "stocktransferitems",
                newName: "TransferedQuantity");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "stocktakingitems",
                newName: "VerifiedQuantity");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "stocklevels",
                newName: "GeneralStatus");

            migrationBuilder.RenameColumn(
                name: "TrasactionId",
                table: "sales",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "sales",
                newName: "GeneralStatus");

            migrationBuilder.RenameIndex(
                name: "IX_sales_TrasactionId",
                table: "sales",
                newName: "IX_sales_TransactionId");

            migrationBuilder.RenameColumn(
                name: "TrasactionId",
                table: "purchases",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "SuppplierId",
                table: "purchases",
                newName: "SupplierId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "purchases",
                newName: "GeneralStatus");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_TrasactionId",
                table: "purchases",
                newName: "IX_purchases_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_SuppplierId",
                table: "purchases",
                newName: "IX_purchases_SupplierId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "TransactionDelivery",
                newName: "GeneralStatus");

            migrationBuilder.RenameIndex(
                name: "IX_transactiondeliveries_TransactionId",
                table: "TransactionDelivery",
                newName: "IX_TransactionDelivery_TransactionId");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "stocktakings",
                newName: "Stage");

            migrationBuilder.RenameIndex(
                name: "IX_stocktaking_VerifiedBy",
                table: "stocktakings",
                newName: "IX_stocktakings_VerifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_stocktaking_LocationId",
                table: "stocktakings",
                newName: "IX_stocktakings_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_stocktaking_ConductedBy",
                table: "stocktakings",
                newName: "IX_stocktakings_ConductedBy");

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "transactionpayments",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "transactionpayments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "transactionitemsdelivered",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "transactionitemsdelivered",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "transactionitemsdelivered",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "transactionitems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "suppliers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "suppliers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "supplierlocations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "supplierlocations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "stocktransfers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "stocktransferitems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "stocktransferitems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedQuantity",
                table: "stocktransferitems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RequestedQuantity",
                table: "stocktransferitems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "stocktakingitems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "stocktakingitems",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "stocklevels",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "sales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TransactionNumber",
                table: "sales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "purchases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TransactionCode",
                table: "purchases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "positions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "positions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "locations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "locations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "locationpayments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "locationpayments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "locationmanangement",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "locationmanangement",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "items",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "itemlocations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "itemlocations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "employees",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "employees",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "employeelocations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "employeelocations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "customers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "customers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "companies",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "companies",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "TransactionId",
                table: "TransactionDelivery",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "TransactionDelivery",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "stocktakings",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "stocktakings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionDelivery",
                table: "TransactionDelivery",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stocktakings",
                table: "stocktakings",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedUser = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(9)", maxLength: 9, nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionCodes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_suppliers_SupplierId",
                table: "purchases",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_transaction_TransactionId",
                table: "purchases",
                column: "TransactionId",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_transaction_TransactionId",
                table: "sales",
                column: "TransactionId",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakingitems_stocktakings_StockTakingId",
                table: "stocktakingitems",
                column: "StockTakingId",
                principalTable: "stocktakings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakings_aspnetusers_ConductedBy",
                table: "stocktakings",
                column: "ConductedBy",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakings_aspnetusers_VerifiedBy",
                table: "stocktakings",
                column: "VerifiedBy",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakings_locations_LocationId",
                table: "stocktakings",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDelivery_transaction_TransactionId",
                table: "TransactionDelivery",
                column: "TransactionId",
                principalTable: "transaction",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactionitemsdelivered_items_ItemId",
                table: "transactionitemsdelivered",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactionitemsdelivered_transactionitems_TransactionItemId",
                table: "transactionitemsdelivered",
                column: "TransactionItemId",
                principalTable: "transactionitems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_purchases_suppliers_SupplierId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_transaction_TransactionId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_transaction_TransactionId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakingitems_stocktakings_StockTakingId",
                table: "stocktakingitems");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakings_aspnetusers_ConductedBy",
                table: "stocktakings");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakings_aspnetusers_VerifiedBy",
                table: "stocktakings");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakings_locations_LocationId",
                table: "stocktakings");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDelivery_transaction_TransactionId",
                table: "TransactionDelivery");

            migrationBuilder.DropForeignKey(
                name: "FK_transactionitemsdelivered_items_ItemId",
                table: "transactionitemsdelivered");

            migrationBuilder.DropForeignKey(
                name: "FK_transactionitemsdelivered_transactionitems_TransactionItemId",
                table: "transactionitemsdelivered");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "ConfirmationCodes");

            migrationBuilder.DropTable(
                name: "TransactionCodes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionDelivery",
                table: "TransactionDelivery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stocktakings",
                table: "stocktakings");

            migrationBuilder.DropColumn(
                name: "Balance",
                table: "transactionpayments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "transactionpayments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "transactionitemsdelivered");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "transactionitemsdelivered");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "transactionitems");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "suppliers");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "supplierlocations");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "supplierlocations");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "stocktransfers");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "stocktransferitems");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "stocktransferitems");

            migrationBuilder.DropColumn(
                name: "ReceivedQuantity",
                table: "stocktransferitems");

            migrationBuilder.DropColumn(
                name: "RequestedQuantity",
                table: "stocktransferitems");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "stocktakingitems");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "stocktakingitems");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "stocklevels");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "TransactionNumber",
                table: "sales");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "TransactionCode",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "positions");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "locations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "locationpayments");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "locationpayments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "locationmanangement");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "locationmanangement");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "items");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "items");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "itemlocations");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "itemlocations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "employees");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "employeelocations");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "employeelocations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "customers");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "companies");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "TransactionDelivery");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "stocktakings");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "stocktakings");

            migrationBuilder.RenameTable(
                name: "TransactionDelivery",
                newName: "transactiondeliveries");

            migrationBuilder.RenameTable(
                name: "stocktakings",
                newName: "stocktaking");

            migrationBuilder.RenameColumn(
                name: "GeneralStatus",
                table: "transactionpayments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TransactionItemId",
                table: "transactionitemsdelivered",
                newName: "TransactionDeliveryId");

            migrationBuilder.RenameColumn(
                name: "GeneralStatus",
                table: "transactionitemsdelivered",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_transactionitemsdelivered_TransactionItemId",
                table: "transactionitemsdelivered",
                newName: "IX_transactionitemsdelivered_TransactionDeliveryId");

            migrationBuilder.RenameColumn(
                name: "GeneralStatus",
                table: "transactionitems",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "transaction",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "TransactionCode",
                table: "transaction",
                newName: "PaymentMethod");

            migrationBuilder.RenameColumn(
                name: "GeneralStatus",
                table: "transaction",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "CancellationReason",
                table: "transaction",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "TransactionCode",
                table: "stocktransfers",
                newName: "TransferCode");

            migrationBuilder.RenameColumn(
                name: "CancellationReason",
                table: "stocktransfers",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "Variance",
                table: "stocktransferitems",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TransferedQuantity",
                table: "stocktransferitems",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "VerifiedQuantity",
                table: "stocktakingitems",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "GeneralStatus",
                table: "stocklevels",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "sales",
                newName: "TrasactionId");

            migrationBuilder.RenameColumn(
                name: "GeneralStatus",
                table: "sales",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_sales_TransactionId",
                table: "sales",
                newName: "IX_sales_TrasactionId");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "purchases",
                newName: "TrasactionId");

            migrationBuilder.RenameColumn(
                name: "SupplierId",
                table: "purchases",
                newName: "SuppplierId");

            migrationBuilder.RenameColumn(
                name: "GeneralStatus",
                table: "purchases",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_TransactionId",
                table: "purchases",
                newName: "IX_purchases_TrasactionId");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_SupplierId",
                table: "purchases",
                newName: "IX_purchases_SuppplierId");

            migrationBuilder.RenameColumn(
                name: "GeneralStatus",
                table: "transactiondeliveries",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionDelivery_TransactionId",
                table: "transactiondeliveries",
                newName: "IX_transactiondeliveries_TransactionId");

            migrationBuilder.RenameColumn(
                name: "Stage",
                table: "stocktaking",
                newName: "Status");

            migrationBuilder.RenameIndex(
                name: "IX_stocktakings_VerifiedBy",
                table: "stocktaking",
                newName: "IX_stocktaking_VerifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_stocktakings_LocationId",
                table: "stocktaking",
                newName: "IX_stocktaking_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_stocktakings_ConductedBy",
                table: "stocktaking",
                newName: "IX_stocktaking_ConductedBy");

            migrationBuilder.AlterColumn<Guid>(
                name: "ItemId",
                table: "transactionitemsdelivered",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "transaction",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "transaction",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "SubTotal",
                table: "transaction",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<Guid>(
                name: "TransactionId",
                table: "transactiondeliveries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_transactiondeliveries",
                table: "transactiondeliveries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stocktaking",
                table: "stocktaking",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_suppliers_SuppplierId",
                table: "purchases",
                column: "SuppplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_transaction_TrasactionId",
                table: "purchases",
                column: "TrasactionId",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_transaction_TrasactionId",
                table: "sales",
                column: "TrasactionId",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktaking_aspnetusers_ConductedBy",
                table: "stocktaking",
                column: "ConductedBy",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktaking_aspnetusers_VerifiedBy",
                table: "stocktaking",
                column: "VerifiedBy",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktaking_locations_LocationId",
                table: "stocktaking",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakingitems_stocktaking_StockTakingId",
                table: "stocktakingitems",
                column: "StockTakingId",
                principalTable: "stocktaking",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactiondeliveries_transaction_TransactionId",
                table: "transactiondeliveries",
                column: "TransactionId",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactionitemsdelivered_items_ItemId",
                table: "transactionitemsdelivered",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactionitemsdelivered_transactiondeliveries_Transaction~",
                table: "transactionitemsdelivered",
                column: "TransactionDeliveryId",
                principalTable: "transactiondeliveries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
