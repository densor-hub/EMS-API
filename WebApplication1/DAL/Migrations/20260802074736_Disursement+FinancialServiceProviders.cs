using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class DisursementFinancialServiceProviders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Locations_LocationId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Transaction_TransactionId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Transaction_TransactionId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTakings_Locations_LocationId",
                table: "StockTakings");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItems_Transaction_TransactionId",
                table: "TransactionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_Transaction_TransactionId",
                table: "TransactionPayments");

            migrationBuilder.DropTable(
                name: "StockTakingItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "StockTakings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StockTakings");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "StockTakings");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "StockTakings");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StockTakings");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StockTakings");

            migrationBuilder.DropColumn(
                name: "TransactionNumber",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "QueuedEmails");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "TransactionCode",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "LocationPayments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "LocationPayments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "LocationPayments");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "LocationPayments");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "LocationPayments");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "LocationPayments");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "LocationPayments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "LocationPayments");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LocationPayments");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "LocationPayments");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "Transactions");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "StockTakings",
                newName: "TransactionId");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "QueuedEmails",
                newName: "ReceiverId");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "LocationPayments",
                newName: "PaymentType");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transactions",
                newName: "TransactionResultsType");

            migrationBuilder.RenameColumn(
                name: "TransactionCode",
                table: "Transactions",
                newName: "TransactionNumber");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "TransactionPayments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DisbursementId",
                table: "TransactionPayments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "TransactionItemsDelivered",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActualQuantity",
                table: "TransactionItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "TransactionItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedQuantity",
                table: "TransactionItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Variance",
                table: "TransactionItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerifiedQuantity",
                table: "TransactionItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "TransactionDelivery",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Suppliers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "SupplierLocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "StockTransfers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "StockTransferItemsDelivered",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "StockTransferItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "StockTakings",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "StockLevels",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "Sales",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "Purchases",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Positions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Locations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "LocationManangement",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Items",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ItemLocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Employees",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "EmployeeLocations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Customers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "ConfirmationCodes",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Companies",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CouponId",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Transactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Address = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banks_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Used = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ExpiryDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupons_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Coupons_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Currency",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currency", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    UserEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionTokens_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleNumber = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicle", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vehicle_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FinancialServiceProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankContactPersons_Banks_FinancialServiceProviderId",
                        column: x => x.FinancialServiceProviderId,
                        principalTable: "Banks",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Disbursements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ConfirmationTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionNumber = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    RequiresExternalConfirmation = table.Column<bool>(type: "boolean", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disbursements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Disbursements_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Disbursements_TransactionTokens_ConfirmationTokenId",
                        column: x => x.ConfirmationTokenId,
                        principalTable: "TransactionTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DriverId = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedtDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UnassignedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CancellationReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleAssignments_Employees_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VehicleAssignments_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DisbursementComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisbursementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    DisbursementType = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisbursementComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DisbursementComments_Disbursements_DisbursementId",
                        column: x => x.DisbursementId,
                        principalTable: "Disbursements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialDepotsits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisbursementId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContactPersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinancialServiceProviderId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialDepotsits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialDepotsits_BankContactPersons_ContactPersonId",
                        column: x => x.ContactPersonId,
                        principalTable: "BankContactPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialDepotsits_Banks_FinancialServiceProviderId",
                        column: x => x.FinancialServiceProviderId,
                        principalTable: "Banks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FinancialDepotsits_Disbursements_DisbursementId",
                        column: x => x.DisbursementId,
                        principalTable: "Disbursements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionPayments_DisbursementId",
                table: "TransactionPayments",
                column: "DisbursementId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakings_TransactionId",
                table: "StockTakings",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TransactionId",
                table: "Comments",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_TransactionType",
                table: "Comments",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CouponId",
                table: "Transactions",
                column: "CouponId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LocationId",
                table: "Transactions",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionAction",
                table: "Transactions",
                column: "TransactionAction");

            migrationBuilder.CreateIndex(
                name: "IX_BankContactPersons_FinancialServiceProviderId",
                table: "BankContactPersons",
                column: "FinancialServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_LocationId",
                table: "Banks",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_LocationId",
                table: "Coupons",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_TransactionId",
                table: "Coupons",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementComments_DisbursementId",
                table: "DisbursementComments",
                column: "DisbursementId");

            migrationBuilder.CreateIndex(
                name: "IX_DisbursementComments_DisbursementType",
                table: "DisbursementComments",
                column: "DisbursementType");

            migrationBuilder.CreateIndex(
                name: "IX_Disbursements_ConfirmationTokenId",
                table: "Disbursements",
                column: "ConfirmationTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_Disbursements_CurrencyCode",
                table: "Disbursements",
                column: "CurrencyCode");

            migrationBuilder.CreateIndex(
                name: "IX_Disbursements_LocationId",
                table: "Disbursements",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Disbursements_TransactionNumber",
                table: "Disbursements",
                column: "TransactionNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Disbursements_Type",
                table: "Disbursements",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialDepotsits_ContactPersonId",
                table: "FinancialDepotsits",
                column: "ContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialDepotsits_DisbursementId",
                table: "FinancialDepotsits",
                column: "DisbursementId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialDepotsits_FinancialServiceProviderId",
                table: "FinancialDepotsits",
                column: "FinancialServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTokens_ExpiresAt",
                table: "TransactionTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTokens_Token",
                table: "TransactionTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTokens_Token_IsUsed",
                table: "TransactionTokens",
                columns: new[] { "Token", "IsUsed" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTokens_TransactionId",
                table: "TransactionTokens",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_LocationId",
                table: "Vehicle",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAssignments_DriverId",
                table: "VehicleAssignments",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAssignments_GeneralStatus",
                table: "VehicleAssignments",
                column: "GeneralStatus");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleAssignments_VehicleId",
                table: "VehicleAssignments",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Transactions_TransactionId",
                table: "Comments",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Locations_LocationId",
                table: "Purchases",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Transactions_TransactionId",
                table: "Sales",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakings_Locations_LocationId",
                table: "StockTakings",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakings_Transactions_TransactionId",
                table: "StockTakings",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItems_Transactions_TransactionId",
                table: "TransactionItems",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPayments_Disbursements_DisbursementId",
                table: "TransactionPayments",
                column: "DisbursementId",
                principalTable: "Disbursements",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPayments_Transactions_TransactionId",
                table: "TransactionPayments",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Coupons_CouponId",
                table: "Transactions",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Locations_LocationId",
                table: "Transactions",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Transactions_TransactionId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Locations_LocationId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Transactions_TransactionId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTakings_Locations_LocationId",
                table: "StockTakings");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTakings_Transactions_TransactionId",
                table: "StockTakings");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItems_Transactions_TransactionId",
                table: "TransactionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_Disbursements_DisbursementId",
                table: "TransactionPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_Transactions_TransactionId",
                table: "TransactionPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Coupons_CouponId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Locations_LocationId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "Currency");

            migrationBuilder.DropTable(
                name: "DisbursementComments");

            migrationBuilder.DropTable(
                name: "FinancialDepotsits");

            migrationBuilder.DropTable(
                name: "VehicleAssignments");

            migrationBuilder.DropTable(
                name: "BankContactPersons");

            migrationBuilder.DropTable(
                name: "Disbursements");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "TransactionTokens");

            migrationBuilder.DropIndex(
                name: "IX_TransactionPayments_DisbursementId",
                table: "TransactionPayments");

            migrationBuilder.DropIndex(
                name: "IX_StockTakings_TransactionId",
                table: "StockTakings");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TransactionId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_TransactionType",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CouponId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_LocationId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransactionAction",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TransactionPayments");

            migrationBuilder.DropColumn(
                name: "DisbursementId",
                table: "TransactionPayments");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropColumn(
                name: "ActualQuantity",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "ExpectedQuantity",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "Variance",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "VerifiedQuantity",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "TransactionDelivery");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SupplierLocations");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StockTransferItemsDelivered");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StockTransferItems");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StockLevels");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "LocationManangement");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ItemLocations");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "EmployeeLocations");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "ConfirmationCodes");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Transactions");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "Transaction");

            migrationBuilder.RenameColumn(
                name: "TransactionId",
                table: "StockTakings",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "QueuedEmails",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "LocationPayments",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "TransactionResultsType",
                table: "Transaction",
                newName: "TransactionType");

            migrationBuilder.RenameColumn(
                name: "TransactionNumber",
                table: "Transaction",
                newName: "TransactionCode");

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "StockTakings",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "StockTakings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StockTakings",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "StockTakings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "StockTakings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StockTakings",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "StockTakings",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "Sales",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionNumber",
                table: "Sales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CustomerId",
                table: "QueuedEmails",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LocationId",
                table: "Purchases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Purchases",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Purchases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "Purchases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "Purchases",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TransactionCode",
                table: "Purchases",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Purchases",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "LocationPayments",
                type: "numeric(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "LocationPayments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "LocationPayments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "LocationPayments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "LocationPayments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "LocationPayments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "LocationPayments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "LocationPayments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LocationPayments",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedBy",
                table: "LocationPayments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "StockTakingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    StockTakingId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActualQuantity = table.Column<int>(type: "integer", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    ExpectedQuantity = table.Column<int>(type: "integer", nullable: false),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    Variance = table.Column<int>(type: "integer", nullable: false),
                    VerifiedQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockTakingItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakingItems_StockTakings_StockTakingId",
                        column: x => x.StockTakingId,
                        principalTable: "StockTakings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingItems_ItemId",
                table: "StockTakingItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingItems_StockTakingId",
                table: "StockTakingItems",
                column: "StockTakingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Locations_LocationId",
                table: "Purchases",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Transaction_TransactionId",
                table: "Purchases",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Transaction_TransactionId",
                table: "Sales",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakings_Locations_LocationId",
                table: "StockTakings",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItems_Transaction_TransactionId",
                table: "TransactionItems",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPayments_Transaction_TransactionId",
                table: "TransactionPayments",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
