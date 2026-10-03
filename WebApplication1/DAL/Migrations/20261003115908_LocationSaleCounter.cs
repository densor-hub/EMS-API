using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class LocationSaleCounter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IncrementalId",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId1",
                table: "Sales",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "SaleDate",
                table: "Sales",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateTable(
                name: "LocationSaleSequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastSaleIncremental = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
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
                    table.PrimaryKey("PK_LocationSaleSequences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LocationSaleSequences_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_Location_Date_Incremental",
                table: "Sales",
                columns: new[] { "LocationId", "SaleDate", "IncrementalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_LocationId1",
                table: "Sales",
                column: "LocationId1");

            migrationBuilder.CreateIndex(
                name: "IX_LocationSaleSequences_LocationId",
                table: "LocationSaleSequences",
                column: "LocationId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Locations_LocationId1",
                table: "Sales",
                column: "LocationId1",
                principalTable: "Locations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Locations_LocationId1",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "LocationSaleSequences");

            migrationBuilder.DropIndex(
                name: "IX_Sales_Location_Date_Incremental",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_LocationId1",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IncrementalId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "LocationId1",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "SaleDate",
                table: "Sales");
        }
    }
}
