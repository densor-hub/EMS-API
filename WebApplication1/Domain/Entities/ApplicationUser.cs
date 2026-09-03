using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Domain.DTO;

namespace WebApplication1.Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public UserRight UserRight { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool? Status { get; set; } = true;
        public int IncrementalId { get; set; }
        // Foreign key to Company
        public Guid? CompanyId { get; set; } // Nullable to allow creation of user account before company set up on fresh account registration
        public virtual  Company Company { get; set; }

        // Refresh token properties
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public ICollection<UserRoutes> UserRoutes { get; set; }
        public ICollection<Sale> Sales { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
        public ICollection<StockLockDownRequest> StockLockDownRequests { get; set; }
        public ICollection<StockTakeItemSubmission> StockTakeItemSubmissions { get; set; }
        public ICollection<StockTakeItemSubmission> StockTakeItemVerifications { get; set; }
        //public ICollection<StockTransfer> InitiatedStockTransfers { get; set; }
        //public ICollection<StockTransfer> ApprovedStockTransfers { get; set; }
        public ICollection<StockLockDownComment> StockLockDownComments { get; set; }
        public ICollection<TransactionComment> TransactionComments { get; set; }
        public ICollection<SupplierItemCostPrice> SupplierItemCostPrice { get; set; }
        public ICollection<SaleTransDeliveryRequest> TransactionDeliveryRequests { get; set; }
        public ICollection<SaleTransDeliveryRequest> TransactionDeliveryRequestUpdates { get; set; }
        public ApplicationUser()
        {
            
        }

        public void setCompany(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
}
