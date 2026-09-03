using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Coupon_on_payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coupons_Transactions_TransactionId",
                table: "Coupons");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_PaymentConfirmationTokens_ConfirmationT~",
                table: "TransactionPayments");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CouponId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Coupons_TransactionId",
                table: "Coupons");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Coupons");

            migrationBuilder.AddColumn<Guid>(
                name: "CouponId",
                table: "TransactionPayments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CurrencyId",
                table: "TransactionPayments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currencies",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CouponId",
                table: "Transactions",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionPayments_CouponId",
                table: "TransactionPayments",
                column: "CouponId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionPayments_CurrencyId",
                table: "TransactionPayments",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Currencies_Code",
                table: "Currencies",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PaymentConfirmationTokens_TransactionPayments_PaymentId",
                table: "PaymentConfirmationTokens",
                column: "PaymentId",
                principalTable: "TransactionPayments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPayments_Coupons_CouponId",
                table: "TransactionPayments",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPayments_Currencies_CurrencyId",
                table: "TransactionPayments",
                column: "CurrencyId",
                principalTable: "Currencies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPayments_PaymentConfirmationTokens_ConfirmationT~",
                table: "TransactionPayments",
                column: "ConfirmationTokenId",
                principalTable: "PaymentConfirmationTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PaymentConfirmationTokens_TransactionPayments_PaymentId",
                table: "PaymentConfirmationTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_Coupons_CouponId",
                table: "TransactionPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_Currencies_CurrencyId",
                table: "TransactionPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_PaymentConfirmationTokens_ConfirmationT~",
                table: "TransactionPayments");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_CouponId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_TransactionPayments_CouponId",
                table: "TransactionPayments");

            migrationBuilder.DropIndex(
                name: "IX_TransactionPayments_CurrencyId",
                table: "TransactionPayments");

            migrationBuilder.DropIndex(
                name: "IX_Currencies_Code",
                table: "Currencies");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "TransactionPayments");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "TransactionPayments");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Currencies",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "Coupons",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CouponId",
                table: "Transactions",
                column: "CouponId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_TransactionId",
                table: "Coupons",
                column: "TransactionId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Coupons_Transactions_TransactionId",
                table: "Coupons",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPayments_PaymentConfirmationTokens_ConfirmationT~",
                table: "TransactionPayments",
                column: "ConfirmationTokenId",
                principalTable: "PaymentConfirmationTokens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
