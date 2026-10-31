using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationSaleSequences_Locations_LocationId",
                table: "LocationSaleSequences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationSaleSequences",
                table: "LocationSaleSequences");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "LocationSaleSequences",
                newName: "LocationSaleSequence");

            migrationBuilder.RenameIndex(
                name: "IX_LocationSaleSequences_LocationId",
                table: "LocationSaleSequence",
                newName: "IX_LocationSaleSequence_LocationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationSaleSequence",
                table: "LocationSaleSequence",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeviceInfo = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreatedByIp = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationSaleSequence_Locations_LocationId",
                table: "LocationSaleSequence",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationSaleSequence_Locations_LocationId",
                table: "LocationSaleSequence");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationSaleSequence",
                table: "LocationSaleSequence");

            migrationBuilder.RenameTable(
                name: "LocationSaleSequence",
                newName: "LocationSaleSequences");

            migrationBuilder.RenameIndex(
                name: "IX_LocationSaleSequence_LocationId",
                table: "LocationSaleSequences",
                newName: "IX_LocationSaleSequences_LocationId");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationSaleSequences",
                table: "LocationSaleSequences",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationSaleSequences_Locations_LocationId",
                table: "LocationSaleSequences",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
