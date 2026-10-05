// Services/Implementations/PurchaseService.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;
using WebApplication1.Domain.Repository;
using WebApplication1.DAL;


namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class PurchaseService : IPurchaseService
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ITransactionService _transactionService;
        


        public PurchaseService(
            AppDbContext context,
             IUserRepository userRepository,
               ISupplierRepository supplierRepository,
               ITransactionService transactionService
            )
        {
            _context = context;
            _userRepository = userRepository;
            _supplierRepository = supplierRepository;
            _transactionService = transactionService;
        }

        public async Task<GetPurchaseDto> GetByIdAsync(Guid id, GeneralStatus generalStatus)
        {
            var purchase = await _context.Purchases
                        .Include(p => p.Supplier)
                        .Include(p => p.PurcasedBy)
                        .Include(x => x.Transaction)
                                .ThenInclude(x=> x.TransactionPayments)
                         .Include(x => x.Transaction)
                                .ThenInclude(t => t.TransactionItems)
                                    .ThenInclude(ti => ti.TransactionItemsDelivered)

                .FirstOrDefaultAsync(p => p.Id == id && p.Transaction.GeneralStatus == generalStatus);

            if (purchase == null)
                return null;

            return new GetPurchaseDto
            {
                Id = purchase.Id,  // Use the actual Purchase Id
                TransactionId = purchase.TransactionId,  // Add separate field if needed
                TransactionCode = purchase.Transaction.TransactionNumber,
                LocationName = purchase.Transaction?.Location?.Name,
                SupplierId = purchase.SupplierId,
                SupplierName = $"{purchase.Supplier?.FirstName ?? ""} {purchase.Supplier?.LastName ?? ""}".Trim(),
                TransactionBy = purchase.PurcasedBy?.FullName,
                CreatedAt = purchase.Transaction.CreatedAt,
                TransactionDate = purchase.Transaction?.TransactionDate ?? DateTime.MinValue,
                TotalAmount = purchase.Transaction?.TotalAmount ?? 0,
                PaidAmount = purchase.Transaction?.TransactionPayments?.Sum(x => x.Amount) ?? 0,
                Balance = (purchase.Transaction?.TotalAmount ?? 0) -
                                  (purchase.Transaction?.TransactionPayments?.Sum(x => x.Amount) ?? 0)
            };
        }

        public async Task<IEnumerable<GetPurchaseDto>> GetAllAsync(Guid locationId, GeneralStatus generalStatus, Guid? supplierId = null, Guid? salesPersonId = null)
        {
            var query = from purchase in _context.Purchases
                        .Include(p => p.Supplier)
                        .Include(p => p.PurcasedBy)
                        .Include(x => x.Transaction)
                                .ThenInclude(t => t.TransactionItems)
                                    .ThenInclude(ti => ti.TransactionItemsDelivered)
                        .Where(x => x.Transaction.LocationId == locationId 
                                // && x.Transaction.GeneralStatus == generalStatus 
                                && (supplierId != null ? x.SupplierId == supplierId : true)
                               && (int)x.GeneralStatus   == (int) generalStatus
                        && (salesPersonId != null && salesPersonId != Guid.Empty ? x.PurcasedById == salesPersonId.ToString() : true)
                        )
                        .AsNoTracking()

                        select new GetPurchaseDto
                        {
                            Id = purchase.Id,
                            TransactionId = purchase.TransactionId,
                            TransactionCode = purchase.Transaction.TransactionNumber,
                            LocationName = purchase.Transaction.Location != null ? purchase.Transaction.Location.Name : "",
                            SupplierId = purchase.SupplierId,
                            SupplierName = purchase.Supplier != null
                                ? purchase.Supplier.FirstName + " " + purchase.Supplier.LastName
                                : "",
                            TransactionBy = purchase.PurcasedBy != null ? purchase.PurcasedBy.FullName : "",
                            CreatedAt = purchase.Transaction.CreatedAt,
                            TransactionDate = purchase.Transaction != null ? purchase.Transaction.TransactionDate : DateTime.MinValue,
                            TotalAmount = purchase.Transaction != null ? purchase.Transaction.TotalAmount : 0,
                            PaidAmount = purchase.Transaction.TransactionPayments
                                .Where(tp => tp.TransactionId == purchase.TransactionId)
                                .Sum(tp => tp.Amount)
                        };

            return await query.ToListAsync();
        }

        public async Task ManagerCheck( UpdatePurchaseDto updateDto)
        {
            // 1. Fix: missing closing paren + Enum.IsDefined(TYPE, value) is the correct overload
            var validStatus = Enum.IsDefined(typeof(GeneralStatus), updateDto.Status);
            if (!validStatus) throw new Exception("Invalid status submitted");

            var allowedStatus = new List<GeneralStatus> { GeneralStatus.Approved, GeneralStatus.Declined };

            if (allowedStatus.Contains(updateDto.Status) == false) throw new Exception("Invalid status submitted");

            // 2. Fix: include related entities if needed (Transaction for the comment)
            var purchase = await _context.Purchases
                .Include(x => x.Transaction)
                .Where(x => x.TransactionId == updateDto.TransactionId)
                .FirstOrDefaultAsync();

            if (purchase == null) throw new Exception("Purchase not found");

            if (purchase.GeneralStatus != GeneralStatus.Initiated)
            {
                throw new Exception("Invalid status submitted");
            }

            var user = await _userRepository.GetUserByRefreshTokenAsync();

            // 3. Fix: guard against null user before using user.Id / user.FullName
            if (user == null) throw new Exception("User not found");

            if (!string.IsNullOrWhiteSpace(updateDto.Remarks))
            {
                var comment = TransactionComment.Create(
                    Guid.NewGuid(),
                    purchase.TransactionId,
                    purchase.Transaction.TransactionType.ToString(),
                    "1",
                    updateDto.Remarks,
                    DateTime.UtcNow,
                    user.Id,
                    user.FullName
                );
                _context.Comments.Add(comment);
            }

            // 4. Fix: user.Id is already a string GUID — no need to re-parse unless the property expects Guid
            purchase.Update(updateDto.Status, Guid.Parse(user.Id), DateTime.UtcNow);
            _context.Purchases.Update(purchase);

            await _context.SaveChangesAsync();
        }
    }
}