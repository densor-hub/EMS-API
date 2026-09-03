using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Linq;
using WebApplication1.DAL;
using WebApplication1.DAL.Migrations;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _context;
        private readonly ITransactionService _transactionService;
        private readonly IUserRepository _user;

        public SaleService(
            AppDbContext context,
               ITransactionService transactionService,
               IUserRepository user
            )
        {
            _context = context;
            _transactionService = transactionService;
            _user = user;
        }


        public async Task<IEnumerable<GetSalesTrans>> GetAllAsync(Guid locationId, GeneralStatus generalStatus , string type, Guid? customerId = null, Guid? salesPersonId = null)
        {
            var query = from sales in _context.Sales
                .Include(p => p.Customer)
                .Include(p => p.SalesPerson)
                .Include(p => p.Transaction)
                    .ThenInclude(c => c.Location)
                .Include(p => p.Transaction)
                    .ThenInclude(t => t.TransactionItems)
                        .ThenInclude(ti => ti.TransactionItemsDelivered)
                .Include(p => p.Transaction)
                    .ThenInclude(t => t.TransactionPayments) // Added this
                .Where(x => x.Transaction.LocationId == locationId
                         && x.GeneralStatus == generalStatus
                         && (type.ToUpper().Trim()  == "GENERAL" ? x.Customer == null:
                            type.ToUpper().Trim() == "CUSTOMER" ? x.Customer != null && (customerId !=null ? x.CustomerId == customerId : true) :false)
                         && (salesPersonId != null && salesPersonId == Guid.Empty ? x.SalesPersonId == salesPersonId.ToString() : true))
                .AsNoTracking() // Moved before select

                        select new GetSalesTrans // Changed from GetSalesTrans to GetSaleDto
                        {
                            Id = sales.Id,
                            TransactionId = sales.TransactionId,
                            TransactionCode = sales.Transaction.TransactionNumber,
                            LocationName = sales.Transaction.Location != null ? sales.Transaction.Location.Name : "",
                            CustomerId = sales.Customer.Id,
                            CustomerName = sales.Customer != null
                                ? sales.Customer.FirstName + " " + sales.Customer.LastName
                                : "",
                            //TransactionBy = sales.SalesPerson != null ? sales.SalesPerson.FullName : "",
                            CreatedAt = sales.CreatedAt,
                            TransactionDate = sales.Transaction != null ? sales.Transaction.TransactionDate : DateTime.MinValue,
                            TotalAmount = sales.Transaction != null ? sales.Transaction.TotalAmount : 0,
                            PaidAmount  = sales.Transaction != null ? sales.Transaction.TransactionPayments != null ?
                               sales.Transaction.TransactionPayments.Where(tp => tp.TransactionId == sales.TransactionId)
                                .Sum(tp => tp.Amount)  : 0 : 0,
                        };

                return await query.OrderByDescending(x=> x.CreatedAt).ToListAsync();
        }

        public async Task<Sale> GetByIdAsync(Guid id)
        {
            return await _context.Sales
                .Include(s => s.Transaction)
                    .ThenInclude(x=> x.Location)
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


        public async Task<GetSalesReceiptDto> GenerateReceipt(Guid? saleTransDeliveryRequestId, string transNumber)
        {
            var user = await _user.GetUserByRefreshTokenAsync();
            if (user == null) throw new Exception("Unauthorized");

            var deliveryRequest = await _context.SaleTransDeliveryRequests
                                    .Include(x => x.Sale)
                                        .ThenInclude(x => x.Customer)
                                    .Include(x => x.Sale)
                                        .ThenInclude(x => x.Transaction)
                                            .ThenInclude(x => x.Location)
                                    .Include(x => x.Sale)
                                        .ThenInclude(x => x.Transaction)
                                            .ThenInclude(x => x.TransactionPayments)
                                    .Include(x => x.Sale)
                                        .ThenInclude(x => x.Transaction)
                                            .ThenInclude(x => x.TransactionItems)
                                                .ThenInclude(x => x.Item)
                                    .Include(x => x.Sale)
                                        .ThenInclude(x => x.Transaction)
                                            .ThenInclude(x => x.TransactionItems)
                                                .ThenInclude(x => x.TransactionItemsDelivered)
                                    .Where(x => saleTransDeliveryRequestId.HasValue && saleTransDeliveryRequestId.Value != Guid.Empty ?
                                                    x.Id == saleTransDeliveryRequestId :
                                                x.Sale.Transaction.TransactionNumber.ToUpper().Trim() == transNumber.ToUpper().Trim()
                                         )
                                    .FirstOrDefaultAsync();

            if (deliveryRequest is null) throw new Exception("Transaction not found");

            //delivery Items params ConfirmTransactionDeliveryDTO createDto, ApplicationUser user, SaleTransDeliveryRequest saleTransDeliveryRequest
            //var transactionItems
            var deliveryDto = new ConfirmTransactionDeliveryDTO
            {
                Date = DateTime.UtcNow,
                LocationId = deliveryRequest.Sale.Transaction.LocationId,
                TransactionId = deliveryRequest.Sale.TransactionId,
                Transportation = null,
                Items = deliveryRequest.Sale.Customer == null ? 
                                        deliveryRequest.Sale.Transaction.TransactionItems.Select(x => new ConfirmTransactionDeliveryDTOItems
                                            {
                                                ItemId = x.ItemId,
                                                Quantity = x.Quantity,
                                                Name = x.Item.Name,
                                                Code = x.Item.Code,
                                                UnitPrice = x.UnitPrice
                                        }).ToList() :
                                        deliveryRequest.TransactionItemDelivered.Select(x => new ConfirmTransactionDeliveryDTOItems
                                            {
                                                ItemId = x.TransactionItem.ItemId,
                                                Quantity = x.Quantity,
                                                Name = x.TransactionItem.Item.Name,
                                                Code = x.TransactionItem.Item.Code,
                                                UnitPrice = x.TransactionItem.UnitPrice
                                        }
                                  ).ToList()

            };


            //delivery items
            if (deliveryRequest.IsDelivered == false || !deliveryRequest.TransactionItemDelivered.All(x=> x.IsDelivered))
            {
                await _transactionService.DeliverItems(deliveryDto, user, deliveryRequest);
            }

            var totalPayment = deliveryRequest.Sale.Transaction?.TransactionPayments != null ?
                                deliveryRequest.Sale.Transaction?.TransactionPayments.Where(tp => tp.TransactionId == deliveryRequest.Sale.TransactionId)
                                .Sum(tp => tp.Amount) ?? 0 : 0;
            var transactionTotal = deliveryRequest.Sale.Transaction != null ? deliveryRequest.Sale.Transaction.TotalAmount : 0;

            var Balance = totalPayment - transactionTotal;
            //return sale transs
            return new GetSalesReceiptDto // Changed from GetSalesTrans to GetSaleDto
            {
                TransactionCode = deliveryRequest.Sale.Transaction.TransactionNumber,
                LocationName = deliveryRequest.Sale.Transaction.Location != null ? deliveryRequest.Sale.Transaction.Location.Name : "",
                CustomerName = deliveryRequest.Sale.Customer != null
                                ? deliveryRequest.Sale.Customer.FirstName + " " + deliveryRequest.Sale.Customer.LastName
                                : "",
                //TransactionBy = sales.SalesPerson != null ? sales.SalesPerson.FullName : "",
                CreatedAt = deliveryRequest.Sale.CreatedAt,
                TransactionDate = deliveryRequest.Sale.Transaction != null ? deliveryRequest.Sale.Transaction.TransactionDate : DateTime.MinValue,
                TotalAmount = transactionTotal,
                PaidAmount = totalPayment,
                Balance = Balance,
                // Check for nullables with safe navigation and null coalescing
                Items = deliveryDto.Items != null ? deliveryDto.Items.Select(x => new GetSaleReceiptItemDto
                {
                    Name = x.Name ,
                    Code = x.Code ?? "",
                    Quantity = x.Quantity,
                    UnitPrice = x.UnitPrice ?? 0,
                }).ToList() : new List<GetSaleReceiptItemDto>()

            };
        }

    }
}
