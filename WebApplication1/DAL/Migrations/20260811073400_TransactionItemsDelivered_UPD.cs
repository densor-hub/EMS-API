using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TransactionItemsDelivered_UPD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemReversals_TransactionItems_TransactionItemId",
                table: "TransactionItemReversals");

            migrationBuilder.DropColumn(
                name: "ActualQuantity",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "ExpectedQuantity",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "Variance",
                table: "TransactionItems");

            migrationBuilder.DropColumn(
                name: "VerifiedQuantity",
                table: "TransactionItems");

            migrationBuilder.RenameColumn(
                name: "Quanity",
                table: "TransactionItemsDelivered",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "TransactionItemId",
                table: "TransactionItemReversals",
                newName: "TransactionItemDeliveredId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionItemReversals_TransactionItemId",
                table: "TransactionItemReversals",
                newName: "IX_TransactionItemReversals_TransactionItemDeliveredId");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "TransactionItemReversals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemReversals_TransactionItemsDelivered_Transact~",
                table: "TransactionItemReversals",
                column: "TransactionItemDeliveredId",
                principalTable: "TransactionItemsDelivered",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemReversals_TransactionItemsDelivered_Transact~",
                table: "TransactionItemReversals");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "TransactionItemReversals");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "TransactionItemsDelivered",
                newName: "Quanity");

            migrationBuilder.RenameColumn(
                name: "TransactionItemDeliveredId",
                table: "TransactionItemReversals",
                newName: "TransactionItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionItemReversals_TransactionItemDeliveredId",
                table: "TransactionItemReversals",
                newName: "IX_TransactionItemReversals_TransactionItemId");

            migrationBuilder.AddColumn<int>(
                name: "ActualQuantity",
                table: "TransactionItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExpectedQuantity",
                table: "TransactionItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Variance",
                table: "TransactionItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VerifiedQuantity",
                table: "TransactionItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemReversals_TransactionItems_TransactionItemId",
                table: "TransactionItemReversals",
                column: "TransactionItemId",
                principalTable: "TransactionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
