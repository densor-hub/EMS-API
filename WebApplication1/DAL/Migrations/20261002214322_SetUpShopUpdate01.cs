using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SetUpShopUpdate01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Locations",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTransactionCounters_CounterDate",
                table: "DailyTransactionCounters",
                column: "CounterDate");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTransactionCounters_LocationId",
                table: "DailyTransactionCounters",
                column: "LocationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DailyTransactionCounters_CounterDate",
                table: "DailyTransactionCounters");

            migrationBuilder.DropIndex(
                name: "IX_DailyTransactionCounters_LocationId",
                table: "DailyTransactionCounters");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Locations",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
