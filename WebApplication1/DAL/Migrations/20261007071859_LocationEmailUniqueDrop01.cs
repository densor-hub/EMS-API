using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class LocationEmailUniqueDrop01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_Email",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Purchases");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Email",
                table: "Locations",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Locations_Email",
                table: "Locations");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Purchases",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Locations_Email",
                table: "Locations",
                column: "Email",
                unique: true);
        }
    }
}
