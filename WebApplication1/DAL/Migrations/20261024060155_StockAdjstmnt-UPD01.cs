using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class StockAdjstmntUPD01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Coupons_CouponId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CouponId",
                table: "Transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CouponId",
                table: "Transactions",
                column: "CouponId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Coupons_CouponId",
                table: "Transactions",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
