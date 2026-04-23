using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;

namespace WebApplication1.DAL.Repository
{
    public class SaleRepository : ISaleRepository
    {
        private readonly AppDbContext _context;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly ITransactionPaymentRepository _transactionPaymentRepository;
        private readonly ITransactionItemsDeliveredRepository _transactionItemsDeliveredRepository;
        private readonly ITransactionRepository _transactionRepository;

        public SaleRepository(
            AppDbContext context,
            ITransactionCodeRepository transactionCodeRepository,
             ITransactionPaymentRepository transactionPaymentRepository,
              ITransactionItemsDeliveredRepository transactionItemsDeliveredRepository,
              ITransactionRepository transactionRepository
            )
        {
            _context = context;
            _transactionCodeRepository = transactionCodeRepository;
            _transactionPaymentRepository = transactionPaymentRepository;
            _transactionItemsDeliveredRepository = transactionItemsDeliveredRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<string> CreateAsync(CreateTransactionDto createDto, Guid userId)
        {
            using var trans = await _context.Database.BeginTransactionAsync();

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
                    TransactionActionType.SALE.ToString()
                );

                await _context.Transaction.AddAsync(transactionEntity);

                await _transactionRepository.CompleteTransaction(transactionEntity, createDto, userId);


                var sale = Sale.Create(Guid.NewGuid(), transactionId, transactionCode, createDto.LocationId, createDto.BusinessPartnerId != Guid.Empty ? createDto.BusinessPartnerId : null, userId.ToString(), DateTime.UtcNow, userId);
                await _context.Sales.AddAsync(sale);

                await _context.SaveChangesAsync();

                await trans.CommitAsync();

                return transactionEntity.TransactionCode;
            }

            catch (Exception ex)
            {
                await trans.RollbackAsync();
                throw;
            }

        }


        public async Task<IEnumerable<PurchaseDto>> GetAllAsync(Guid locationId, GeneralStatus generalStatus, Guid? customerId = null, Guid? salesPersonId = null)
        {
            var query = from sales in _context.Sales
                         .Include(p => p.Location)
                         .Include(p => p.Customer)
                         .Include(p => p.SalesPerson)
                         .Include(p => p.Transaction)
                             .ThenInclude(t => t.TransactionItems)
                                 .ThenInclude(ti => ti.TransactionItemsDelivered)
                         .Where(x => x.LocationId == locationId && x.GeneralStatus == generalStatus && (customerId != null ? x.CustomerId == customerId : true)
                                 && (salesPersonId != null && salesPersonId != Guid.Empty ? x.SalesPersonId == salesPersonId.ToString() : true)
                            )
                         .AsNoTracking()

                        select new PurchaseDto
                        {
                            Id = sales.Id,
                            TransactionId = sales.TransactionId,
                            TransactionCode = sales.TransactionNumber,
                            LocationName = sales.Location != null ? sales.Location.Name : "",
                            CustomerId = sales.Customer != null ? (Guid)sales.CustomerId : Guid.Empty,
                            CustomerName = sales.Customer != null
                                ? sales.Customer.FirstName + " " + sales.Customer.LastName
                                : "",
                            TransactionBy = sales.SalesPerson != null ? sales.SalesPerson.FullName : "",
                            CreatedAt = sales.CreatedAt,
                            TransactionDate = sales.Transaction != null ? sales.Transaction.Date : DateTime.MinValue,
                            TotalAmount = sales.Transaction != null ? sales.Transaction.TotalAmount : 0,
                            PaidAmount = sales.Transaction.TransactionPayments
                                .Where(tp => tp.TransactionId == sales.TransactionId)
                                .Sum(tp => tp.Amount),
                            Items = sales.Transaction.TransactionItems.Select(x => new TransactionItemDto
                            {
                                ItemId = x.Item.Id,
                                Quantity = x.Quantity,
                                UnitPrice = x.UnitPrice,  // Purchase Price
                                DeliveredQuantity = x.TransactionItemsDelivered.Sum(x => x.Quanity),
                                ItemName = x.Item.Name,
                                Code = x.Item.Code,
                                CostPrice = x.Item.CostPrice
                            }).ToList(),
                            Payments = sales.Transaction.TransactionPayments.OrderByDescending(x => x.PaymentDate).Select(x => new TransactionPaymentsDto
                            {
                                Amount = x.Amount,
                                PaymentDate = x.PaymentDate,
                                PaymentMethod = x.PaymentMethod
                            }).ToList()

                        };

            return await query.ToListAsync();
        }

        public async Task<Sale> GetByIdAsync(Guid id)
        {
            return await _context.Sales
                .Include(s => s.Location)
                .Include(s => s.Customer)
                .Include(s => s.SalesPerson)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate)
        {
            var sales = await _context.Sales
                .Include(s => s.Transaction)
                .Where(s => s.CreatedAt.Date >= startDate.Date &&
                           s.CreatedAt.Date <= endDate.Date &&
                           s.GeneralStatus != GeneralStatus.SoftDeleted)
                .ToListAsync();

            return sales.Sum(s => s.Transaction?.TotalAmount ?? 0);
        }


        public async Task<bool> DeleteAsync(Guid id, Guid userId, string reason, Sale? preloadedSale = null)
        {
            var sale = preloadedSale ?? await _context.Sales.FirstOrDefaultAsync(p => p.Id == id && p.GeneralStatus != GeneralStatus.SoftDeleted);

            if (sale == null)    return false;
            var now = DateTime.UtcNow;
            sale.SoftDelete(reason, userId, now);
            sale.Transaction.SoftDelete(now, userId);

            foreach (var item in sale.Transaction.TransactionItems)
            {
                item.SoftDelete(userId, now);

                var deliveredItems = item.TransactionItemsDelivered.Sum(x => x.Quanity);
                
                if (sale.Transaction.TransactionType == TransactinType.Debit)
                {
                    //get delivered items 
                    item.Item.StockLevel.AddActual(item.Quantity - deliveredItems);
                }

                if (sale.Transaction.TransactionType == TransactinType.Deposit)
                {
                    item.Item.StockLevel.AddAvailable(item.Quantity - deliveredItems);
                }

            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SaleCancellationDto> CancellationAsync(CreateSaleCancellationDto createDto, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var sale = await _context.Sales
                    .Include(p => p.Transaction)
                        .ThenInclude(t => t.TransactionItems)
                    .FirstOrDefaultAsync(p => p.Id == createDto.SaleId
                        && p.GeneralStatus != GeneralStatus.SoftDeleted);

                if (sale == null)
                    throw new Exception("Purchase not found or already cancelled");

                var now = DateTime.UtcNow;

                 await DeleteAsync(sale.Id, userId, createDto.Reason, sale);

                // Here you might want to create a cancellation record
                // For now, we'll just update the purchase status

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new SaleCancellationDto
                {
                    Id = sale.Id,
                    SaleId = sale.Id,
                    TransactionNumber = sale.TransactionNumber,
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
