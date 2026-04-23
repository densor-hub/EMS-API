using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;

namespace WebApplication1.DAL.Repository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;
        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> AddPayment(TransactionPaymentsDto createDto, Guid userId)
        {
            var transRecord = _context.Transaction
                .Include(X => X.TransactionPayments)
                .Where(x => x.Id == createDto.TransationId).FirstOrDefault();

            if (transRecord == null) { throw new Exception("Submitted transaction not found"); }

            var toatalAmount = transRecord.TotalAmount;
            var totalPayments = transRecord.TransactionPayments.Sum(x => x.Amount);

            if (totalPayments + createDto.Amount > toatalAmount)
            {
                //throw new Exception($"Amount paid exceeds total purhcase amount, Expected {toatalAmount - totalPayments}");
            }

            var payment = TransactionPayment.Create(
                Guid.NewGuid(),
                transRecord.Id,
                 DateTime.SpecifyKind((DateTime)createDto.PaymentDate, DateTimeKind.Utc),
                createDto.Amount,
                toatalAmount - totalPayments,
                createDto.PaymentMethod,
                DateTime.UtcNow,
                userId);

            await _context.TransactionPayments.AddAsync(payment);
            await _context.SaveChangesAsync();

            return payment.Id;
        }


        public async Task CompleteTransaction(Transaction transaction, CreateTransactionDto createDto, Guid userId)
        {
            // Create Transaction Items
            var transactionItems = createDto.Items.Select(itemDto => TransactionItem.Create(
                    Guid.NewGuid(),
                    transaction.Id,
                    itemDto.ItemId,
                    itemDto.Quantity,
                    itemDto.UnitPrice,
                    itemDto.Quantity * itemDto.UnitPrice,
                    userId,
                    DateTime.UtcNow
                ));


            //create transaction items delivered
            //automatic stock quaity updates
            var stockLevelsToBeCreated = new List<StockLevel>();
            var stockLevelsToBeUpdated = new List<StockLevel>();
            var itemsDelivered = new List<TransactionItemDelivered>();

            var itemsPrice = await _context.Items.Where(x => x.ItemLocations.Any(x => x.LocationId == createDto.LocationId)).Select(x=>  new {x.SellingPrice, x.CostPrice, x.Id, x.Name} ).ToListAsync();
            foreach (var transactionItem in transactionItems)
            {
                var item = itemsPrice.Where(x => x.Id == transactionItem.ItemId).FirstOrDefault();
                if (transactionItem.UnitPrice < item?.SellingPrice) throw new Exception($"Selling Price of {item.Name} cannot be less than {item.CostPrice}");

                var itemPosted = createDto.Items.Where(x => x.ItemId == transactionItem.ItemId).FirstOrDefault();

                if (itemPosted != null && itemPosted.DeliveredQuantity > 0)
                {
                    var transactionItemDelivered = TransactionItemDelivered.Create(Guid.NewGuid(), transactionItem.Id, itemPosted.DeliveredQuantity ?? 0, DateTime.SpecifyKind((DateTime)createDto.Date, DateTimeKind.Utc));
                    itemsDelivered.Add(transactionItemDelivered);
                }

                var stockLevel = await _context.StockLevels.Where(x => x.LocationId == createDto.LocationId && x.ItemId == transactionItem.ItemId).FirstOrDefaultAsync();

                if (transaction.TransactionAction == TransactionActionType.PURCHASE.ToString())
                {
                    if (stockLevel == null)
                    {
                        var newStockLevel = StockLevel.Create(Guid.NewGuid(), itemPosted.Quantity, itemPosted.Quantity, transactionItem.ItemId, createDto.LocationId);
                        stockLevelsToBeCreated.Add(newStockLevel);
                    }
                    else
                    {
                        stockLevel.AddActual(itemPosted.Quantity);
                        stockLevel.AddAvailable(itemPosted.Quantity);
                        stockLevelsToBeUpdated.Add(stockLevel);
                    }
                }

                if (transaction.TransactionAction == TransactionActionType.SALE.ToString())
                {
                    if (stockLevel == null || stockLevel.AvailableQuanity < transactionItem.Quantity)
                    {
                        // if (createDto.ForceIt == false)
                        // {
                        //var itemName = await _context.Items.Where(x => x.Id == transactionItem.ItemId).Select(x=> x.Name).FirstOrDefaultAsync();
                        throw new Exception($"Insufficient stock for item {item?.Name}. Available: {stockLevel?.AvailableQuanity ?? 0}, Requested: {transactionItem.Quantity}");
                        // }
                    }

                    if (transaction.TransactionType == TransactinType.Debit)
                    {
                        stockLevel.SubstractActual(itemPosted.Quantity);
                        stockLevel.SubstractAvailable(itemPosted.Quantity);
                        stockLevelsToBeUpdated.Add(stockLevel);
                    }

                    if (transaction.TransactionType == TransactinType.Deposit)
                    {
                        stockLevel.SubstractActual(itemPosted.Quantity);
                        stockLevelsToBeUpdated.Add(stockLevel);
                    }

                }

            }


            //add all to db
            await _context.TransactionItems.AddRangeAsync(transactionItems);
            await _context.TransactionItemsDelivered.AddRangeAsync(itemsDelivered);

            if (stockLevelsToBeUpdated.Any()) _context.StockLevels.UpdateRange(stockLevelsToBeUpdated);
            if (stockLevelsToBeCreated.Any()) await _context.StockLevels.AddRangeAsync(stockLevelsToBeCreated);

            //create transaction payments
            if (createDto.AmountPaid > 0)
            {
                var transactionPayment = TransactionPayment.Create(Guid.NewGuid(), transaction.Id, DateTime.SpecifyKind((DateTime)createDto.Date, DateTimeKind.Utc),
                    createDto.AmountPaid, createDto.TotalAmount - createDto.AmountPaid, createDto.PaymentMethod, DateTime.UtcNow, userId);

                await _context.TransactionPayments.AddAsync(transactionPayment);
            }
        }

        public async Task<IEnumerable<TransactionItemsReceivedDto>> GetAllDeliveredItemsToDate(Guid transactionId)
        {
            var transactionItemsRecieved = _context.TransactionItemsDelivered
                 .Include(x => x.TransactionItem)
                     .ThenInclude(X => X.Item)
                 .Where(x => x.TransactionItem.TransactionId == transactionId).AsNoTracking();

            return await transactionItemsRecieved.OrderByDescending(x=> x.DeliveryDate).Select(x => new TransactionItemsReceivedDto
            {
                Id = x.Id,
                Date = x.DeliveryDate.Date,
                ItemName = x.TransactionItem.Item.Name,
                Quantity = x.Quanity
            }).ToListAsync();
        }


        public async Task DeliverItems(ConfirmTransactionDeliveryDTO createDto, Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var userCompanyLocations = _context.Users
               .Include(x => x.Company)
               .ThenInclude(comp => comp.Locations)
               .Where(x => x.Id == userId.ToString());

                if (userCompanyLocations.Any() == false) throw new Exception("Invalid request, user location detials not found");

                var validLocation = userCompanyLocations.Where(x => x.Company.Locations.Any(x => x.Id == createDto.LocationId));
                if (validLocation.Any() == false) { throw new Exception("Invalid location"); }

                var transRecord = await _context.Transaction
                        .Include(t => t.TransactionItems)
                         .ThenInclude(t => t.TransactionItemsDelivered)
                    .Where(p => p.Id == createDto.TransactionId).FirstOrDefaultAsync();

                if (transRecord == null) { throw new Exception("Invalid request, record not found"); }

                Guid purchaseOrSaleId = await _context.Sales.Where(x => x.TransactionId == transRecord.Id).Select(x => x.Id).FirstOrDefaultAsync(); 
                if (purchaseOrSaleId == Guid.Empty) await _context.Purchases.Where(x => x.TransactionId == transRecord.Id).Select(x => x.Id).FirstOrDefaultAsync();


                if (purchaseOrSaleId == Guid.Empty) { throw new Exception("Invalid request, record not found"); }

              

                var submitedItemIds = createDto.Items.Select(x => x.ItemId).ToList();

                var validSubmitted = transRecord.TransactionItems.Where(x => submitedItemIds.Contains(x.ItemId)).ToList();
                //create transaction items delivered

                if (validSubmitted.Any() == false) { throw new Exception("Invalid request, items submitted not found"); }
                var itemsDelivered = new List<TransactionItemDelivered>();

                var stockLevelsToBeCreated = new List<StockLevel>();
                var stockLevelsToBeUpdated = new List<StockLevel>();

                var itemsInDb = await _context.Items.Where(x => x.ItemLocations.Any(x => x.LocationId == createDto.LocationId)).Select(x => new {  x.Id, x.Name }).ToListAsync();
                foreach (var transactionItem in validSubmitted)
                {
                    var itemPosted = createDto.Items.Where(x => x.ItemId == transactionItem.ItemId).FirstOrDefault();
                    var item = itemsInDb.Where(x => x.Id == transactionItem.ItemId).FirstOrDefault();

                    if (itemPosted != null && itemPosted.Quantity > 0)
                    {
                        var transactionItemDelivered = TransactionItemDelivered.Create(Guid.NewGuid(), transactionItem.Id, itemPosted.Quantity, DateTime.SpecifyKind((DateTime)createDto.Date, DateTimeKind.Utc));
                        itemsDelivered.Add(transactionItemDelivered);
                    }

                    //automatic stock quaity updates
                    var stockLevel = await _context.StockLevels.Where(x => x.LocationId == createDto.LocationId && x.ItemId == transactionItem.ItemId).FirstOrDefaultAsync();

                    if (transRecord.TransactionAction == TransactionActionType.PURCHASE.ToString())
                    {
                        if (stockLevel == null)
                        {
                            var newStockLevel = StockLevel.Create(Guid.NewGuid(), itemPosted.Quantity, itemPosted.Quantity, transactionItem.ItemId, createDto.LocationId);
                            stockLevelsToBeCreated.Add(newStockLevel);
                        }
                        else
                        {
                            stockLevel.AddActual(itemPosted.Quantity);
                            stockLevel.AddAvailable(itemPosted.Quantity);
                            stockLevelsToBeUpdated.Add(stockLevel);
                        }
                    }

                    if (transRecord.TransactionAction == TransactionActionType.SALE.ToString())
                    {
                        if (stockLevel == null)
                        {
                            throw new Exception($"Insufficient stock for item {item?.Name}. Available: {stockLevel?.AvailableQuanity ?? 0}, Requested: {transactionItem.Quantity}");
                        }
                        else
                        {
                            stockLevel.SubstractAvailable(itemPosted.Quantity);
                            stockLevelsToBeUpdated.Add(stockLevel);
                        }

                    }

                }

                await _context.TransactionItemsDelivered.AddRangeAsync(itemsDelivered);

                if (createDto.Transportation !=null)
                {
                    var newTrasansactionTransportation = TransactionTransportation.Create(
                    Guid.NewGuid(),
                    transRecord.Id,
                    (decimal)createDto.Transportation,
                    DateTime.SpecifyKind((DateTime)createDto.Date, DateTimeKind.Utc));

                    await _context.AddAsync(newTrasansactionTransportation);
                }
                //add all to db


                if (stockLevelsToBeUpdated.Any()) _context.StockLevels.UpdateRange(stockLevelsToBeUpdated);
                if (stockLevelsToBeCreated.Any()) await _context.StockLevels.AddRangeAsync(stockLevelsToBeCreated);


                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                throw;
            }

        }
    }
}
