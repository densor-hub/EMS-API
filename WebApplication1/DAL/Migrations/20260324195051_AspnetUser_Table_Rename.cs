using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AspnetUser_Table_Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_applicationroutes_applicationroutes_ParentId",
                table: "applicationroutes");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_aspnetusers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_aspnetusers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_aspnetusers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_aspnetusers_companies_CompanyId",
                table: "aspnetusers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_aspnetusers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_customers_locations_LocationId",
                table: "customers");

            migrationBuilder.DropForeignKey(
                name: "FK_employeelocations_employees_EmployeeId",
                table: "employeelocations");

            migrationBuilder.DropForeignKey(
                name: "FK_employeelocations_locations_LocationId",
                table: "employeelocations");

            migrationBuilder.DropForeignKey(
                name: "FK_employees_companies_CompanyId",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "FK_employees_positions_PositionId",
                table: "employees");

            migrationBuilder.DropForeignKey(
                name: "FK_itemlocations_items_ItemId",
                table: "itemlocations");

            migrationBuilder.DropForeignKey(
                name: "FK_itemlocations_locations_LocationId",
                table: "itemlocations");

            migrationBuilder.DropForeignKey(
                name: "FK_items_companies_CompanyId",
                table: "items");

            migrationBuilder.DropForeignKey(
                name: "FK_locationmanangement_employees_ManagerId",
                table: "locationmanangement");

            migrationBuilder.DropForeignKey(
                name: "FK_locationmanangement_locations_LocationId",
                table: "locationmanangement");

            migrationBuilder.DropForeignKey(
                name: "FK_locationpayments_employees_ReceiverId",
                table: "locationpayments");

            migrationBuilder.DropForeignKey(
                name: "FK_locationpayments_locations_LocationId",
                table: "locationpayments");

            migrationBuilder.DropForeignKey(
                name: "FK_locations_companies_CompanyId",
                table: "locations");

            migrationBuilder.DropForeignKey(
                name: "FK_positions_companies_CompanyId",
                table: "positions");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_aspnetusers_PurcasedById",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_locations_LocationId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_suppliers_SupplierId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_transaction_TransactionId",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_aspnetusers_SalesPersonId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_customers_CustomerId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_locations_LocationId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_sales_transaction_TransactionId",
                table: "sales");

            migrationBuilder.DropForeignKey(
                name: "FK_stocklevels_items_ItemId",
                table: "stocklevels");

            migrationBuilder.DropForeignKey(
                name: "FK_stocklevels_locations_LocationId",
                table: "stocklevels");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakingitems_items_ItemId",
                table: "stocktakingitems");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakingitems_stocktakings_StockTakingId",
                table: "stocktakingitems");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakings_aspnetusers_ConductedBy",
                table: "stocktakings");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakings_aspnetusers_VerifiedBy",
                table: "stocktakings");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktakings_locations_LocationId",
                table: "stocktakings");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktransferitems_items_ItemId",
                table: "stocktransferitems");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktransferitems_stocktransfers_StockTransferId",
                table: "stocktransferitems");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktransfers_aspnetusers_ApprovedByUser",
                table: "stocktransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktransfers_aspnetusers_InitiatedById",
                table: "stocktransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktransfers_locations_FromLocationId",
                table: "stocktransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_stocktransfers_locations_ToLocationId",
                table: "stocktransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_supplierlocations_locations_LocationId",
                table: "supplierlocations");

            migrationBuilder.DropForeignKey(
                name: "FK_supplierlocations_suppliers_SupplierId",
                table: "supplierlocations");

            migrationBuilder.DropForeignKey(
                name: "FK_suppliers_companies_CompanyId",
                table: "suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_suppliers_locations_LocationId",
                table: "suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDelivery_transaction_TransactionId",
                table: "TransactionDelivery");

            migrationBuilder.DropForeignKey(
                name: "FK_transactionitems_items_ItemId",
                table: "transactionitems");

            migrationBuilder.DropForeignKey(
                name: "FK_transactionitems_transaction_TransactionId",
                table: "transactionitems");

            migrationBuilder.DropForeignKey(
                name: "FK_transactionitemsdelivered_items_ItemId",
                table: "transactionitemsdelivered");

            migrationBuilder.DropForeignKey(
                name: "FK_transactionitemsdelivered_transactionitems_TransactionItemId",
                table: "transactionitemsdelivered");

            migrationBuilder.DropForeignKey(
                name: "FK_transactionpayments_transaction_TransactionId",
                table: "transactionpayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transactionpayments",
                table: "transactionpayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transactionitemsdelivered",
                table: "transactionitemsdelivered");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transactionitems",
                table: "transactionitems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transaction",
                table: "transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_suppliers",
                table: "suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_supplierlocations",
                table: "supplierlocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stocktransfers",
                table: "stocktransfers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stocktransferitems",
                table: "stocktransferitems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stocktakings",
                table: "stocktakings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stocktakingitems",
                table: "stocktakingitems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_stocklevels",
                table: "stocklevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_sales",
                table: "sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_purchases",
                table: "purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_positions",
                table: "positions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_locations",
                table: "locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_locationpayments",
                table: "locationpayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_locationmanangement",
                table: "locationmanangement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_items",
                table: "items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_itemlocations",
                table: "itemlocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employees",
                table: "employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_employeelocations",
                table: "employeelocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_customers",
                table: "customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_companies",
                table: "companies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_aspnetusers",
                table: "aspnetusers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_applicationroutes",
                table: "applicationroutes");

            migrationBuilder.RenameTable(
                name: "transactionpayments",
                newName: "TransactionPayments");

            migrationBuilder.RenameTable(
                name: "transactionitemsdelivered",
                newName: "TransactionItemsDelivered");

            migrationBuilder.RenameTable(
                name: "transactionitems",
                newName: "TransactionItems");

            migrationBuilder.RenameTable(
                name: "transaction",
                newName: "Transaction");

            migrationBuilder.RenameTable(
                name: "suppliers",
                newName: "Suppliers");

            migrationBuilder.RenameTable(
                name: "supplierlocations",
                newName: "SupplierLocations");

            migrationBuilder.RenameTable(
                name: "stocktransfers",
                newName: "StockTransfers");

            migrationBuilder.RenameTable(
                name: "stocktransferitems",
                newName: "StockTransferItems");

            migrationBuilder.RenameTable(
                name: "stocktakings",
                newName: "StockTakings");

            migrationBuilder.RenameTable(
                name: "stocktakingitems",
                newName: "StockTakingItems");

            migrationBuilder.RenameTable(
                name: "stocklevels",
                newName: "StockLevels");

            migrationBuilder.RenameTable(
                name: "sales",
                newName: "Sales");

            migrationBuilder.RenameTable(
                name: "purchases",
                newName: "Purchases");

            migrationBuilder.RenameTable(
                name: "positions",
                newName: "Positions");

            migrationBuilder.RenameTable(
                name: "locations",
                newName: "Locations");

            migrationBuilder.RenameTable(
                name: "locationpayments",
                newName: "LocationPayments");

            migrationBuilder.RenameTable(
                name: "locationmanangement",
                newName: "LocationManangement");

            migrationBuilder.RenameTable(
                name: "items",
                newName: "Items");

            migrationBuilder.RenameTable(
                name: "itemlocations",
                newName: "ItemLocations");

            migrationBuilder.RenameTable(
                name: "employees",
                newName: "Employees");

            migrationBuilder.RenameTable(
                name: "employeelocations",
                newName: "EmployeeLocations");

            migrationBuilder.RenameTable(
                name: "customers",
                newName: "Customers");

            migrationBuilder.RenameTable(
                name: "companies",
                newName: "Companies");

            migrationBuilder.RenameTable(
                name: "aspnetusers",
                newName: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "applicationroutes",
                newName: "ApplicationRoutes");

            migrationBuilder.RenameIndex(
                name: "IX_transactionpayments_TransactionId",
                table: "TransactionPayments",
                newName: "IX_TransactionPayments_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_transactionitemsdelivered_TransactionItemId",
                table: "TransactionItemsDelivered",
                newName: "IX_TransactionItemsDelivered_TransactionItemId");

            migrationBuilder.RenameIndex(
                name: "IX_transactionitemsdelivered_ItemId",
                table: "TransactionItemsDelivered",
                newName: "IX_TransactionItemsDelivered_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_transactionitems_TransactionId",
                table: "TransactionItems",
                newName: "IX_TransactionItems_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_transactionitems_ItemId",
                table: "TransactionItems",
                newName: "IX_TransactionItems_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_suppliers_LocationId",
                table: "Suppliers",
                newName: "IX_Suppliers_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_suppliers_Email",
                table: "Suppliers",
                newName: "IX_Suppliers_Email");

            migrationBuilder.RenameIndex(
                name: "IX_suppliers_CompanyId",
                table: "Suppliers",
                newName: "IX_Suppliers_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_supplierlocations_SupplierId",
                table: "SupplierLocations",
                newName: "IX_SupplierLocations_SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_supplierlocations_LocationId",
                table: "SupplierLocations",
                newName: "IX_SupplierLocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_stocktransfers_ToLocationId",
                table: "StockTransfers",
                newName: "IX_StockTransfers_ToLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_stocktransfers_InitiatedById",
                table: "StockTransfers",
                newName: "IX_StockTransfers_InitiatedById");

            migrationBuilder.RenameIndex(
                name: "IX_stocktransfers_FromLocationId",
                table: "StockTransfers",
                newName: "IX_StockTransfers_FromLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_stocktransfers_ApprovedByUser",
                table: "StockTransfers",
                newName: "IX_StockTransfers_ApprovedByUser");

            migrationBuilder.RenameIndex(
                name: "IX_stocktransferitems_StockTransferId",
                table: "StockTransferItems",
                newName: "IX_StockTransferItems_StockTransferId");

            migrationBuilder.RenameIndex(
                name: "IX_stocktransferitems_ItemId",
                table: "StockTransferItems",
                newName: "IX_StockTransferItems_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_stocktakings_VerifiedBy",
                table: "StockTakings",
                newName: "IX_StockTakings_VerifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_stocktakings_LocationId",
                table: "StockTakings",
                newName: "IX_StockTakings_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_stocktakings_ConductedBy",
                table: "StockTakings",
                newName: "IX_StockTakings_ConductedBy");

            migrationBuilder.RenameIndex(
                name: "IX_stocktakingitems_StockTakingId",
                table: "StockTakingItems",
                newName: "IX_StockTakingItems_StockTakingId");

            migrationBuilder.RenameIndex(
                name: "IX_stocktakingitems_ItemId",
                table: "StockTakingItems",
                newName: "IX_StockTakingItems_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_stocklevels_LocationId",
                table: "StockLevels",
                newName: "IX_StockLevels_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_stocklevels_ItemId",
                table: "StockLevels",
                newName: "IX_StockLevels_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_sales_TransactionId",
                table: "Sales",
                newName: "IX_Sales_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_sales_SalesPersonId",
                table: "Sales",
                newName: "IX_Sales_SalesPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_sales_LocationId",
                table: "Sales",
                newName: "IX_Sales_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_sales_CustomerId",
                table: "Sales",
                newName: "IX_Sales_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_TransactionId",
                table: "Purchases",
                newName: "IX_Purchases_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_SupplierId",
                table: "Purchases",
                newName: "IX_Purchases_SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_PurcasedById",
                table: "Purchases",
                newName: "IX_Purchases_PurcasedById");

            migrationBuilder.RenameIndex(
                name: "IX_purchases_LocationId",
                table: "Purchases",
                newName: "IX_Purchases_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_positions_CompanyId",
                table: "Positions",
                newName: "IX_Positions_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_locations_TypeOfLocation",
                table: "Locations",
                newName: "IX_Locations_TypeOfLocation");

            migrationBuilder.RenameIndex(
                name: "IX_locations_Email",
                table: "Locations",
                newName: "IX_Locations_Email");

            migrationBuilder.RenameIndex(
                name: "IX_locations_CompanyId_TypeOfLocation",
                table: "Locations",
                newName: "IX_Locations_CompanyId_TypeOfLocation");

            migrationBuilder.RenameIndex(
                name: "IX_locations_CompanyId",
                table: "Locations",
                newName: "IX_Locations_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_locationpayments_ReceiverId",
                table: "LocationPayments",
                newName: "IX_LocationPayments_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_locationpayments_LocationId",
                table: "LocationPayments",
                newName: "IX_LocationPayments_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_locationmanangement_ManagerId",
                table: "LocationManangement",
                newName: "IX_LocationManangement_ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_locationmanangement_LocationId",
                table: "LocationManangement",
                newName: "IX_LocationManangement_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_items_CompanyId",
                table: "Items",
                newName: "IX_Items_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_items_Category",
                table: "Items",
                newName: "IX_Items_Category");

            migrationBuilder.RenameIndex(
                name: "IX_itemlocations_LocationId",
                table: "ItemLocations",
                newName: "IX_ItemLocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_itemlocations_ItemId_LocationId",
                table: "ItemLocations",
                newName: "IX_ItemLocations_ItemId_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_itemlocations_ItemId",
                table: "ItemLocations",
                newName: "IX_ItemLocations_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_PositionId",
                table: "Employees",
                newName: "IX_Employees_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_employees_Email",
                table: "Employees",
                newName: "IX_Employees_Email");

            migrationBuilder.RenameIndex(
                name: "IX_employees_CompanyId",
                table: "Employees",
                newName: "IX_Employees_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_employeelocations_LocationId",
                table: "EmployeeLocations",
                newName: "IX_EmployeeLocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_employeelocations_EmployeeId_LocationId",
                table: "EmployeeLocations",
                newName: "IX_EmployeeLocations_EmployeeId_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_employeelocations_EmployeeId",
                table: "EmployeeLocations",
                newName: "IX_EmployeeLocations_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_customers_LocationId",
                table: "Customers",
                newName: "IX_Customers_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_customers_Email",
                table: "Customers",
                newName: "IX_Customers_Email");

            migrationBuilder.RenameIndex(
                name: "IX_companies_PhoneNumber",
                table: "Companies",
                newName: "IX_Companies_PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_companies_Email",
                table: "Companies",
                newName: "IX_Companies_Email");

            migrationBuilder.RenameIndex(
                name: "IX_aspnetusers_Email",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_Email");

            migrationBuilder.RenameIndex(
                name: "IX_aspnetusers_CompanyId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_applicationroutes_ParentId",
                table: "ApplicationRoutes",
                newName: "IX_ApplicationRoutes_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionPayments",
                table: "TransactionPayments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionItemsDelivered",
                table: "TransactionItemsDelivered",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TransactionItems",
                table: "TransactionItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SupplierLocations",
                table: "SupplierLocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockTransfers",
                table: "StockTransfers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockTransferItems",
                table: "StockTransferItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockTakings",
                table: "StockTakings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockTakingItems",
                table: "StockTakingItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StockLevels",
                table: "StockLevels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sales",
                table: "Sales",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Positions",
                table: "Positions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Locations",
                table: "Locations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationPayments",
                table: "LocationPayments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationManangement",
                table: "LocationManangement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Items",
                table: "Items",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemLocations",
                table: "ItemLocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employees",
                table: "Employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeLocations",
                table: "EmployeeLocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Customers",
                table: "Customers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Companies",
                table: "Companies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationRoutes",
                table: "ApplicationRoutes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationRoutes_ApplicationRoutes_ParentId",
                table: "ApplicationRoutes",
                column: "ParentId",
                principalTable: "ApplicationRoutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Locations_LocationId",
                table: "Customers",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLocations_Employees_EmployeeId",
                table: "EmployeeLocations",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeLocations_Locations_LocationId",
                table: "EmployeeLocations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                table: "Employees",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Positions_PositionId",
                table: "Employees",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLocations_Items_ItemId",
                table: "ItemLocations",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLocations_Locations_LocationId",
                table: "ItemLocations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Companies_CompanyId",
                table: "Items",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationManangement_Employees_ManagerId",
                table: "LocationManangement",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationManangement_Locations_LocationId",
                table: "LocationManangement",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationPayments_Employees_ReceiverId",
                table: "LocationPayments",
                column: "ReceiverId",
                principalTable: "Employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LocationPayments_Locations_LocationId",
                table: "LocationPayments",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Locations_Companies_CompanyId",
                table: "Locations",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_Companies_CompanyId",
                table: "Positions",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_AspNetUsers_PurcasedById",
                table: "Purchases",
                column: "PurcasedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Locations_LocationId",
                table: "Purchases",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Suppliers_SupplierId",
                table: "Purchases",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Transaction_TransactionId",
                table: "Purchases",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_AspNetUsers_SalesPersonId",
                table: "Sales",
                column: "SalesPersonId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Transaction_TransactionId",
                table: "Sales",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLevels_Items_ItemId",
                table: "StockLevels",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLevels_Locations_LocationId",
                table: "StockLevels",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakingItems_Items_ItemId",
                table: "StockTakingItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakingItems_StockTakings_StockTakingId",
                table: "StockTakingItems",
                column: "StockTakingId",
                principalTable: "StockTakings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakings_AspNetUsers_ConductedBy",
                table: "StockTakings",
                column: "ConductedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakings_AspNetUsers_VerifiedBy",
                table: "StockTakings",
                column: "VerifiedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTakings_Locations_LocationId",
                table: "StockTakings",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_Items_ItemId",
                table: "StockTransferItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransferItems_StockTransfers_StockTransferId",
                table: "StockTransferItems",
                column: "StockTransferId",
                principalTable: "StockTransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApprovedByUser",
                table: "StockTransfers",
                column: "ApprovedByUser",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_AspNetUsers_InitiatedById",
                table: "StockTransfers",
                column: "InitiatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Locations_FromLocationId",
                table: "StockTransfers",
                column: "FromLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransfers_Locations_ToLocationId",
                table: "StockTransfers",
                column: "ToLocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierLocations_Locations_LocationId",
                table: "SupplierLocations",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierLocations_Suppliers_SupplierId",
                table: "SupplierLocations",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Companies_CompanyId",
                table: "Suppliers",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Suppliers_Locations_LocationId",
                table: "Suppliers",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDelivery_Transaction_TransactionId",
                table: "TransactionDelivery",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItems_Items_ItemId",
                table: "TransactionItems",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItems_Transaction_TransactionId",
                table: "TransactionItems",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemsDelivered_Items_ItemId",
                table: "TransactionItemsDelivered",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionItems_TransactionItemId",
                table: "TransactionItemsDelivered",
                column: "TransactionItemId",
                principalTable: "TransactionItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionPayments_Transaction_TransactionId",
                table: "TransactionPayments",
                column: "TransactionId",
                principalTable: "Transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationRoutes_ApplicationRoutes_ParentId",
                table: "ApplicationRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Companies_CompanyId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Locations_LocationId",
                table: "Customers");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLocations_Employees_EmployeeId",
                table: "EmployeeLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeLocations_Locations_LocationId",
                table: "EmployeeLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Companies_CompanyId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Positions_PositionId",
                table: "Employees");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemLocations_Items_ItemId",
                table: "ItemLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemLocations_Locations_LocationId",
                table: "ItemLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Companies_CompanyId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationManangement_Employees_ManagerId",
                table: "LocationManangement");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationManangement_Locations_LocationId",
                table: "LocationManangement");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationPayments_Employees_ReceiverId",
                table: "LocationPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_LocationPayments_Locations_LocationId",
                table: "LocationPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_Locations_Companies_CompanyId",
                table: "Locations");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_Companies_CompanyId",
                table: "Positions");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_AspNetUsers_PurcasedById",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Locations_LocationId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Suppliers_SupplierId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Transaction_TransactionId",
                table: "Purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_AspNetUsers_SalesPersonId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Locations_LocationId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Transaction_TransactionId",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLevels_Items_ItemId",
                table: "StockLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLevels_Locations_LocationId",
                table: "StockLevels");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTakingItems_Items_ItemId",
                table: "StockTakingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTakingItems_StockTakings_StockTakingId",
                table: "StockTakingItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTakings_AspNetUsers_ConductedBy",
                table: "StockTakings");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTakings_AspNetUsers_VerifiedBy",
                table: "StockTakings");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTakings_Locations_LocationId",
                table: "StockTakings");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_Items_ItemId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransferItems_StockTransfers_StockTransferId",
                table: "StockTransferItems");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_AspNetUsers_ApprovedByUser",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_AspNetUsers_InitiatedById",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Locations_FromLocationId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransfers_Locations_ToLocationId",
                table: "StockTransfers");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierLocations_Locations_LocationId",
                table: "SupplierLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_SupplierLocations_Suppliers_SupplierId",
                table: "SupplierLocations");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Companies_CompanyId",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_Suppliers_Locations_LocationId",
                table: "Suppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionDelivery_Transaction_TransactionId",
                table: "TransactionDelivery");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItems_Items_ItemId",
                table: "TransactionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItems_Transaction_TransactionId",
                table: "TransactionItems");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_Items_ItemId",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionItemsDelivered_TransactionItems_TransactionItemId",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropForeignKey(
                name: "FK_TransactionPayments_Transaction_TransactionId",
                table: "TransactionPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionPayments",
                table: "TransactionPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionItemsDelivered",
                table: "TransactionItemsDelivered");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TransactionItems",
                table: "TransactionItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transaction",
                table: "Transaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Suppliers",
                table: "Suppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SupplierLocations",
                table: "SupplierLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockTransfers",
                table: "StockTransfers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockTransferItems",
                table: "StockTransferItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockTakings",
                table: "StockTakings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockTakingItems",
                table: "StockTakingItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StockLevels",
                table: "StockLevels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sales",
                table: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Purchases",
                table: "Purchases");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Positions",
                table: "Positions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Locations",
                table: "Locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationPayments",
                table: "LocationPayments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationManangement",
                table: "LocationManangement");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Items",
                table: "Items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemLocations",
                table: "ItemLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Employees",
                table: "Employees");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeLocations",
                table: "EmployeeLocations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Customers",
                table: "Customers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Companies",
                table: "Companies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AspNetUsers",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationRoutes",
                table: "ApplicationRoutes");

            migrationBuilder.RenameTable(
                name: "TransactionPayments",
                newName: "transactionpayments");

            migrationBuilder.RenameTable(
                name: "TransactionItemsDelivered",
                newName: "transactionitemsdelivered");

            migrationBuilder.RenameTable(
                name: "TransactionItems",
                newName: "transactionitems");

            migrationBuilder.RenameTable(
                name: "Transaction",
                newName: "transaction");

            migrationBuilder.RenameTable(
                name: "Suppliers",
                newName: "suppliers");

            migrationBuilder.RenameTable(
                name: "SupplierLocations",
                newName: "supplierlocations");

            migrationBuilder.RenameTable(
                name: "StockTransfers",
                newName: "stocktransfers");

            migrationBuilder.RenameTable(
                name: "StockTransferItems",
                newName: "stocktransferitems");

            migrationBuilder.RenameTable(
                name: "StockTakings",
                newName: "stocktakings");

            migrationBuilder.RenameTable(
                name: "StockTakingItems",
                newName: "stocktakingitems");

            migrationBuilder.RenameTable(
                name: "StockLevels",
                newName: "stocklevels");

            migrationBuilder.RenameTable(
                name: "Sales",
                newName: "sales");

            migrationBuilder.RenameTable(
                name: "Purchases",
                newName: "purchases");

            migrationBuilder.RenameTable(
                name: "Positions",
                newName: "positions");

            migrationBuilder.RenameTable(
                name: "Locations",
                newName: "locations");

            migrationBuilder.RenameTable(
                name: "LocationPayments",
                newName: "locationpayments");

            migrationBuilder.RenameTable(
                name: "LocationManangement",
                newName: "locationmanangement");

            migrationBuilder.RenameTable(
                name: "Items",
                newName: "items");

            migrationBuilder.RenameTable(
                name: "ItemLocations",
                newName: "itemlocations");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "employees");

            migrationBuilder.RenameTable(
                name: "EmployeeLocations",
                newName: "employeelocations");

            migrationBuilder.RenameTable(
                name: "Customers",
                newName: "customers");

            migrationBuilder.RenameTable(
                name: "Companies",
                newName: "companies");

            migrationBuilder.RenameTable(
                name: "AspNetUsers",
                newName: "aspnetusers");

            migrationBuilder.RenameTable(
                name: "ApplicationRoutes",
                newName: "applicationroutes");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionPayments_TransactionId",
                table: "transactionpayments",
                newName: "IX_transactionpayments_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionItemsDelivered_TransactionItemId",
                table: "transactionitemsdelivered",
                newName: "IX_transactionitemsdelivered_TransactionItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionItemsDelivered_ItemId",
                table: "transactionitemsdelivered",
                newName: "IX_transactionitemsdelivered_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionItems_TransactionId",
                table: "transactionitems",
                newName: "IX_transactionitems_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TransactionItems_ItemId",
                table: "transactionitems",
                newName: "IX_transactionitems_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Suppliers_LocationId",
                table: "suppliers",
                newName: "IX_suppliers_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Suppliers_Email",
                table: "suppliers",
                newName: "IX_suppliers_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Suppliers_CompanyId",
                table: "suppliers",
                newName: "IX_suppliers_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierLocations_SupplierId",
                table: "supplierlocations",
                newName: "IX_supplierlocations_SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_SupplierLocations_LocationId",
                table: "supplierlocations",
                newName: "IX_supplierlocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_ToLocationId",
                table: "stocktransfers",
                newName: "IX_stocktransfers_ToLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_InitiatedById",
                table: "stocktransfers",
                newName: "IX_stocktransfers_InitiatedById");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_FromLocationId",
                table: "stocktransfers",
                newName: "IX_stocktransfers_FromLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransfers_ApprovedByUser",
                table: "stocktransfers",
                newName: "IX_stocktransfers_ApprovedByUser");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransferItems_StockTransferId",
                table: "stocktransferitems",
                newName: "IX_stocktransferitems_StockTransferId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTransferItems_ItemId",
                table: "stocktransferitems",
                newName: "IX_stocktransferitems_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTakings_VerifiedBy",
                table: "stocktakings",
                newName: "IX_stocktakings_VerifiedBy");

            migrationBuilder.RenameIndex(
                name: "IX_StockTakings_LocationId",
                table: "stocktakings",
                newName: "IX_stocktakings_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTakings_ConductedBy",
                table: "stocktakings",
                newName: "IX_stocktakings_ConductedBy");

            migrationBuilder.RenameIndex(
                name: "IX_StockTakingItems_StockTakingId",
                table: "stocktakingitems",
                newName: "IX_stocktakingitems_StockTakingId");

            migrationBuilder.RenameIndex(
                name: "IX_StockTakingItems_ItemId",
                table: "stocktakingitems",
                newName: "IX_stocktakingitems_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_StockLevels_LocationId",
                table: "stocklevels",
                newName: "IX_stocklevels_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_StockLevels_ItemId",
                table: "stocklevels",
                newName: "IX_stocklevels_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_TransactionId",
                table: "sales",
                newName: "IX_sales_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_SalesPersonId",
                table: "sales",
                newName: "IX_sales_SalesPersonId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_LocationId",
                table: "sales",
                newName: "IX_sales_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_CustomerId",
                table: "sales",
                newName: "IX_sales_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_TransactionId",
                table: "purchases",
                newName: "IX_purchases_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_SupplierId",
                table: "purchases",
                newName: "IX_purchases_SupplierId");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_PurcasedById",
                table: "purchases",
                newName: "IX_purchases_PurcasedById");

            migrationBuilder.RenameIndex(
                name: "IX_Purchases_LocationId",
                table: "purchases",
                newName: "IX_purchases_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Positions_CompanyId",
                table: "positions",
                newName: "IX_positions_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Locations_TypeOfLocation",
                table: "locations",
                newName: "IX_locations_TypeOfLocation");

            migrationBuilder.RenameIndex(
                name: "IX_Locations_Email",
                table: "locations",
                newName: "IX_locations_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Locations_CompanyId_TypeOfLocation",
                table: "locations",
                newName: "IX_locations_CompanyId_TypeOfLocation");

            migrationBuilder.RenameIndex(
                name: "IX_Locations_CompanyId",
                table: "locations",
                newName: "IX_locations_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationPayments_ReceiverId",
                table: "locationpayments",
                newName: "IX_locationpayments_ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationPayments_LocationId",
                table: "locationpayments",
                newName: "IX_locationpayments_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationManangement_ManagerId",
                table: "locationmanangement",
                newName: "IX_locationmanangement_ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_LocationManangement_LocationId",
                table: "locationmanangement",
                newName: "IX_locationmanangement_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_CompanyId",
                table: "items",
                newName: "IX_items_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_Items_Category",
                table: "items",
                newName: "IX_items_Category");

            migrationBuilder.RenameIndex(
                name: "IX_ItemLocations_LocationId",
                table: "itemlocations",
                newName: "IX_itemlocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemLocations_ItemId_LocationId",
                table: "itemlocations",
                newName: "IX_itemlocations_ItemId_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_ItemLocations_ItemId",
                table: "itemlocations",
                newName: "IX_itemlocations_ItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_PositionId",
                table: "employees",
                newName: "IX_employees_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_Email",
                table: "employees",
                newName: "IX_employees_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Employees_CompanyId",
                table: "employees",
                newName: "IX_employees_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeLocations_LocationId",
                table: "employeelocations",
                newName: "IX_employeelocations_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeLocations_EmployeeId_LocationId",
                table: "employeelocations",
                newName: "IX_employeelocations_EmployeeId_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_EmployeeLocations_EmployeeId",
                table: "employeelocations",
                newName: "IX_employeelocations_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_LocationId",
                table: "customers",
                newName: "IX_customers_LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_Customers_Email",
                table: "customers",
                newName: "IX_customers_Email");

            migrationBuilder.RenameIndex(
                name: "IX_Companies_PhoneNumber",
                table: "companies",
                newName: "IX_companies_PhoneNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Companies_Email",
                table: "companies",
                newName: "IX_companies_Email");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_Email",
                table: "aspnetusers",
                newName: "IX_aspnetusers_Email");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_CompanyId",
                table: "aspnetusers",
                newName: "IX_aspnetusers_CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationRoutes_ParentId",
                table: "applicationroutes",
                newName: "IX_applicationroutes_ParentId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transactionpayments",
                table: "transactionpayments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transactionitemsdelivered",
                table: "transactionitemsdelivered",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transactionitems",
                table: "transactionitems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transaction",
                table: "transaction",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_suppliers",
                table: "suppliers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_supplierlocations",
                table: "supplierlocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stocktransfers",
                table: "stocktransfers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stocktransferitems",
                table: "stocktransferitems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stocktakings",
                table: "stocktakings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stocktakingitems",
                table: "stocktakingitems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_stocklevels",
                table: "stocklevels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sales",
                table: "sales",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_purchases",
                table: "purchases",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_positions",
                table: "positions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_locations",
                table: "locations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_locationpayments",
                table: "locationpayments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_locationmanangement",
                table: "locationmanangement",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_items",
                table: "items",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_itemlocations",
                table: "itemlocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employees",
                table: "employees",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employeelocations",
                table: "employeelocations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_customers",
                table: "customers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_companies",
                table: "companies",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_aspnetusers",
                table: "aspnetusers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_applicationroutes",
                table: "applicationroutes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_applicationroutes_applicationroutes_ParentId",
                table: "applicationroutes",
                column: "ParentId",
                principalTable: "applicationroutes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_aspnetusers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_aspnetusers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_aspnetusers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_aspnetusers_companies_CompanyId",
                table: "aspnetusers",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_aspnetusers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_customers_locations_LocationId",
                table: "customers",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_employeelocations_employees_EmployeeId",
                table: "employeelocations",
                column: "EmployeeId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employeelocations_locations_LocationId",
                table: "employeelocations",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_companies_CompanyId",
                table: "employees",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_employees_positions_PositionId",
                table: "employees",
                column: "PositionId",
                principalTable: "positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_itemlocations_items_ItemId",
                table: "itemlocations",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_itemlocations_locations_LocationId",
                table: "itemlocations",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_items_companies_CompanyId",
                table: "items",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_locationmanangement_employees_ManagerId",
                table: "locationmanangement",
                column: "ManagerId",
                principalTable: "employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_locationmanangement_locations_LocationId",
                table: "locationmanangement",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_locationpayments_employees_ReceiverId",
                table: "locationpayments",
                column: "ReceiverId",
                principalTable: "employees",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_locationpayments_locations_LocationId",
                table: "locationpayments",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_locations_companies_CompanyId",
                table: "locations",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_positions_companies_CompanyId",
                table: "positions",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_aspnetusers_PurcasedById",
                table: "purchases",
                column: "PurcasedById",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_locations_LocationId",
                table: "purchases",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_suppliers_SupplierId",
                table: "purchases",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_transaction_TransactionId",
                table: "purchases",
                column: "TransactionId",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_aspnetusers_SalesPersonId",
                table: "sales",
                column: "SalesPersonId",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_customers_CustomerId",
                table: "sales",
                column: "CustomerId",
                principalTable: "customers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_sales_locations_LocationId",
                table: "sales",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_sales_transaction_TransactionId",
                table: "sales",
                column: "TransactionId",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocklevels_items_ItemId",
                table: "stocklevels",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocklevels_locations_LocationId",
                table: "stocklevels",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakingitems_items_ItemId",
                table: "stocktakingitems",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakingitems_stocktakings_StockTakingId",
                table: "stocktakingitems",
                column: "StockTakingId",
                principalTable: "stocktakings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakings_aspnetusers_ConductedBy",
                table: "stocktakings",
                column: "ConductedBy",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakings_aspnetusers_VerifiedBy",
                table: "stocktakings",
                column: "VerifiedBy",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktakings_locations_LocationId",
                table: "stocktakings",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktransferitems_items_ItemId",
                table: "stocktransferitems",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktransferitems_stocktransfers_StockTransferId",
                table: "stocktransferitems",
                column: "StockTransferId",
                principalTable: "stocktransfers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktransfers_aspnetusers_ApprovedByUser",
                table: "stocktransfers",
                column: "ApprovedByUser",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktransfers_aspnetusers_InitiatedById",
                table: "stocktransfers",
                column: "InitiatedById",
                principalTable: "aspnetusers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktransfers_locations_FromLocationId",
                table: "stocktransfers",
                column: "FromLocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stocktransfers_locations_ToLocationId",
                table: "stocktransfers",
                column: "ToLocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_supplierlocations_locations_LocationId",
                table: "supplierlocations",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_supplierlocations_suppliers_SupplierId",
                table: "supplierlocations",
                column: "SupplierId",
                principalTable: "suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_suppliers_companies_CompanyId",
                table: "suppliers",
                column: "CompanyId",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_suppliers_locations_LocationId",
                table: "suppliers",
                column: "LocationId",
                principalTable: "locations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TransactionDelivery_transaction_TransactionId",
                table: "TransactionDelivery",
                column: "TransactionId",
                principalTable: "transaction",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactionitems_items_ItemId",
                table: "transactionitems",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactionitems_transaction_TransactionId",
                table: "transactionitems",
                column: "TransactionId",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactionitemsdelivered_items_ItemId",
                table: "transactionitemsdelivered",
                column: "ItemId",
                principalTable: "items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_transactionitemsdelivered_transactionitems_TransactionItemId",
                table: "transactionitemsdelivered",
                column: "TransactionItemId",
                principalTable: "transactionitems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_transactionpayments_transaction_TransactionId",
                table: "transactionpayments",
                column: "TransactionId",
                principalTable: "transaction",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
