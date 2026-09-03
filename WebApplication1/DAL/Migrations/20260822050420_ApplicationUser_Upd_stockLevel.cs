using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUser_Upd_stockLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_ApplicationUserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Transactions_TransactionId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApplicationUserId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApplicationUserId1",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliveryRequests_Trans~",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_ApplicationUserId",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransfers_ApplicationUserId1",
                table: "StockTransfers");

            migrationBuilder.DropIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TransactionDeliveryRequesId",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "StockTransfers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CreatedById",
                table: "Comments",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Transactions_TransactionId",
                table: "Comments",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliveryRequests_Trans~",
                table: "TransactionItemsDelivered",
                column: "TransactionDeliveryRequestId",
                principalTable: "TransactionDeliveryRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CreatedById",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Transactions_TransactionId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliveryRequests_Trans~",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropIndex(
                name: "IX_Comments_CreatedById",
                table: "Comments");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionDeliveryRequesId",
                table: "TransactionItemsDelivered",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "StockTransfers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "StockTransfers",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedById",
                table: "Comments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Comments",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ApplicationUserId",
                table: "StockTransfers",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransfers_ApplicationUserId1",
                table: "StockTransfers",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_ApplicationUserId",
                table: "Comments",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Transactions_TransactionId",
                table: "Comments",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApplicationUserId",
                table: "StockTransfers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApplicationUserId1",
                table: "StockTransfers",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliveryRequests_Trans~",
                table: "TransactionItemsDelivered",
                column: "TransactionDeliveryRequestId",
                principalTable: "TransactionDeliveryRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
