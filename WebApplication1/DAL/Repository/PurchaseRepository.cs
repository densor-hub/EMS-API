// Services/Implementations/PurchaseService.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;
using WebApplication1.Domain.Repository;
using WebApplication1.DAL;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApplication1.Domain.DTO;

namespace WebApplication1.Services.Implementations
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly AppDbContext _context;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly ITransactionRepository _transactionRepository;
        

        public PurchaseRepository(
            AppDbContext context,
            ITransactionCodeRepository transactionCodeRepository,
             ITransactionRepository transactionRepository
            )
        {
            _context = context;
            _transactionCodeRepository = transactionCodeRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<PurchaseDto> GetByIdAsync(Guid id, GeneralStatus generalStatus)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Location)
                .Include(p => p.Supplier)
                .Include(p => p.PurcasedBy)  // Fixed typo
                .Include(p => p.Transaction)
                    .ThenInclude(t => t.TransactionItems)
                        .ThenInclude(ti => ti.Item)
                .Include(p => p.Transaction)
                    .ThenInclude(t => t.TransactionPayments)
                .FirstOrDefaultAsync(p => p.Id == id && p.GeneralStatus == generalStatus);

            if (purchase == null)
                return null;

            return new PurchaseDto
            {
                Id = purchase.Id,  // Use the actual Purchase Id
                TransactionId = purchase.TransactionId,  // Add separate field if needed
                TransactionCode = purchase.TransactionCode,
                LocationName = purchase.Location?.Name,
                SupplierId = purchase.SupplierId,
                SupplierName = $"{purchase.Supplier?.FirstName ?? ""} {purchase.Supplier?.LastName ?? ""}".Trim(),
                TransactionBy = purchase.PurcasedBy?.FullName,
                CreatedAt = purchase.CreatedAt,
                TransactionDate = purchase.Transaction?.Date ?? DateTime.MinValue,
                TotalAmount = purchase.Transaction?.TotalAmount ?? 0,
                PaidAmount = purchase.Transaction?.TransactionPayments?.Sum(x => x.Amount) ?? 0,
                Balance  = (purchase.Transaction?.TotalAmount ?? 0) -
                                  (purchase.Transaction?.TransactionPayments?.Sum(x => x.Amount) ?? 0)
            };
        }

        public async Task<IEnumerable<PurchaseDto>> GetAllAsync(Guid locationId, GeneralStatus generalStatus, Guid? supplierId = null, Guid? salesPersonId = null)
        {
            var query = from purchase in _context.Purchases
                        .Include(p => p.Location)
                        .Include(p => p.Supplier)
                        .Include(p => p.PurcasedBy)
                        .Include(p => p.Transaction)
                            .ThenInclude(t => t.TransactionItems)
                                .ThenInclude(ti => ti.TransactionItemsDelivered)
                        .Where(x => x.LocationId == locationId && x.GeneralStatus == generalStatus && (supplierId != null ? x.SupplierId == supplierId : true)
                        && (salesPersonId != null && salesPersonId != Guid.Empty ? x.PurcasedById == salesPersonId.ToString() : true)
                        )
                        .AsNoTracking()

                        select new PurchaseDto
                        {
                            Id = purchase.Id, 
                            TransactionId = purchase.TransactionId,
                            TransactionCode = purchase.TransactionCode,
                            LocationName = purchase.Location != null ? purchase.Location.Name : "",
                            SupplierId = purchase.SupplierId,
                            SupplierName = purchase.Supplier != null
                                ? purchase.Supplier.FirstName + " " + purchase.Supplier.LastName
                                : "",
                            TransactionBy = purchase.PurcasedBy != null ? purchase.PurcasedBy.FullName : "",
                            CreatedAt = purchase.CreatedAt,
                            TransactionDate = purchase.Transaction != null ? purchase.Transaction.Date : DateTime.MinValue,
                            TotalAmount = purchase.Transaction != null ? purchase.Transaction.TotalAmount : 0,
                            PaidAmount = purchase.Transaction.TransactionPayments
                                .Where(tp => tp.TransactionId == purchase.TransactionId)
                                .Sum(tp => tp.Amount),
                            Items = purchase.Transaction.TransactionItems.Select(x => new TransactionItemDto
                            {
                                ItemId = x.Item.Id,
                                Quantity = x.Quantity,
                                UnitPrice = x.UnitPrice,  // Purchase Price
                                DeliveredQuantity = x.TransactionItemsDelivered.Sum(x=> x.Quanity),
                                ItemName = x.Item.Name,
                                Code = x.Item.Code,
                                CostPrice = x.Item.CostPrice
                            }).ToList(),
                            Payments = purchase.Transaction.TransactionPayments.OrderByDescending(x=> x.PaymentDate).Select(x=> new TransactionPaymentsDto
                            {
                                Amount = x.Amount,
                                PaymentDate = x.PaymentDate,
                                PaymentMethod = x.PaymentMethod
                            }).ToList()
                           
                        };

            return await query.ToListAsync();
        }
        

        
        public async Task<Guid> CreateAsync(CreateTransactionDto createDto, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
               
                var transactionId = Guid.NewGuid();
                var purchaseId = Guid.NewGuid();
                var now = DateTime.UtcNow;
                var transactionCode = await _transactionCodeRepository.GenerateUniquePinAsync();

                // Create Transaction
                var transactionEntity = Transaction.Create(
                    transactionId,
                    transactionCode,
                     DateTime.SpecifyKind((DateTime)createDto.Date, DateTimeKind.Utc),
                    createDto.TotalAmount,
                    createDto.TaxAmount,
                    createDto.DiscountAmount,
                        createDto.AmountPaid == 0 ? PaymentStatus.Pending :
                        createDto.TotalAmount > createDto.AmountPaid ? PaymentStatus.Partial :
                        createDto.TotalAmount >= createDto.AmountPaid ? PaymentStatus.Completed : PaymentStatus.Completed,
                    userId,
                    now,
                    createDto.TransactinType,
                    TransactionActionType.PURCHASE.ToString()
                );

                await _context.Transaction.AddAsync(transactionEntity);


                await _transactionRepository.CompleteTransaction(transactionEntity, createDto, userId);
                
                // Create Purchase
                var purchase = Purchase.Create(
                    purchaseId,
                    createDto.LocationId,
                    createDto.BusinessPartnerId,
                    userId,
                    transactionCode,
                    transactionId,
                    now,
                    userId
                );

                await _context.Purchases.AddAsync(purchase);

                //save and commit
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return  purchaseId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PurchaseDto> UpdateAsync(Guid id,UpdatePurchaseDto updateDto, Guid userId)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Transaction)
                    .ThenInclude(t => t.TransactionItems)
                .FirstOrDefaultAsync(p => p.Id == id && p.GeneralStatus != GeneralStatus.SoftDeleted);

            if (purchase == null)
                return null;

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var now = DateTime.UtcNow;

                // Update Transaction
                purchase.Transaction.Update(
                    updateDto.Date,
                    updateDto.TotalAmount,
                    updateDto.TaxAmount,
                    updateDto.DiscountAmount,
                    PaymentStatus.Pending,
                    userId,
                    now
                );

                // Update Transaction Items
                var existingItems = purchase.Transaction.TransactionItems.ToList();

                // Remove items not in the update
                foreach (var item in existingItems)
                {
                    if (!updateDto.Items.Any(i => i.Id == item.Id))
                    {
                        item.SoftDelete(userId, now);
                    }
                }

                // Update or add items
                foreach (var itemDto in updateDto.Items)
                {
                    var existingItem = existingItems.FirstOrDefault(i => i.Id == itemDto.Id);
                    var total = itemDto.Quantity * itemDto.UnitPrice;

                    if (existingItem != null)
                    {
                        // Update existing item
                        typeof(TransactionItem).GetProperty("Quantity").SetValue(existingItem, itemDto.Quantity);
                        typeof(TransactionItem).GetProperty("UnitPrice").SetValue(existingItem, itemDto.UnitPrice);
                        typeof(TransactionItem).GetProperty("Total").SetValue(existingItem, total);
                        existingItem.UpdatedAt = now;
                        existingItem.UpdatedBy = userId;
                    }
                    else
                    {
                        // Add new item
                        var newItem = TransactionItem.Create(
                            Guid.NewGuid(),
                            purchase.TransactionId,
                            itemDto.ItemId,
                            itemDto.Quantity,
                            itemDto.UnitPrice,
                            total,
                            userId,
                            now
                        );
                        await _context.TransactionItems.AddAsync(newItem);
                    }
                }

                // Update Purchase properties
                typeof(Purchase).GetProperty("LocationId").SetValue(purchase, updateDto.LocationId);
                typeof(Purchase).GetProperty("SupplierId").SetValue(purchase, updateDto.SupplierId);
                typeof(Purchase).GetProperty("PurcasedById").SetValue(purchase, updateDto.PurchasedBy.ToString());

                purchase.UpdatedAt = now;
                purchase.UpdatedBy = userId;

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return await GetByIdAsync(id, GeneralStatus.Active);
            }
            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, Guid userId, string reason)
        {
            var purchase = await _context.Purchases
                .Include(p => p.Transaction)
                    .ThenInclude(t => t.TransactionItems)
                .FirstOrDefaultAsync(p => p.Id == id && p.GeneralStatus != GeneralStatus.SoftDeleted);

            if (purchase == null)
                return false;
            var now = DateTime.UtcNow;
            purchase.SoftDelete(reason, userId,now);
            purchase.Transaction.SoftDelete(now, userId);

            foreach (var item in purchase.Transaction.TransactionItems)
            {
                item.SoftDelete(userId, now);
            }

            purchase.UpdatedAt = DateTime.UtcNow;
            purchase.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return true;
        }

        private PurchaseDto MapToDto(Purchase purchase)
        {
            return new PurchaseDto
            {
                //Id = purchase.Id,
                Id = purchase.TransactionId,
                TransactionCode = purchase.TransactionCode,
                //LocationId = purchase.LocationId,
                LocationName = purchase.Location?.Name,
                SupplierId = purchase.SupplierId,
                SupplierName = $"{purchase?.Supplier?.FirstName??""} {purchase?.Supplier?.LastName??""}",
                //PurchasedById = purchase.PurcasedById,
                TransactionBy = purchase.PurcasedBy?.FullName,
                CreatedAt = purchase.CreatedAt,
                TransactionDate = purchase.Transaction.Date,
                TotalAmount = purchase.Transaction.TotalAmount,
                PaidAmount = purchase.Transaction.TransactionPayments.Any() ?  purchase.Transaction.TransactionPayments.Sum(x=> x.Amount) : 0
            };
        }

        public async Task<PurchaseCancellationDto> CancelAsync(CreatePurchaseCancellationDto createDto, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var purchase = await _context.Purchases
                    .Include(p => p.Transaction)
                        .ThenInclude(t => t.TransactionItems)
                    .FirstOrDefaultAsync(p => p.Id == createDto.PurchaseId 
                        && p.GeneralStatus != GeneralStatus.SoftDeleted);

                if (purchase == null)
                    throw new Exception("Purchase not found or already cancelled");

                var now = DateTime.UtcNow;

                // Soft delete the purchase
                purchase.SoftDelete(createDto.Reason, userId, now);
               

                // Soft delete the associated transaction
                if (purchase.Transaction != null)
                {
                    purchase.Transaction.SoftDelete(now, userId);

                    // Soft delete all transaction items
                    foreach (var item in purchase.Transaction.TransactionItems)
                    {
                        item.SoftDelete(userId, now);
                    }
                }

                // Here you might want to create a cancellation record
                // For now, we'll just update the purchase status

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new PurchaseCancellationDto
                {
                    Id = purchase.Id,
                    PurchaseId = purchase.Id,
                    PurchaseNumber = purchase.TransactionCode,
                    CancellationDate = now,
                    Reason = createDto.Reason,
                    CancelledBy = userId
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        
    }
}