using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ItemSplit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemSplitSetups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    TargetItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceQuantity = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    TargetQuantity = table.Column<int>(type: "integer", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedById = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GeneralStatus = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSplitSetups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemSplitSetups_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSplitSetups_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSplitSetups_Items_SourceItemId",
                        column: x => x.SourceItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSplitSetups_Items_TargetItemId",
                        column: x => x.TargetItemId,
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemSplitTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemSplitSetUpId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedById = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SourceItemNameSnapShot = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TragetItemNameSnapShot = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TargetItemQuantitySnapShot = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSplitTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemSplitTransactions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSplitTransactions_ItemSplitSetups_ItemSplitSetUpId",
                        column: x => x.ItemSplitSetUpId,
                        principalTable: "ItemSplitSetups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSplitTransactions_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitSetup_SourceItem_Status",
                table: "ItemSplitSetups",
                columns: new[] { "SourceItemId", "GeneralStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitSetups_CreatedById",
                table: "ItemSplitSetups",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitSetups_GeneralStatus",
                table: "ItemSplitSetups",
                column: "GeneralStatus");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitSetups_SourceItemId",
                table: "ItemSplitSetups",
                column: "SourceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitSetups_TargetItemId",
                table: "ItemSplitSetups",
                column: "TargetItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitSetups_UpdatedById",
                table: "ItemSplitSetups",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "UX_ItemSplitSetup_Source_Target",
                table: "ItemSplitSetups",
                columns: new[] { "SourceItemId", "TargetItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitTransaction_Location_CreatedAt",
                table: "ItemSplitTransactions",
                columns: new[] { "LocationId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitTransaction_Setup_CreatedAt",
                table: "ItemSplitTransactions",
                columns: new[] { "ItemSplitSetUpId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitTransactions_CreatedAt",
                table: "ItemSplitTransactions",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSplitTransactions_CreatedById",
                table: "ItemSplitTransactions",
                column: "CreatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemSplitTransactions");

            migrationBuilder.DropTable(
                name: "ItemSplitSetups");
        }
    }
}
