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
                        .Where(x => x.Transaction.LocationId == locationId && x.Transaction.GeneralStatus == generalStatus && (supplierId != null ? x.SupplierId == supplierId : true)
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


        //public async Task<GetPurchaseDto> UpdateAsync(Guid id, UpdatePurchaseDto updateDto, Guid userId)
        //{
        //    var purchase = await _context.Purchases
        //        .Include(x => x.Transaction)
        //             .ThenInclude(t => t.TransactionItems)
        //        .FirstOrDefaultAsync(p => p.Id == id && p.Transaction.GeneralStatus != GeneralStatus.SoftDeleted);

        //    if (purchase == null)
        //        return null;

        //    using var dbTransaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var now = DateTime.UtcNow;

        //        // Update Transaction
        //        purchase.Transaction.Update(
        //            //updateDto.Date,
        //            updateDto.TotalAmount,
        //            updateDto.TaxAmount,
        //           // updateDto.DiscountAmount,
        //           // PaymentStatus.Pending,
        //            userId,
        //            now
        //        );

        //        // Update Transaction Items
        //        var existingItems = purchase.Transaction.TransactionItems.ToList();

        //        // Remove items not in the update
        //        foreach (var item in existingItems)
        //        {
        //            if (!updateDto.Items.Any(i => i.Id == item.Id))
        //            {
        //                item.SoftDelete(userId, now);
        //            }
        //        }

        //        // Update or add items
        //        foreach (var itemDto in updateDto.Items)
        //        {
        //            var existingItem = existingItems.FirstOrDefault(i => i.Id == itemDto.Id);
        //            var total = itemDto.Quantity * itemDto.UnitPrice;

        //            if (existingItem != null)
        //            {
        //                // Update existing item
        //                typeof(TransactionItem).GetProperty("Quantity").SetValue(existingItem, itemDto.Quantity);
        //                typeof(TransactionItem).GetProperty("UnitPrice").SetValue(existingItem, itemDto.UnitPrice);
        //                typeof(TransactionItem).GetProperty("Total").SetValue(existingItem, total);
        //                existingItem.UpdatedAt = now;
        //                existingItem.UpdatedBy = userId;
        //            }
        //            else
        //            {
        //                // Add new item
        //                var newItem = TransactionItem.Create(
        //                    Guid.NewGuid(),
        //                    purchase.TransactionId,
        //                    itemDto.ItemId,
        //                    itemDto.Quantity,
        //                    itemDto.UnitPrice,
        //                    total,
        //                    userId,
        //                    now
        //                );
        //                await _context.TransactionItems.AddAsync(newItem);
        //            }
        //        }

        //        // Update Purchase properties
        //        typeof(Purchase).GetProperty("LocationId").SetValue(purchase, updateDto.LocationId);
        //        typeof(Purchase).GetProperty("SupplierId").SetValue(purchase, updateDto.SupplierId);
        //        typeof(Purchase).GetProperty("PurcasedById").SetValue(purchase, updateDto.PurchasedBy.ToString());


        //        await _context.SaveChangesAsync();
        //        await dbTransaction.CommitAsync();

        //        return await GetByIdAsync(id, GeneralStatus.Active);
        //    }
        //    catch
        //    {
        //        await dbTransaction.RollbackAsync();
        //        throw;
        //    }
        //}

        //public async Task<bool> DeleteAsync(Guid id, Guid userId, string reason)
        //{
        //    var purchase = await _context.Purchases
        //        .Include(x=> x.Transaction)
        //            .ThenInclude(t => t.TransactionItems)
        //        .FirstOrDefaultAsync(p => p.Id == id && p.Transaction.GeneralStatus != GeneralStatus.SoftDeleted);

        //    if (purchase == null)
        //        return false;
        //    var now = DateTime.UtcNow;
        //    purchase.Transaction.SoftDelete(now, userId);

        //    foreach (var item in purchase.Transaction.TransactionItems)
        //    {
        //        item.SoftDelete(userId, now);
        //    }

        //    await _context.SaveChangesAsync();
        //    return true;
        //}


        //public async Task<PurchaseCancellationDto> CancelAsync(CreatePurchaseCancellationDto createDto, Guid userId)
        //{
        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var purchase = await _context.Purchases
        //             .Include(p => p.Transaction)
        //                .ThenInclude(t => t.TransactionItems)
        //                    .ThenInclude(x=> x.TransactionItemsDelivered)
        //            .FirstOrDefaultAsync(p => p.Id == createDto.PurchaseId
        //                && p.Transaction.GeneralStatus != GeneralStatus.SoftDeleted);

        //        if (purchase == null)
        //            throw new Exception("Transaction not found or already cancelled");

        //        if (purchase.Transaction.TransactionItems.Any(x=> x.TransactionItemsDelivered.Any()))
        //        {
        //            throw new Exception("Transaction has Items delivered, hence cannot be cancelled");
        //        }

        //        var now = DateTime.UtcNow;

        //        // Soft delete the associated transaction
        //        if (purchase.Transaction != null)
        //        {
        //            purchase.Transaction.SoftDelete(now, userId);

        //            // Soft delete all transaction items
        //            foreach (var item in purchase.Transaction.TransactionItems)
        //            {
        //                item.SoftDelete(userId, now);
        //            }
        //        }

        //        // Here you might want to create a cancellation record
        //        // For now, we'll just update the purchase status

        //        await _context.SaveChangesAsync();
        //        await transaction.CommitAsync();

        //        return new PurchaseCancellationDto
        //        {
        //            Id = purchase.Id,
        //            PurchaseId = purchase.Id,
        //            PurchaseNumber = purchase.Transaction.TransactionNumber, 
        //            CancellationDate = now,
        //            Reason = createDto.Reason,
        //            CancelledBy = userId
        //        };
        //    }
        //    catch
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}


    }
}