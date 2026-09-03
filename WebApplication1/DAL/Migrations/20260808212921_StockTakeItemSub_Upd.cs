using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StockTakeItemSub_Upd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhysicalAvailableQuantity",
                table: "StockTakeItemSubmissions",
                newName: "PhysicalUnitOfMeasureQuantity");

            migrationBuilder.AddColumn<int>(
                name: "PhysicalAdditionalPiecesQuantity",
                table: "StockTakeItemSubmissions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhysicalAdditionalPiecesQuantity",
                table: "StockTakeItemSubmissions");

            migrationBuilder.RenameColumn(
                name: "PhysicalUnitOfMeasureQuantity",
                table: "StockTakeItemSubmissions",
                newName: "PhysicalAvailableQuantity");
        }
    }
}
