using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Delivery_Request_UPD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDeliverRequests_AspNetUsers_CreatedById",
                table: "TransactionDeliverRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDeliverRequests_AspNetUsers_UpdatedById",
                table: "TransactionDeliverRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDeliverRequests_Transactions_TransactionId",
                table: "TransactionDeliverRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliverRequests_Transa~",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionDeliverRequests",
                table: "TransactionDeliverRequests");

            migrationBuilder.RenameTable(
                name: "TransactionDeliverRequests",
                newName: "TransactionDeliveryRequests");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionDeliverRequests_UpdatedById",
                table: "TransactionDeliveryRequests",
                newName: "IX_TransactionDeliveryRequests_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionDeliverRequests_TransactionId",
                table: "TransactionDeliveryRequests",
                newName: "IX_TransactionDeliveryRequests_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionDeliverRequests_CreatedById",
                table: "TransactionDeliveryRequests",
                newName: "IX_TransactionDeliveryRequests_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionDeliveryRequests",
                table: "TransactionDeliveryRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDeliveryRequests_AspNetUsers_CreatedById",
                table: "TransactionDeliveryRequests",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDeliveryRequests_AspNetUsers_UpdatedById",
                table: "TransactionDeliveryRequests",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDeliveryRequests_Transactions_TransactionId",
                table: "TransactionDeliveryRequests",
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
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDeliveryRequests_AspNetUsers_CreatedById",
                table: "TransactionDeliveryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDeliveryRequests_AspNetUsers_UpdatedById",
                table: "TransactionDeliveryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDeliveryRequests_Transactions_TransactionId",
                table: "TransactionDeliveryRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliveryRequests_Trans~",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionDeliveryRequests",
                table: "TransactionDeliveryRequests");

            migrationBuilder.RenameTable(
                name: "TransactionDeliveryRequests",
                newName: "TransactionDeliverRequests");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionDeliveryRequests_UpdatedById",
                table: "TransactionDeliverRequests",
                newName: "IX_TransactionDeliverRequests_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionDeliveryRequests_TransactionId",
                table: "TransactionDeliverRequests",
                newName: "IX_TransactionDeliverRequests_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionDeliveryRequests_CreatedById",
                table: "TransactionDeliverRequests",
                newName: "IX_TransactionDeliverRequests_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionDeliverRequests",
                table: "TransactionDeliverRequests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDeliverRequests_AspNetUsers_CreatedById",
                table: "TransactionDeliverRequests",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDeliverRequests_AspNetUsers_UpdatedById",
                table: "TransactionDeliverRequests",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDeliverRequests_Transactions_TransactionId",
                table: "TransactionDeliverRequests",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliverRequests_Transa~",
                table: "TransactionItemsDelivered",
                column: "TransactionDeliveryRequestId",
                principalTable: "TransactionDeliverRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
