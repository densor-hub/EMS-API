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

        public DbSet<Company> Companies { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<LocationPayments> LocationPayments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeLocation> EmployeeLocations { get; set; }
        public DbSet<LocationManangement> LocationManangement { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<ItemLocation> ItemLocations { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierLocation> SupplierLocations { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<TransactionPayment> TransactionPayments { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }
        public DbSet<TransactionTransportation> TransactionTransportations { get; set; }
        //public DbSet<TransactionDelivery> TransactionDeliveries { get; set; }
        public DbSet<TransactionItemDelivered> TransactionItemsDelivered { get; set; }
        public DbSet<StockTaking> StockTakings { get; set; }
        public DbSet<StockTakingItem> StockTakingItems { get; set; }
        public DbSet<StockTransfer> StockTransfers { get; set; }
        public DbSet<StockTransferItem> StockTransferItems { get; set; }
        public DbSet<StockTransferItemsDelivered> StockTransferItemsDelivered { get; set; }
        public DbSet<StockLevel> StockLevels { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<QueuedEmail> QueuedEmails { get; set; }
        public DbSet <PinCode> TransactionCodes { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<ConfirmationCodes> ConfirmationCodes { get; set; }
        public DbSet<ApplicationRoutes> ApplicationRoutes { get; set; }
        public DbSet<PositionRoutes> PositionRoutes { get; set; }
        public DbSet<UserRoutes> UserRoutes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}