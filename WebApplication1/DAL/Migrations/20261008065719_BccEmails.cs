using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class BccEmails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinancialServiceDisbursement_FinancialServiceProviderContac~",
                table: "FinancialServiceDisbursement");

            migrationBuilder.DropIndex(
                name: "IX_FinancialServiceDisbursement_ContactPersonId",
                table: "FinancialServiceDisbursement");

            migrationBuilder.DropColumn(
                name: "ContactPersonId",
                table: "FinancialServiceDisbursement");

            migrationBuilder.CreateTable(
                name: "FinancialServiceDisbursementContactPersons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisbursementId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContactPersonId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialServiceDisbursementContactPersons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialServiceDisbursementContactPersons_FinancialService~",
                        column: x => x.ContactPersonId,
                        principalTable: "FinancialServiceProviderContactPersons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialServiceDisbursementContactPersons_FinancialServic~1",
                        column: x => x.DisbursementId,
                        principalTable: "FinancialServiceDisbursement",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialServiceDisbursementContactPersons_ContactPersonId",
                table: "FinancialServiceDisbursementContactPersons",
                column: "ContactPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialServiceDisbursementContactPersons_DisbursementId",
                table: "FinancialServiceDisbursementContactPersons",
                column: "DisbursementId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialServiceDisbursementContactPersons");

            migrationBuilder.AddColumn<Guid>(
                name: "ContactPersonId",
                table: "FinancialServiceDisbursement",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FinancialServiceDisbursement_ContactPersonId",
                table: "FinancialServiceDisbursement",
                column: "ContactPersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_FinancialServiceDisbursement_FinancialServiceProviderContac~",
                table: "FinancialServiceDisbursement",
                column: "ContactPersonId",
                principalTable: "FinancialServiceProviderContactPersons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
