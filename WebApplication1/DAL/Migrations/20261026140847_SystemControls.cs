using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SystemControls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemControls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedById = table.Column<string>(type: "text", nullable: true),
                    SaleToDeliveryMode = table.Column<bool>(type: "boolean", nullable: true),
                    RecordingOldTransactions = table.Column<bool>(type: "boolean", nullable: true),
                    MinimumSalePaymentPercentage = table.Column<int>(type: "integer", nullable: true),
                    PosPrinterSize = table.Column<int>(type: "integer", nullable: true),
                    DefaultPageSize = table.Column<int>(type: "integer", nullable: true),
                    Currency = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemControls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemControls_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SystemControls_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SystemControls_LocationId",
                table: "SystemControls",
                column: "LocationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemControls_UpdatedById",
                table: "SystemControls",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SystemControls");
        }
    }
}
