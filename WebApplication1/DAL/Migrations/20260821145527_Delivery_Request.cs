using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class Delivery_Request : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransactionComment_AspNetUsers_ApplicationUserId",
                table: "TransactionComment");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionComment_Transactions_TransactionId",
                table: "TransactionComment");

            migrationBuilder.DropTable(
                name: "TransactionDelivery");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionComment",
                table: "TransactionComment");

            migrationBuilder.RenameTable(
                name: "TransactionComment",
                newName: "Comments");

            migrationBuilder.RenameColumn(
                name: "BatchId",
                table: "TransactionItemsDelivered",
                newName: "TransactionDeliveryRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionComment_TransactionType",
                table: "Comments",
                newName: "IX_Comments_TransactionType");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionComment_TransactionId",
                table: "Comments",
                newName: "IX_Comments_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionComment_ApplicationUserId",
                table: "Comments",
                newName: "IX_Comments_ApplicationUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "TransactionDeliveryRequesId",
                table: "TransactionItemsDelivered",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TransactionDeliverRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsDelivered = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedById = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionDeliverRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransactionDeliverRequests_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionDeliverRequests_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransactionDeliverRequests_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransactionItemsDelivered_TransactionDeliveryRequestId",
                table: "TransactionItemsDelivered",
                column: "TransactionDeliveryRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDeliverRequests_CreatedById",
                table: "TransactionDeliverRequests",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDeliverRequests_TransactionId",
                table: "TransactionDeliverRequests",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionDeliverRequests_UpdatedById",
                table: "TransactionDeliverRequests",
                column: "UpdatedById");

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
                name: "FK_TransactionItemsDelivered_TransactionDeliverRequests_Transa~",
                table: "TransactionItemsDelivered",
                column: "TransactionDeliveryRequestId",
                principalTable: "TransactionDeliverRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_ApplicationUserId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Transactions_TransactionId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionDeliverRequests_Transa~",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropTable(
                name: "TransactionDeliverRequests");

            migrationBuilder.DropIndex(
                name: "IX_TransactionItemsDelivered_TransactionDeliveryRequestId",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TransactionDeliveryRequesId",
                table: "TransactionItemsDelivered");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "TransactionComment");

            migrationBuilder.RenameColumn(
                name: "TransactionDeliveryRequestId",
                table: "TransactionItemsDelivered",
                newName: "BatchId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TransactionType",
                table: "TransactionComment",
                newName: "IX_TransactionComment_TransactionType");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_TransactionId",
                table: "TransactionComment",
                newName: "IX_TransactionComment_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_ApplicationUserId",
                table: "TransactionComment",
                newName: "IX_TransactionComment_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionComment",
                table: "TransactionComment",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "TransactionDelivery",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionDelivery", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionComment_AspNetUsers_ApplicationUserId",
                table: "TransactionComment",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionComment_Transactions_TransactionId",
                table: "TransactionComment",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
