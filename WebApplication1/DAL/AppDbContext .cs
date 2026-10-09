using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;
using WebApplication1.Domain.Entities;
using WebApplication1.Services.Hubs.Entities;

namespace WebApplication1.DAL
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<AuditSnapshot> AuditSnapshots { get; set; }

        public DbSet<Company> Companies { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<EmployeeDisbursement> EmployeeDisbursements { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeLocation> EmployeeLocations { get; set; }
        //public DbSet<LocationManangement> LocationManangement { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemLocation> ItemLocations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierItemCostPrice> SupplierItemCostPrices { get; set; }
        public DbSet<SupplierLocation> SupplierLocations { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Payment> TransactionPayments { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }
        public DbSet<TransactionTransportation> TransactionTransportations { get; set; }
        public DbSet<SaleTransDeliveryRequest> SaleTransDeliveryRequests { get; set; }
        public DbSet<TransactionItemDelivered> TransactionItemsDelivered { get; set; }
        public DbSet<TransactionItemReceived> TransactionItemReceived { get; set; }
        public DbSet <TransactionItemReversal> TransactionItemReversals { get; set; } 
        public DbSet<StockLockDownRequest> StockLockDownRequests { get; set; }
        public DbSet<StockLockDownItem> StockLockDownItems { get; set; }
        public DbSet<StockTakeItemSubmission> StockTakeItemSubmissions { get; set; }
        public DbSet<StockLockDownComment> StockLockDownComments { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<StockLevel> StockLevels { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<QueuedEmail> QueuedEmails { get; set; }
        public DbSet <PinCode> TransactionCodes { get; set; }
        public DbSet<TransactionComment> Comments { get; set; }
        public DbSet<ConfirmationCodes> ConfirmationCodes { get; set; }
        public DbSet<ApplicationRoute> ApplicationRoutes { get; set; }
        public DbSet<PositionRoutes> PositionRoutes { get; set; }
        public DbSet<UserRoutes> UserRoutes { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<FinancialServiceProvider> FinancialServiceProviders { get; set; }
        public DbSet<FinancialServiceProviderContactPerson> FinancialServiceProviderContactPersons { get; set; }
        public DbSet<FinancialServiceDisbursement> FinancialServiceDisbursement { get; set; }
        public DbSet<FinancialServiceDisbursementContactPerson> FinancialServiceDisbursementContactPersons { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<PaymentConfirmationToken> PaymentConfirmationTokens { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<DailyTransactionCounter> DailyTransactionCounters { get; set; }
        public DbSet<LocationSaleSequence> LocationSaleSequences { get; set; }
        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}