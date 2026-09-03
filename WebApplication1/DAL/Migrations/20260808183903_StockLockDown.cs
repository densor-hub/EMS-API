using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StockLockDown : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Locations_LocationId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionComment_Transactions_TransactionId",
                table: "TransactionComment");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Locations_LocationId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "LocationManangement");

            migrationBuilder.DropTable(
                name: "StockTakings");

            migrationBuilder.DropIndex(
                name: "IX_Sales_LocationId",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_LocationId",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "TransactionComment");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Purchases");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Vehicles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Vehicles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "VehicleAssignments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "VehicleAssignments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Transactions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TransactionPayments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TransactionPayments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TransactionItemsDelivered",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TransactionItemsDelivered",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TransactionItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TransactionItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TransactionDelivery",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TransactionDelivery",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Stage",
                table: "TransactionComment",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "TransactionComment",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "TransactionComment",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "TransactionComment",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Suppliers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Suppliers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SupplierLocations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "SupplierLocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StockTransfers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StockTransfers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StockTransferItemsDelivered",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StockTransferItemsDelivered",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StockTransferItems",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StockTransferItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StockLevels",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "StockLevels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Sales",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Sales",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Purchases",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Purchases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Positions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Positions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Locations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Locations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Items",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Items",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ItemLocations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ItemLocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "FinancialServiceProviders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FinancialServiceProviders",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "FinancialServiceProviderContactPersons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "FinancialServiceProviderContactPersons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Employees",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Employees",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EmployeeLocations",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "EmployeeLocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsManager",
                table: "EmployeeLocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Customers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Customers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Coupons",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Coupons",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ConfirmationCodes",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ConfirmationCodes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Companies",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Companies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OldValue = table.Column<string>(type: "text", nullable: false),
                    NewValue = table.Column<string>(type: "text", nullable: false),
                    ChangedBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ChangedByName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    IpAddress = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RequestPath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    RequestMethod = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Metadata = table.Column<string>(type: "jsonb", nullable: false),
                    Details = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditSnapshots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "text", nullable: false),
                    EntityId = table.Column<string>(type: "text", nullable: false),
                    Snapshot = table.Column<string>(type: "text", nullable: false),
                    SnapshotAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TakenBy = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditSnapshots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockLockDownRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TurnAroundTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLockDownRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockLockDownRequests_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLockDownRequests_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockLockDownComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockLockDownRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Stage = table.Column<string>(type: "text", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLockDownComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockLockDownComments_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockLockDownComments_StockLockDownRequests_StockLockDownRe~",
                        column: x => x.StockLockDownRequestId,
                        principalTable: "StockLockDownRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockLockDownItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    StockLockDownRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsEscalated = table.Column<bool>(type: "boolean", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockLockDownItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockLockDownItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockLockDownItems_StockLockDownRequests_StockLockDownReque~",
                        column: x => x.StockLockDownRequestId,
                        principalTable: "StockLockDownRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockTakeItemSubmissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StockLockDownItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VerifiedById = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    SystemAvailableQuantity = table.Column<int>(type: "integer", nullable: false),
                    PhysicalAvailableQuantity = table.Column<int>(type: "integer", nullable: false),
                    Variance = table.Column<int>(type: "integer", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakeItemSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTakeItemSubmissions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakeItemSubmissions_AspNetUsers_VerifiedById",
                        column: x => x.VerifiedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakeItemSubmissions_StockLockDownItems_StockLockDownIt~",
                        column: x => x.StockLockDownItemId,
                        principalTable: "StockLockDownItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionComment_ApplicationUserId",
                table: "TransactionComment",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedAt",
                table: "AuditLogs",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ChangedBy",
                table: "AuditLogs",
                column: "ChangedBy");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_StockLockDownComments_CreatedById",
                table: "StockLockDownComments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockLockDownComments_StockLockDownRequestId",
                table: "StockLockDownComments",
                column: "StockLockDownRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLockDownComments_TransactionType",
                table: "StockLockDownComments",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_StockLockDownItems_ItemId",
                table: "StockLockDownItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLockDownItems_StockLockDownRequestId",
                table: "StockLockDownItems",
                column: "StockLockDownRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLockDownRequests_CreatedById",
                table: "StockLockDownRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockLockDownRequests_LocationId",
                table: "StockLockDownRequests",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockLockDownRequests_TransactionDate",
                table: "StockLockDownRequests",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeItemSubmissions_CreatedById",
                table: "StockTakeItemSubmissions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeItemSubmissions_StockLockDownItemId",
                table: "StockTakeItemSubmissions",
                column: "StockLockDownItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakeItemSubmissions_VerifiedById",
                table: "StockTakeItemSubmissions",
                column: "VerifiedById");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionComment_AspNetUsers_ApplicationUserId",
                table: "TransactionComment",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionComment_Transactions_TransactionId",
                table: "TransactionComment",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Locations_LocationId",
                table: "Transactions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionComment_AspNetUsers_ApplicationUserId",
                table: "TransactionComment");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionComment_Transactions_TransactionId",
                table: "TransactionComment");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Locations_LocationId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "AuditSnapshots");

            migrationBuilder.DropTable(
                name: "StockLockDownComments");

            migrationBuilder.DropTable(
                name: "StockTakeItemSubmissions");

            migrationBuilder.DropTable(
                name: "StockLockDownItems");

            migrationBuilder.DropTable(
                name: "StockLockDownRequests");

            migrationBuilder.DropIndex(
                name: "IX_TransactionComment_ApplicationUserId",
                table: "TransactionComment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TransactionPayments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TransactionPayments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TransactionDelivery");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TransactionDelivery");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "TransactionComment");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "TransactionComment");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "TransactionComment");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SupplierLocations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "SupplierLocations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StockTransferItemsDelivered");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StockTransferItemsDelivered");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StockTransferItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StockTransferItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StockLevels");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "StockLevels");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ItemLocations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ItemLocations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "FinancialServiceProviders");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FinancialServiceProviders");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "FinancialServiceProviderContactPersons");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "FinancialServiceProviderContactPersons");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EmployeeLocations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "EmployeeLocations");

            migrationBuilder.DropColumn(
                name: "IsManager",
                table: "EmployeeLocations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ConfirmationCodes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ConfirmationCodes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Companies");

            migrationBuilder.AlterColumn<int>(
                name: "Stage",
                table: "TransactionComment",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "TransactionComment",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocationManangement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ManagerId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    IsMainManager = table.Column<bool>(type: "boolean", nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationManangement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationManangement_Employees_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LocationManangement_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StockTakings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConductedBy = table.Column<string>(type: "text", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    VerifiedBy = table.Column<string>(type: "text", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    NextStockTakingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    StockTakingDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTakings_AspNetUsers_ConductedBy",
                        column: x => x.ConductedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakings_AspNetUsers_VerifiedBy",
                        column: x => x.VerifiedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakings_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StockTakings_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_LocationId",
                table: "Sales",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_LocationId",
                table: "Purchases",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationManangement_LocationId",
                table: "LocationManangement",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationManangement_ManagerId",
                table: "LocationManangement",
                column: "ManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_ConductedBy",
                table: "StockTakings",
                column: "ConductedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_LocationId",
                table: "StockTakings",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_TransactionId",
                table: "StockTakings",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_VerifiedBy",
                table: "StockTakings",
                column: "VerifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Locations_LocationId",
                table: "Purchases",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionComment_Transactions_TransactionId",
                table: "TransactionComment",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Locations_LocationId",
                table: "Transactions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
