using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool? Status { get; set; } = true;
        // Foreign key to Company
        public Guid? CompanyId { get; set; } // Nullable if user might not belong to a company
        public virtual Company? Company { get; set; }

        // Refresh token properties
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
       
        public ICollection<UserRoutes> UserRoutes { get; set; }
        public ICollection<Sale> Sales { get; set; }
        public ICollection<Purchase> Purchases { get; set; }
        public ICollection<StockTaking> ConductedStockTakings { get; set; }
        public ICollection<StockTaking> VerifiedStockTakings { get; set; }
        public ICollection<StockTransfer> InitiatedStockTransfers { get; set; }
        public ICollection<StockTransfer> ApprovedStockTransfers { get; set; }
        public ApplicationUser()
        {
            
        }

        public void setCompany(Guid companyId)
        {
            CompanyId = companyId;
        }
    }
}
