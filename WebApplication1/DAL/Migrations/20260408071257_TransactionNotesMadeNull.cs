using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TransactionNotesMadeNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDelivery_Transaction_TransactionId",
                table: "TransactionDelivery");

            migrationBuilder.DropIndex(
                name: "IX_TransactionDelivery_TransactionId",
                table: "TransactionDelivery");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "TransactionDelivery");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Transaction",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditLimit",
                table: "Customers",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TransactionId",
                table: "TransactionDelivery",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "Transaction",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditLimit",
                table: "Customers",
                type: "numeric",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDelivery_TransactionId",
                table: "TransactionDelivery",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDelivery_Transaction_TransactionId",
                table: "TransactionDelivery",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id");
        }
    }
}
