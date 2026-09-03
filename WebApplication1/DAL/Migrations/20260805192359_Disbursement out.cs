using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Disbursementout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BankContactPersons_Banks_FinancialServiceProviderId",
                table: "BankContactPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_Banks_Locations_LocationId",
                table: "Banks");

            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Transactions_TransactionId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_Disbursements_DisbursementId",
                table: "TransactionPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Locations_LocationId",
                table: "Vehicle");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleAssignments_Vehicle_VehicleId",
                table: "VehicleAssignments");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "DisbursementComments");

            migrationBuilder.DropTable(
                name: "FinancialDepotsits");

            migrationBuilder.DropTable(
                name: "LocationPayments");

            migrationBuilder.DropTable(
                name: "TransactionTransportations");

            migrationBuilder.DropTable(
                name: "Disbursements");

            migrationBuilder.DropTable(
                name: "TransactionTokens");

            migrationBuilder.DropIndex(
                name: "IX_TransactionPayments_DisbursementId",
                table: "TransactionPayments");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TransactionId",
                table: "Purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currency",
                table: "Currency");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Banks",
                table: "Banks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BankContactPersons",
                table: "BankContactPersons");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "VehicleAssignments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "TransactionPayments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "TransactionDelivery");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "SupplierLocations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "StockTransferItemsDelivered");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "StockTransferItems");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "StockLevels");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "LocationManangement");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "ItemLocations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "EmployeeLocations");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "ConfirmationCodes");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Banks");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "BankContactPersons");

            migrationBuilder.RenameTable(
                name: "Vehicle",
                newName: "Vehicles");

            migrationBuilder.RenameTable(
                name: "Currency",
                newName: "Currencies");

            migrationBuilder.RenameTable(
                name: "Banks",
                newName: "FinancialServiceProviders");

            migrationBuilder.RenameTable(
                name: "BankContactPersons",
                newName: "FinancialServiceProviderContactPersons");

            migrationBuilder.RenameColumn(
                name: "TransactionAction",
                table: "Transactions",
                newName: "TransactionType");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Transactions",
                newName: "TransactionDate");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_TransactionAction",
                table: "Transactions",
                newName: "IX_Transactions_TransactionType");

            migrationBuilder.RenameColumn(
                name: "DisbursementId",
                table: "TransactionPayments",
                newName: "ConfirmationTokenId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicle_LocationId",
                table: "Vehicles",
                newName: "IX_Vehicles_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Banks_LocationId",
                table: "FinancialServiceProviders",
                newName: "IX_FinancialServiceProviders_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_BankContactPersons_FinancialServiceProviderId",
                table: "FinancialServiceProviderContactPersons",
                newName: "IX_FinancialServiceProviderContactPersons_FinancialServiceProv~");

            migrationBuilder.AlterColumn<int>(
                name: "TransactionResultsType",
                table: "Transactions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<bool>(
                name: "RequiresExternalApproval",
                table: "Transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "TransactionPayments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedBy",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeneralStatus",
                table: "Purchases",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinancialServiceProviders",
                table: "FinancialServiceProviders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinancialServiceProviderContactPersons",
                table: "FinancialServiceProviderContactPersons",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EmployeeDisbursements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeDisbursements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeDisbursements_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeDisbursements_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeDisbursements_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialServiceDisbursement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FinancialServiceProviderId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContactPersonId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialServiceDisbursement", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialServiceDisbursement_FinancialServiceProviderContac~",
                        column: x => x.ContactPersonId,
                        principalTable: "FinancialServiceProviderContactPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialServiceDisbursement_FinancialServiceProviders_Fina~",
                        column: x => x.FinancialServiceProviderId,
                        principalTable: "FinancialServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialServiceDisbursement_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentConfirmationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    UserEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentConfirmationTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransactionComment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionComment_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TransactionItemReversals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReversalDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionItemReversals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionItemReversals_TransactionItems_TransactionItemId",
                        column: x => x.TransactionItemId,
                        principalTable: "TransactionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionPayments_ConfirmationTokenId",
                table: "TransactionPayments",
                column: "ConfirmationTokenId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TransactionId",
                table: "Purchases",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialServiceProviderContactPersons_Code",
                table: "FinancialServiceProviderContactPersons",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialServiceProviderContactPersons_Email",
                table: "FinancialServiceProviderContactPersons",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDisbursements_EmployeeId",
                table: "EmployeeDisbursements",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDisbursements_LocationId",
                table: "EmployeeDisbursements",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeDisbursements_TransactionId",
                table: "EmployeeDisbursements",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialServiceDisbursement_ContactPersonId",
                table: "FinancialServiceDisbursement",
                column: "ContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialServiceDisbursement_FinancialServiceProviderId",
                table: "FinancialServiceDisbursement",
                column: "FinancialServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialServiceDisbursement_TransactionId",
                table: "FinancialServiceDisbursement",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentConfirmationTokens_ExpiresAt",
                table: "PaymentConfirmationTokens",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentConfirmationTokens_PaymentId",
                table: "PaymentConfirmationTokens",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentConfirmationTokens_Token",
                table: "PaymentConfirmationTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentConfirmationTokens_Token_IsUsed",
                table: "PaymentConfirmationTokens",
                columns: new[] { "Token", "IsUsed" });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionComment_TransactionId",
                table: "TransactionComment",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionComment_TransactionType",
                table: "TransactionComment",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItemReversals_Id",
                table: "TransactionItemReversals",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItemReversals_TransactionItemId",
                table: "TransactionItemReversals",
                column: "TransactionItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Transactions_TransactionId",
                table: "Coupons",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialServiceProviderContactPersons_FinancialServiceProv~",
                table: "FinancialServiceProviderContactPersons",
                column: "FinancialServiceProviderId",
                principalTable: "FinancialServiceProviders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialServiceProviders_Locations_LocationId",
                table: "FinancialServiceProviders",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPayments_PaymentConfirmationTokens_ConfirmationT~",
                table: "TransactionPayments",
                column: "ConfirmationTokenId",
                principalTable: "PaymentConfirmationTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleAssignments_Vehicles_VehicleId",
                table: "VehicleAssignments",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_Locations_LocationId",
                table: "Vehicles",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Transactions_TransactionId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialServiceProviderContactPersons_FinancialServiceProv~",
                table: "FinancialServiceProviderContactPersons");

            migrationBuilder.DropForeignKey(
                name: "FK_FinancialServiceProviders_Locations_LocationId",
                table: "FinancialServiceProviders");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_PaymentConfirmationTokens_ConfirmationT~",
                table: "TransactionPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleAssignments_Vehicles_VehicleId",
                table: "VehicleAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_Locations_LocationId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "EmployeeDisbursements");

            migrationBuilder.DropTable(
                name: "FinancialServiceDisbursement");

            migrationBuilder.DropTable(
                name: "PaymentConfirmationTokens");

            migrationBuilder.DropTable(
                name: "TransactionComment");

            migrationBuilder.DropTable(
                name: "TransactionItemReversals");

            migrationBuilder.DropIndex(
                name: "IX_TransactionPayments_ConfirmationTokenId",
                table: "TransactionPayments");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TransactionId",
                table: "Purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicles",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FinancialServiceProviders",
                table: "FinancialServiceProviders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FinancialServiceProviderContactPersons",
                table: "FinancialServiceProviderContactPersons");

            migrationBuilder.DropIndex(
                name: "IX_FinancialServiceProviderContactPersons_Code",
                table: "FinancialServiceProviderContactPersons");

            migrationBuilder.DropIndex(
                name: "IX_FinancialServiceProviderContactPersons_Email",
                table: "FinancialServiceProviderContactPersons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Currencies",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "RequiresExternalApproval",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "TransactionPayments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "GeneralStatus",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Purchases");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                newName: "Vehicle");

            migrationBuilder.RenameTable(
                name: "FinancialServiceProviders",
                newName: "Banks");

            migrationBuilder.RenameTable(
                name: "FinancialServiceProviderContactPersons",
                newName: "BankContactPersons");

            migrationBuilder.RenameTable(
                name: "Currencies",
                newName: "Currency");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                table: "Transactions",
                newName: "TransactionAction");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "Transactions",
                newName: "Date");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_TransactionType",
                table: "Transactions",
                newName: "IX_Transactions_TransactionAction");

            migrationBuilder.RenameColumn(
                name: "ConfirmationTokenId",
                table: "TransactionPayments",
                newName: "DisbursementId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_LocationId",
                table: "Vehicle",
                newName: "IX_Vehicle_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_FinancialServiceProviders_LocationId",
                table: "Banks",
                newName: "IX_Banks_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_FinancialServiceProviderContactPersons_FinancialServiceProv~",
                table: "BankContactPersons",
                newName: "IX_BankContactPersons_FinancialServiceProviderId");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "VehicleAssignments",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TransactionResultsType",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Transactions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentStatus",
                table: "Transactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "TransactionPayments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "TransactionItemsDelivered",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "TransactionItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "TransactionDelivery",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Suppliers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "SupplierLocations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "StockTransfers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "StockTransferItemsDelivered",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "StockTransferItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "StockLevels",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Sales",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Positions",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Locations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "LocationManangement",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Items",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "ItemLocations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Employees",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "EmployeeLocations",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Coupons",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "ConfirmationCodes",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Companies",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyId",
                table: "AspNetUsers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Vehicle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Banks",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "BankContactPersons",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicle",
                table: "Vehicle",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Banks",
                table: "Banks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BankContactPersons",
                table: "BankContactPersons",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Currency",
                table: "Currency",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    TransactionType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LocationPayments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiverId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationPayments_Employees_ReceiverId",
                        column: x => x.ReceiverId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationPayments_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TransactionTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsRevoked = table.Column<bool>(type: "boolean", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    Token = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UserAgent = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    UserEmail = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
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
                name: "TransactionTransportations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTransportations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Disbursements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfirmationTokenId = table.Column<Guid>(type: "uuid", nullable: true),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CancellationReason = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrencyCode = table.Column<string>(type: "text", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: false),
                    RequiresExternalConfirmation = table.Column<bool>(type: "boolean", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TransactionNumber = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
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
                name: "DisbursementComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisbursementId = table.Column<Guid>(type: "uuid", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DisbursementType = table.Column<string>(type: "text", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false)
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
                    ContactPersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    DisbursementId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "IX_Purchases_TransactionId",
                table: "Purchases",
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
                name: "IX_LocationPayments_LocationId",
                table: "LocationPayments",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationPayments_ReceiverId",
                table: "LocationPayments",
                column: "ReceiverId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_BankContactPersons_Banks_FinancialServiceProviderId",
                table: "BankContactPersons",
                column: "FinancialServiceProviderId",
                principalTable: "Banks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Banks_Locations_LocationId",
                table: "Banks",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Transactions_TransactionId",
                table: "Coupons",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases",
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
                name: "FK_Vehicle_Locations_LocationId",
                table: "Vehicle",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleAssignments_Vehicle_VehicleId",
                table: "VehicleAssignments",
                column: "VehicleId",
                principalTable: "Vehicle",
                principalColumn: "Id");
        }
    }
}
