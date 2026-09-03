using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Purchase_transaction_relation_fix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TransactionId",
                table: "Purchases");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TransactionId",
                table: "Purchases",
                column: "TransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TransactionId",
                table: "Purchases");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TransactionId",
                table: "Purchases",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Transactions_TransactionId",
                table: "Purchases",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
