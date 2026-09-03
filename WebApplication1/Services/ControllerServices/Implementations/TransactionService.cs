using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using VMS.Modules.Licenses.Core.Emails.EmailSenderService.Entities;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;
using WebApplication1.Services.Emails.EmailService.Entities;
using WebApplication1.Services.Emails.EmailService.Queuer;
using WebApplication1.Services.Emails.TemplateService.Enitities;
using WebApplication1.Services.TokenService;
using HandlebarsDotNet;
using static System.Runtime.InteropServices.JavaScript.JSType;
using WebApplication1.Services.QrCodeService;
using System.Threading;
using System.Globalization;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;
using ZXing;
using Npgsql;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;
        private readonly ICompanyRepository _companyRepository;
        private readonly IEmailQueueRepository _emailQueueRepository;
        private readonly EmailSettings _emailSettings;
        private readonly IPaymentTokenService _paymentTokenService;
        private readonly ITransactionCodeRepository _transactionCodeRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IItemRepository _itemRepository;
        private readonly IQrCodeService _qrCodeService;
        private readonly IEmployeeLocationRepository _employeeLocationRepository;

        public TransactionService(AppDbContext context,
               ICompanyRepository companyRepository,
             IEmailQueueRepository emailQueueRepository,
            IOptions<EmailSettings> emailSettings,
            IPaymentTokenService paymentTokenService,
            ITransactionCodeRepository transactionCodeRepository,
            IUserRepository userRepository,
            ILocationRepository locationRepository,
             ITransactionRepository transactionRepository,
             IItemRepository itemRepository,
             IQrCodeService qrCodeService,
              IEmployeeLocationRepository employeeLocationRepository
            )
        {
            _context = context;
            _companyRepository = companyRepository;
            _emailQueueRepository = emailQueueRepository;
            _paymentTokenService = paymentTokenService;
            _emailSettings = emailSettings.Value;
            _transactionCodeRepository = transactionCodeRepository;
            _userRepository = userRepository;
            _locationRepository = locationRepository;
            _transactionRepository = transactionRepository;
            _itemRepository = itemRepository;
            _qrCodeService = qrCodeService;
            _employeeLocationRepository = employeeLocationRepository;
        }

        private async Task<Transaction> CreateGeneralTransactionAsync(CreateTransactionDto createDto, Location location, ApplicationUser user, string transactionNumber)
        {
            try
            {

                var transaction = Transaction.Create(Guid.NewGuid(), transactionNumber, DateTime.SpecifyKind(createDto.Date ?? DateTime.UtcNow, DateTimeKind.Utc),
                    0, createDto.TaxAmount, createDto.DiscountAmount, Guid.Parse(user.Id), DateTime.UtcNow, createDto.TransactionResultsType, createDto.TransactionType.ToString(),
                    location.Id, createDto.BusinessPartnerId.HasValue && createDto.BusinessPartnerId != Guid.Empty && createDto.TransactionType != TransactionType.TRAN);

                await _context.Transactions.AddAsync(transaction);


                //createDto.Remark save in remarks
                if (!string.IsNullOrEmpty(createDto.Remarks))
                {
                    var remark = TransactionComment.Create(Guid.NewGuid(), transaction.Id, transaction.TransactionType, "Initiated", createDto.Remarks, DateTime.UtcNow,
                      user.Id, user.FullName);
                    await _context.Comments.AddAsync(remark);
                }


                // await dbTrans.CommitAsync();
                return transaction;
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "Error creating disbursement");
                throw;
            }
        }


       

        private async Task<SpecificTransactionCreationReturnDto> CreateSpecificTypeOfTransaction(Transaction transaction, ApplicationUser user, Guid? BusinessPartnerId)
        {

            var qrCode = "";
            var EmailReceiver = new EmailReceiver();

            if (transaction.TransactionType == TransactionType.PURC.ToString())
            {
                var supplier = await _context.Suppliers.Where(x => x.Id == BusinessPartnerId).FirstOrDefaultAsync();
                if (supplier == null) { throw new Exception("Supplier not found"); }

                var purchase = Purchase.Create(Guid.NewGuid(), supplier.Id, Guid.Parse(user.Id), transaction.Id);
                await _context.Purchases.AddAsync(purchase);

                EmailReceiver.Email = supplier?.Email??"";
                EmailReceiver.Name =$"{supplier?.FirstName??""} {supplier?.LastName ?? ""}";
                EmailReceiver.Code = supplier?.Code ?? "";
                EmailReceiver.Id = supplier?.Id ?? Guid.Empty;
            }
            else if (transaction.TransactionType == TransactionType.SALE.ToString())
            {
                var sale = Sale.Create(Guid.NewGuid(), transaction.Id, BusinessPartnerId.HasValue && BusinessPartnerId.Value != Guid.Empty ? BusinessPartnerId.Value : null, user.Id, DateTime.UtcNow, Guid.Parse(user.Id));
                await _context.Sales.AddAsync(sale);

                if (BusinessPartnerId.HasValue && BusinessPartnerId.Value != Guid.Empty)
                {
                    var customer = await _context.Customers.Where(x => x.Id == BusinessPartnerId).FirstOrDefaultAsync();
                    if (customer == null) { throw new Exception("Contact person not found"); }


                    EmailReceiver.Email = customer?.Email ?? "";
                    EmailReceiver.Name = $"{customer?.FirstName ?? ""} {customer?.LastName ?? ""}";
                    EmailReceiver.Code = customer?.Code ?? "";
                    EmailReceiver.Id = customer?.Id ?? Guid.Empty;
                }
                else
                {
                    //HERE IS AN INSTANT SALE
                    var newDeliveryRequest = SaleTransDeliveryRequest.Create(Guid.NewGuid(), sale.Id, false, DateTime.UtcNow, user.Id, DateTime.UtcNow);
                    await _context.SaleTransDeliveryRequests.AddAsync(newDeliveryRequest);
                    //generate QR Code using the deliveryRequestId
                    qrCode = _qrCodeService.GenerateQrCodeBase64(newDeliveryRequest.Id.ToString(), 300, 300);
                }


            }

            else if (transaction.TransactionType == TransactionType.DEPO.ToString())
            {
                var contactPerson = await _context.FinancialServiceProviderContactPersons.Where(x => x.Id == BusinessPartnerId).FirstOrDefaultAsync();
                if (contactPerson == null) { throw new Exception("Contact person not found"); }

                EmailReceiver.Email = contactPerson?.Email ?? "";
                EmailReceiver.Name = $"{contactPerson?.FirstName ?? ""} {contactPerson?.LastName ?? ""}";
                EmailReceiver.Code = contactPerson?.Code ?? "";
                EmailReceiver.Id = contactPerson?.Id ?? Guid.Empty;

                var fs_disbursement = FinancialServiceDisbursement.Create(Guid.NewGuid(), contactPerson.Id, Guid.Parse(user.Id), transaction.Id);
                await _context.FinancialServiceDisbursement.AddAsync(fs_disbursement);
            }
            else if (transaction.TransactionType == TransactionType.TRAN.ToString())
            {
                if (BusinessPartnerId.HasValue == false) throw new Exception("A destination or target shop is required for this transaction to compltete");
                var stockTransferRequest = StockTransfer.Create(Guid.NewGuid(), transaction.Id, transaction.TransactionDate, StockTransferEnum.Pending, transaction.LocationId, (Guid)BusinessPartnerId, Guid.Parse(user.Id), DateTime.UtcNow);
                await _context.StockTransfers.AddAsync(stockTransferRequest);
            }

            else
            {
                var employee = await _context.Employees
                    .Include(x => x.EmployeeLocations)
                    .Where(x => x.Id == BusinessPartnerId).FirstOrDefaultAsync();
                if (employee == null) { throw new Exception("Employee not found"); }

                var location = employee.EmployeeLocations.Where(x => x.GeneralStatus == GeneralStatus.Active).OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                var employee_disbursement = EmployeeDisbursement.Create(Guid.NewGuid(), location.Id, employee.Id, transaction.Id);
                await _context.EmployeeDisbursements.AddAsync(employee_disbursement);

                EmailReceiver.Email = employee?.Email ?? "";
                EmailReceiver.Name = $"{employee?.FirstName ?? ""} {employee?.LastName ?? ""}";
                EmailReceiver.Code = employee?.Code ?? "";
                EmailReceiver.Id = employee?.Id ?? Guid.Empty;
            }

            return new SpecificTransactionCreationReturnDto { QrCode = qrCode, EmailReceiver = EmailReceiver};
        }


        public async Task ProcessTransactionItemsAsync(Transaction transaction, CreateTransactionDto createDto, Guid userId)
        {


            // Get all items in one query with dictionary for O(1) lookup
            var itemsPrice = await _context.Items
                .Where(x => x.ItemLocations.Any(l => l.LocationId == createDto.LocationId))
                .Select(x => new { x.SellingPrice, x.Id, x.Name, x.CostPrice })
                .ToDictionaryAsync(x => x.Id);

            if (createDto.TransactionType == TransactionType.PURC)
            {
                if (createDto.Items.Any(x => !x.UnitPrice.HasValue || x.UnitPrice.Value == 0)) throw new Exception("Purchase price is required for all items");
            }


            // Create Transaction Items
            var transactionItems = createDto.Items.Select(itemDto => TransactionItem.Create(
                Guid.NewGuid(),
                transaction.Id,
                itemDto.ItemId,
                itemDto.Quantity,
                //itemDto.UnitPrice.Value,
                createDto.TransactionType == TransactionType.PURC ? itemDto.UnitPrice.Value : itemsPrice[itemDto.ItemId].SellingPrice,
                //itemDto.Quantity * itemDto.UnitPrice.Value,
                createDto.TransactionType == TransactionType.PURC ? itemDto.UnitPrice.Value * itemDto.Quantity : itemsPrice[itemDto.ItemId].SellingPrice * itemDto.Quantity,
                userId,
                DateTime.UtcNow
            )).ToList(); // Materialize to avoid multiple enumeration

            // Pre-process DTO items for O(1) lookups
            var createDtoItemsDict = createDto.Items.ToDictionary(x => x.ItemId);



            // Get all stock levels for this location in one query
            var stockLevelsDict = await _context.StockLevels
                .Where(x => x.LocationId == createDto.LocationId)
                .ToDictionaryAsync(x => x.ItemId);

            var stockLevelsToBeCreated = new List<StockLevel>();
            var stockLevelsToBeUpdated = new Dictionary<Guid, StockLevel>(); // Use dictionary to avoid duplicates
            var itemsDelivered = new List<TransactionItemDelivered>();

            var utcDate = DateTime.SpecifyKind((DateTime)createDto.Date, DateTimeKind.Utc);

            decimal calculatedTotal = 0;
            var batchId = Guid.NewGuid();
            foreach (var transactionItem in transactionItems)
            {
                itemsPrice.TryGetValue(transactionItem.ItemId, out var item);

                var submittedItem = createDto.Items.FirstOrDefault(x => x.ItemId == transactionItem.ItemId);

                if (item == null) throw new Exception($"Item {transactionItem.ItemId} submitted was not found");

                var amount = createDto.TransactionType == TransactionType.PURC ? submittedItem.UnitPrice.Value * submittedItem.Quantity : item.SellingPrice * transactionItem.Quantity;
                calculatedTotal = calculatedTotal + amount;

                //if it is a pruchase transaction
                //check for price in the supplier price
                //if none exists , create new 
                //if what exists is same as what came, dont create again
                //if there is a difference in price from what alrady exists, create new 

                if (createDto.TransactionType == TransactionType.PURC)
                {
                    var existingSupplierPrice = await _context.SupplierItemCostPrices.FirstOrDefaultAsync(x => x.SupplierId == createDto.BusinessPartnerId && x.ItemId == item.Id && x.Price == transactionItem.UnitPrice);
                    if (existingSupplierPrice == null)
                    {
                        var newSupplierPrice = SupplierItemCostPrice.Create(Guid.NewGuid(), (Guid)createDto.BusinessPartnerId, item.Id, transactionItem.UnitPrice, userId.ToString(), DateTime.UtcNow);
                        await _context.SupplierItemCostPrices.AddAsync(newSupplierPrice);
                    }

                    if (submittedItem.UnitPrice > item?.CostPrice)
                        transactionItem.SetToPending();
                        //throw new Exception($"Selling Price of {item.Name} cannot be less than {item.SellingPrice}");
                }



                if (createDto.TransactionType == TransactionType.SALE)
                {
                    if (submittedItem.UnitPrice < item?.SellingPrice)
                        throw new Exception($"Selling Price of {item.Name} cannot be less than {item.SellingPrice}");
                }


                //

                createDtoItemsDict.TryGetValue(transactionItem.ItemId, out var itemPosted);

                //if (itemPosted?.DeliveredQuantity > 0)
                //{
                //    var transactionItemDelivered = TransactionItemDelivered.Create(
                //        Guid.NewGuid(),
                //        transactionItem.Id,
                //        batchId,
                //        itemPosted.DeliveredQuantity ?? 0,
                //        utcDate
                //    );
                //    itemsDelivered.Add(transactionItemDelivered);

                //}

                stockLevelsDict.TryGetValue(transactionItem.ItemId, out var stockLevel);

                if (transaction.TransactionType == TransactionType.SALE.ToString())
                {
                    if (stockLevel == null || stockLevel.AvailableQuanity < transactionItem.Quantity)
                    {
                        throw new Exception($"Insufficient stock for item {item?.Name}. Available: {stockLevel?.AvailableQuanity ?? 0}, Requested: {transactionItem.Quantity}");
                    }

                    var quantityForTransaction = itemPosted.DeliveredQuantity.HasValue && itemPosted.DeliveredQuantity.Value > 0 ? itemPosted?.DeliveredQuantity.Value : itemPosted.Quantity;
                    // Subtract the quantity delivered
                    //deposit - substract actual
                    if (transaction.TransactionResultsType == TransactionResultsType.Deposit)
                    {
                        stockLevel.SubstractActual((int)quantityForTransaction);
                    }

                    if (transaction.TransactionResultsType == TransactionResultsType.Debit)
                    {
                        // general sale, no customer deduct both av and ac
                        //customer sale, deduct available (deposit)
                        stockLevel.SubstractAvailable((int)quantityForTransaction);

                        var sale = await _context.Sales
                            .Include(x => x.Customer)
                            .Where(x => x.TransactionId == transactionItem.Id).FirstOrDefaultAsync();
                        if (sale?.Customer == null)
                        {
                            stockLevel.SubstractActual((int)quantityForTransaction);
                        }

                    }

                    // Track stock level once
                    if (stockLevelsToBeUpdated.ContainsKey(stockLevel.Id))
                    {
                        stockLevelsToBeUpdated.Remove(stockLevel.Id);
                        stockLevelsToBeUpdated.Add(stockLevel.Id, stockLevel);
                    }
                    else
                    {
                        stockLevelsToBeUpdated.Add(stockLevel.Id, stockLevel);

                    }

                }

                if (transaction.TransactionType == TransactionType.PURC.ToString())
                {
                    if (stockLevel == null)
                    {
                        //create Zero Stock
                        var newStockLevel = StockLevel.Create(
                            Guid.NewGuid(),
                            0,
                            0,
                            transactionItem.ItemId,
                            createDto.LocationId
                        );
                        stockLevelsToBeCreated.Add(newStockLevel);

                        // Add to dictionary to avoid duplicate creation if same item appears again
                        stockLevelsDict.Add(transactionItem.ItemId, newStockLevel);
                    }

                }

            }


            transaction.UpdateTotalAmountWithCalculatedValue(calculatedTotal);
            // Batch operations
            await _context.TransactionItems.AddRangeAsync(transactionItems);
            await _context.TransactionItemsDelivered.AddRangeAsync(itemsDelivered);

            if (stockLevelsToBeUpdated.Any())
                _context.StockLevels.UpdateRange(stockLevelsToBeUpdated.Values);
            if (stockLevelsToBeCreated.Any())
                await _context.StockLevels.AddRangeAsync(stockLevelsToBeCreated);
        }


        public async Task<(Transaction transaction, ApplicationUser user, Payment payment)> AddPaymentExternalCallAsync(Transaction transRecord, TransactionPaymentsDto createDto, TransactionResultsType? transResults) // TransactionPaymentsDto createDto,
        {
            var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var transactionNumber = await _transactionCodeRepository.GenerateTransactionCodeAsync("TXP", transRecord.LocationId);

                var emailReceiver = await GetEmailRceiverForExistsingTransaction(transRecord);

                var paymentReturnData = await AddPaymentInternalCallAsync(transRecord, createDto, null, transactionNumber, emailReceiver);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return (paymentReturnData.transaction, paymentReturnData.user, paymentReturnData.payment);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<(Transaction transaction, ApplicationUser user, Payment payment)> AddPaymentInternalCallAsync(Transaction transRecord, TransactionPaymentsDto createDto, Guid? batchId, string? paymentTransactionNumber, EmailReceiver? emailReceiver ) // TransactionPaymentsDto createDto,
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync();

            if (user == null) { throw new Exception("Invalid transaction"); }

            if (transRecord == null) { throw new Exception("Submitted transaction not found"); }

            Guid? couponId = null;
            if (!string.IsNullOrEmpty(createDto.CouponCode))
            {
                var coupon = await _context.Coupons.Where(x => x.Code.ToUpper().Trim() == createDto.CouponCode.ToUpper().Trim()).FirstOrDefaultAsync();
                if (coupon != null)
                {
                    if (coupon.Used)
                    {
                        throw new Exception("Coupon has been used already");
                    }

                    if (coupon.ExpiryDate.HasValue && coupon.ExpiryDate.Value.Date < DateTime.Today.Date)
                    {
                        throw new Exception("Coupon has expired");
                    }

                    couponId = coupon.Id;
                }
                else
                {
                    throw new Exception("Coupon not found");
                }
            }

            //validate amount being paid
            var totalAmount = transRecord.TotalAmount;
            var totalPayments = transRecord.TransactionPayments != null ? transRecord.TransactionPayments.Sum(x => x.Amount) : 0;

            var balance = totalAmount - (totalPayments + createDto.Amount);

           if (transRecord.TransactionType != TransactionType.SALE.ToString())
            {
                if (balance < 0)
                {
                    throw new Exception($"Balance left to be paid is {(totalAmount - totalPayments):N0}");
                }
            }

            //valid currency
            if (string.IsNullOrEmpty(createDto.CurrencyCode)) createDto.CurrencyCode = "GHS";
            var currencyCode = _context.Currencies.Where(x => x.Code.ToUpper().Trim() == createDto.CurrencyCode.ToUpper().Trim()).FirstOrDefault();
            if (currencyCode == null) { throw new Exception("Currency not found"); }

           // var transactionNumber = await _transactionCodeRepository.GenerateTransactionCodeAsync("PAY", transRecord.LocationId);

            var payment = Payment.Create(
                   Guid.NewGuid(),
                   transRecord.Id,
                   paymentTransactionNumber,
                   DateTime.SpecifyKind(createDto.PaymentDate, DateTimeKind.Utc),
                   createDto.Amount,
                   balance,
                   createDto.PaymentMethod,
                   DateTime.UtcNow,
                   Guid.Parse(user.Id),
                   transRecord.RequiresExternalApproval == false ? PaymentStatus.Completed : PaymentStatus.Pending,
                   currencyCode.Id,
                   couponId
              );


            if (transRecord.TotalAmount > 0 )
            {
                await _context.TransactionPayments.AddAsync(payment);
            }

            if (transRecord.RequiresExternalApproval)
            {
                await SaveTransactionEmailTemplate(transRecord, user, payment, batchId, emailReceiver);
            }

            return (transRecord, user, payment);
        }

        public async Task SaveTransactionEmailTemplate(Transaction transaction, ApplicationUser user, Payment payment, Guid? BatchId, EmailReceiver? emailReceiver)
        {

            bool isSale = transaction.TransactionType == TransactionType.SALE.ToString();
            bool isSaleReversal = transaction.TransactionType == TransactionType.SREV.ToString();
            bool isPurchase = transaction.TransactionType == TransactionType.PURC.ToString();
            bool isPurchaseReversal = transaction.TransactionType == TransactionType.PREV.ToString();
            bool isTransfer = transaction.TransactionType == TransactionType.TRAN.ToString();
            bool isTransferReversal = transaction.TransactionType == TransactionType.TRAN.ToString();

            var company = await _companyRepository.GetByIdAsync(user.CompanyId);


            var totalPayments = transaction.TransactionPayments != null ? transaction.TransactionPayments.Sum(x => x.Amount) : 0;
            var balance = transaction.TotalAmount - (totalPayments); //Payment has been added to database hence the sum here will automatically include the current payment being made


            // var token = payment is not null ? await _paymentTokenService.GenerateTokenAsync(receiverEmail ?? "", payment) : null;

            var location = await _context.Locations.FirstOrDefaultAsync(x => x.Id == transaction.LocationId);


            var emailItemQuanties = await GetTransactionItemsByType(transaction, BatchId);

            var emailItems = emailItemQuanties
                ?.Select(yz =>
                {
                    
                    return new EmailItem
                    {
                        Name = yz.Name ?? "N/A",
                        Price = yz.Price.ToString("N2", CultureInfo.InvariantCulture), // Fixed format
                        Quantity = yz.Quantity,
                        Amount = (yz.Quantity * yz.Price).ToString("N2", CultureInfo.InvariantCulture)
                    };
                    
                })
                .Where(e => e != null) // filter out nulls
                .ToList() ?? new List<EmailItem>();

            var emailTemplate = new AllEmailsTemplateModel
            {
                CompanyName = $"{company.Name} - {location?.Name ?? ""}",
                CompanyAddress = location?.Address ?? company?.Address ?? "",
                CompanyPhone = location?.Phone ?? company?.PhoneNumber ?? "",
                AppName = _emailSettings.AppName,
                ReceiverName = emailReceiver?.Name??"",
                PrimaryEmail = emailReceiver?.Email ?? "",
                Currency = payment?.Currency?.Code ?? "GHS",
                Cost = transaction?.TotalAmount,
                Amount = payment?.Amount,
                Date = payment?.PaymentDate ?? DateTime.UtcNow,
                Balance = balance,
                Reference = transaction?.TransactionNumber ?? "",
                PinCode = transaction?.TransactionNumber ?? "",
                Items = emailItems,
                //  Balance = payment.Amount - transaction.TotalAmount

            };

            var subject = "";
            var templateName = "";

            if (payment == null)
            {
              subject = isSale || isTransfer ? "Items Delivery" :
                        isSaleReversal || isPurchaseReversal || isTransferReversal ? "Items Reversal" :
                        isPurchase ? "Items Receival" : "Items Delivery";

                templateName = isSale || isTransfer ? "ItemsDelivery" :
                            isSaleReversal || isPurchaseReversal || isTransferReversal ? "ItemsReversal" :
                           isPurchase  ? "ItemsReceival" : "ItemsDelivery" ;
            } 
            else
            {
                subject = "Payment Confirmation";
                templateName = "CustomerPayment";
            }

            var queuedEmail = new QueuedEmail
            {
                To = emailReceiver?.Email??"",
                Subject = subject,
                TemplateName = templateName,
                TemplateModelJson = JsonSerializer.Serialize(emailTemplate),
                TemplateModelType = typeof(AllEmailsTemplateModel).AssemblyQualifiedName,
                ReceiverId = emailReceiver?.Id??Guid.Empty,
                CreatedAt = DateTime.UtcNow,
                //ReceiverId = Guid.Empty,
                Status = EmailQueueStatus.Pending
            };

            await _context.QueuedEmails.AddAsync(queuedEmail);
        }

        public async Task<IReadOnlyList<IdAndNameQtyDTO>> GetTransactionItemsByType(
         Transaction? transaction,
         Guid? BatchId 
         )
        {
            // Early exit if transaction is null
            if (transaction is null)
                return Array.Empty<IdAndNameQtyDTO>();

            // Guard: Ensure TransactionItems is loaded (if using EF, you should .Include() it)
            var items = transaction.TransactionItems;
            if (items is null || !items.Any())
                return Array.Empty<IdAndNameQtyDTO>();

            bool isSale = transaction.TransactionType == TransactionType.SALE.ToString();
            bool isSaleReversal = transaction.TransactionType == TransactionType.SREV.ToString();
            bool isPurchase = transaction.TransactionType == TransactionType.PURC.ToString();
            bool isPurchaseReversal = transaction.TransactionType == TransactionType.PREV.ToString();
            bool isTransfer = transaction.TransactionType == TransactionType.TRAN.ToString();
            // Determine which path to take based on transResults

            var transactionItemsIds = items.Select(i => i.ItemId).ToList();
            var itemNames =  await _context.Items.Where(x => transactionItemsIds.Contains(x.Id)).Select(x=> new { x.Id, x.Name}).ToDictionaryAsync(x=> x.Id);
             IReadOnlyList<IdAndNameQtyDTO> results = null;
           if (BatchId.HasValue && BatchId.Value != Guid.Empty)
            {
                if (isSaleReversal || isPurchaseReversal)
                {
                    results = items
                        .SelectMany(ti => ti.TransactionItemsDelivered.Where(x => BatchId.HasValue && BatchId.Value != Guid.Empty && x.BatchId == BatchId) ?? Enumerable.Empty<TransactionItemDelivered>())
                        .SelectMany(delivered => delivered.TransactionItemReversals ?? Enumerable.Empty<TransactionItemReversal>())
                        .Select(reversal => SafeCreateDto(
                            reversal?.TransactionItemDelivered?.TransactionItem?.ItemId,
                            itemNames[(Guid)reversal?.TransactionItemDelivered?.TransactionItem?.ItemId]?.Name,
                            reversal?.Quantity ?? 0,
                            reversal?.TransactionItemDelivered?.TransactionItem?.UnitPrice ?? 0))
                        .Where(dto => dto.Id != Guid.Empty) // filter out invalid entries
                        .ToArray();
                }

                if (isSale)
                {
                    results = items
                        .SelectMany(ti => ti.TransactionItemsDelivered.Where(x => BatchId.HasValue && BatchId.Value != Guid.Empty && x.SaleTransDeliveryRequestId == BatchId ) ?? Enumerable.Empty<TransactionItemDelivered>())
                        .Select(delivered => SafeCreateDto(
                            delivered?.TransactionItem?.ItemId,
                            itemNames[(Guid)delivered?.TransactionItem?.ItemId]?.Name,
                            delivered?.Quantity ?? 0,
                            delivered?.TransactionItem?.UnitPrice ?? 0))
                        .Where(dto => dto.Id != Guid.Empty)
                        .ToArray();
                }

                if (isPurchase || isTransfer)
                {
                    results = items
                        .SelectMany(ti => ti.TransactionItemsDelivered ?? Enumerable.Empty<TransactionItemDelivered>())
                        .Select(delivered => SafeCreateDto(
                            delivered?.TransactionItem?.ItemId,
                            itemNames[(Guid)delivered?.TransactionItem?.ItemId]?.Name,
                            delivered?.Quantity ?? 0,
                            delivered?.TransactionItem?.UnitPrice ?? 0))
                        .Where(dto => dto.Id != Guid.Empty)
                        .ToArray();
                }
            }


            if ( results == null)
            {
                results = items
                        .Select(ti => new IdAndNameQtyDTO
                        {
                            Id = ti.ItemId,
                            Name = itemNames[ti.ItemId]?.Name ?? "N/A",
                            Quantity = ti.Quantity,
                            Price = ti.UnitPrice //if available?,
                        })
                        .ToArray();
            }

            return results;


            //    var result = (string)transaction.TransactionType switch
            //{
            //    TransactionType.SALE.ToString() =>
            //        // Reversal can be from Delivery or Receival? You used TransactionItemsDelivered.
            //        items
            //            .SelectMany(ti => ti.TransactionItemsDelivered ?? Enumerable.Empty<TransactionItemDelivered>())
            //            .SelectMany(delivered => delivered.TransactionItemReversals ?? Enumerable.Empty<TransactionItemReversal>())
            //            .Select(reversal => SafeCreateDto(
            //                reversal?.TransactionItemDelivered?.TransactionItem?.ItemId,
            //                reversal?.TransactionItemDelivered?.TransactionItem?.Item?.Name,
            //                reversal?.Quantity ?? 0,
            //                reversal?.TransactionItemDelivered?.TransactionItem?.UnitPrice ?? 0))
            //            .Where(dto => dto.Id != Guid.Empty) // filter out invalid entries
            //            .ToArray(),


            //    TransactionResultsType.SaleDelivery =>
            //        // Reversal can be from Delivery or Receival? You used TransactionItemsDelivered.
            //        items
            //            .SelectMany(ti => ti.TransactionItemsDelivered ?? Enumerable.Empty<TransactionItemDelivered>())
            //            .Select(delivered => delivered.SaleTransDeliveryRequest ??  SaleTransDeliveryRequest())
            //            .Select(reversal => SafeCreateDto(
            //                reversal?.TransactionItemDelivered?.TransactionItem?.ItemId,
            //                reversal?.TransactionItemDelivered?.TransactionItem?.Item?.Name,
            //                reversal?.Quantity ?? 0,
            //                reversal?.TransactionItemDelivered?.TransactionItem?.UnitPrice ?? 0))
            //            .Where(dto => dto.Id != Guid.Empty) // filter out invalid entries
            //            .ToArray(),


            //    TransactionResultsType.Delivery =>
            //        items
            //            .SelectMany(ti => ti.TransactionItemsDelivered ?? Enumerable.Empty<TransactionItemDelivered>())
            //            .Select(delivered => SafeCreateDto(
            //                delivered?.TransactionItem?.ItemId,
            //                delivered?.TransactionItem?.Item?.Name,
            //                delivered?.Quantity ?? 0,
            //                delivered?.TransactionItem?.UnitPrice ?? 0))
            //            .Where(dto => dto.Id != Guid.Empty)
            //            .ToArray(),

            //    TransactionResultsType.TransferReceival =>
            //        items
            //            .SelectMany(ti => ti.TransactionItemReceived ?? Enumerable.Empty<TransactionItemReceived>())
            //            .Select(received => SafeCreateDto(
            //                received?.TransactionItem?.ItemId,
            //                received?.TransactionItem?.Item?.Name,
            //                received?.Quantity ?? 0,
            //                received?.TransactionItem?.UnitPrice ?? 0))
            //            .Where(dto => dto.Id != Guid.Empty)
            //            .ToArray(),

            //    TransactionResultsType.TransferReversal =>
            //        items
            //            .SelectMany(ti => ti.TransactionItemReceived ?? Enumerable.Empty<TransactionItemReceived>())
            //            .SelectMany(received => received.TransactionItemReversals ?? Enumerable.Empty<TransactionItemReversal>())
            //            .Select(reversal => SafeCreateDto(
            //                reversal?.TransactionItemDelivered?.TransactionItem?.ItemId,
            //                reversal?.TransactionItemDelivered?.TransactionItem?.Item?.Name,
            //                reversal?.Quantity ?? 0,
            //                reversal?.TransactionItemDelivered?.TransactionItem?.UnitPrice ?? 0))
            //            .Where(dto => dto.Id != Guid.Empty)
            //            .ToArray(),

            //    // Fallback: Customer Sale, Purchase from Supplier, Stock Transfer Request
            //    _ =>
            //        items
            //            .Select(ti => new IdAndNameQtyDTO
            //            {
            //                Id = ti.ItemId,
            //                Name = ti.Item?.Name ?? "N/A",
            //                Quantity = ti.Quantity,
            //                Price = 0 // or ti.UnitPrice if available?
            //            })
            //            .ToArray()
            //};

        }

        // Helper method to safely create DTO with fallback values
        private IdAndNameQtyDTO SafeCreateDto(Guid? id, string? name, int quantity, decimal price)
        {
            return new IdAndNameQtyDTO
            {
                Id = id ?? Guid.Empty,
                Name = name ?? "",
                Quantity = quantity,
                Price = price
            };
        }

        public async Task<IEnumerable<TransactionItemsReceivedDto>> GetAllDeliveredItemsToDate(Guid transactionId)
        {
            var transactionItemsRecieved = _context.TransactionItemsDelivered
                 .Include(x => x.TransactionItem)
                     .ThenInclude(x => x.Item)
                 .Where(x => x.TransactionItem.TransactionId == transactionId).AsNoTracking();

            return await transactionItemsRecieved.OrderByDescending(x => x.DeliveryDate).Select(x => new TransactionItemsReceivedDto
            {
                Id = x.Id,
                Date = x.DeliveryDate.Date,
                ItemName = x.TransactionItem.Item.Name,
                Quantity = x.Quantity
            }).ToListAsync();
        }


        public async Task<string> DeliverItems(
            ConfirmTransactionDeliveryDTO createDto,
            ApplicationUser user,
            SaleTransDeliveryRequest? saleTransDeliveryRequest,
            CancellationToken cancellationToken = default)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var qrCode = string.Empty;
                var userId = Guid.Parse(user.Id);

                // 1. Validate location access
                var validLocation = await _employeeLocationRepository.HasAccessToLocation(createDto.LocationId)
                    ?? throw new Exception("Invalid location");

                // 2. Fetch or resolve transaction record
                var transRecord = saleTransDeliveryRequest?.Sale?.Transaction
                    ?? await _context.Transactions
                        .Include(t => t.TransactionItems)
                            .ThenInclude(t => t.TransactionItemsDelivered)
                        .Include(t => t.TransactionItems)
                            .ThenInclude(t => t.Item)
                        .FirstOrDefaultAsync(p => p.Id == createDto.TransactionId, cancellationToken)
                    ?? throw new Exception("Invalid request, record not found");

                // 3. Sort item IDs deterministically to prevent database deadlocks
                var sortedSubmittedItemIds = createDto?.Items?
                    .Select(x => x.ItemId)
                    .OrderBy(id => id)
                    .ToList();

                if (sortedSubmittedItemIds == null || !sortedSubmittedItemIds.Any())
                    throw new Exception("No items found for delivery");

                var validSubmitted = transRecord.TransactionItems
                    .Where(x => sortedSubmittedItemIds.Contains(x.ItemId))
                    .ToList();

                if (validSubmitted.Count != sortedSubmittedItemIds.Count)
                    throw new Exception("Invalid request: one or more submitted items were not found in the transaction.");

                // 4. PostgreSQL: Set session lock timeout to 2 seconds
                await _context.Database.ExecuteSqlRawAsync("SET LOCAL lock_timeout = '2s';", cancellationToken);

                // 5. PostgreSQL: Resolve schema and table names dynamically to build quoted SQL
                var entityType = _context.Model.FindEntityType(typeof(StockLevel));
                var tableName = entityType?.GetTableName() ?? "StockLevels";
                var schema = entityType?.GetSchema();

                var quotedTable = string.IsNullOrEmpty(schema)
                    ? $"\"{tableName}\""
                    : $"\"{schema}\".\"{tableName}\"";

                // Offset parameters by +1 so @p0 is kept exclusively for LocationId ({0})
                var itemIdParams = sortedSubmittedItemIds
                    .Select((id, i) => new NpgsqlParameter($"@p{i + 1}", id))
                    .ToArray();

                var inClause = string.Join(", ", itemIdParams.Select(p => p.ParameterName));

                var sql = $@"
            SELECT * FROM {quotedTable}
            WHERE ""LocationId"" = {{0}} AND ""ItemId"" IN ({inClause})
            ORDER BY ""ItemId""
            FOR UPDATE";

                var parameters = new object[] { validLocation.Id }.Concat(itemIdParams).ToArray();

                var stockLevels = await _context.StockLevels
                    .FromSqlRaw(sql, parameters)
                    .ToListAsync(cancellationToken);

                var itemsDelivered = new List<TransactionItemDelivered>();
                var stockLevelsToBeCreated = new List<StockLevel>();
                var stockLevelsToBeUpdated = new List<StockLevel>();
                var transactionItemsDeliveredToBeUpdated = new List<TransactionItemDelivered>();
                var batchId = Guid.NewGuid();

                // 6. Handle optional transportation costs
                if (createDto?.Transportation > 0)
                {
                    var newTransportation = TransactionTransportation.Create(
                        Guid.NewGuid(),
                        transRecord.Id,
                        (decimal)createDto.Transportation,
                        DateTime.SpecifyKind(createDto.Date, DateTimeKind.Utc));

                    await _context.TransactionTransportations.AddAsync(newTransportation, cancellationToken);
                }

                SaleTransDeliveryRequest? saleTransDeliveryRequestToBeCreated = null;

                // Load sales context dictionary for location
                var salesDictionary = await _context.Sales
                    .Where(s => s.Transaction.LocationId == validLocation.Id)
                    .Select(s => new { s.TransactionId, s.CustomerId, s.Id })
                    .ToDictionaryAsync(s => s.TransactionId, s => new { s.CustomerId, s.Id }, cancellationToken);

                // 7. Process delivery calculations and stock modifications
                foreach (var transactionItem in validSubmitted)
                {
                    var itemPosted = createDto?.Items?.FirstOrDefault(x => x.ItemId == transactionItem.ItemId)
                        ?? throw new Exception("Invalid item submitted");

                    var orderedQuantity = transactionItem.Quantity;
                    var alreadyDeliveredQty = transactionItem.TransactionItemsDelivered
                        .Where(x => x.IsDelivered)
                        .Sum(x => x.Quantity);

                    if (itemPosted.Quantity <= 0)
                        throw new Exception($"Quantity delivered for {transactionItem.Item.Name} must be greater than zero.");

                    if (itemPosted.Quantity + alreadyDeliveredQty > orderedQuantity)
                        throw new Exception($"Quantity delivered for {transactionItem.Item.Name} exceeds quantity ordered.");

                    bool isSale = transRecord.TransactionType == TransactionType.SALE.ToString();

                    if (isSale && saleTransDeliveryRequest == null && saleTransDeliveryRequestToBeCreated == null)
                    {
                        if (salesDictionary.TryGetValue(transRecord.Id, out var saleInfo) &&
                            saleInfo.CustomerId.HasValue && saleInfo.CustomerId != Guid.Empty)
                        {
                            saleTransDeliveryRequestToBeCreated = SaleTransDeliveryRequest.Create(
                                Guid.NewGuid(),
                                saleInfo.Id,
                                false,
                                DateTime.UtcNow,
                                user.Id,
                                DateTime.SpecifyKind(createDto.Date, DateTimeKind.Utc));

                            await _context.SaleTransDeliveryRequests.AddAsync(saleTransDeliveryRequestToBeCreated, cancellationToken);
                            qrCode = _qrCodeService.GenerateQrCodeBase64(saleTransDeliveryRequestToBeCreated.Id.ToString());
                        }
                    }

                    bool isDelivered = !isSale || (isSale && saleTransDeliveryRequest != null);

                    if (saleTransDeliveryRequest != null &&
                       (!saleTransDeliveryRequest.IsDelivered || transactionItem.TransactionItemsDelivered.Any(x => !x.IsDelivered)))
                    {
                        var transItemDelivered = transactionItem.TransactionItemsDelivered
                            .FirstOrDefault(x => !x.IsDelivered);

                        if (transItemDelivered != null)
                        {
                            transItemDelivered.Delivered(Guid.Parse(user.Id), saleTransDeliveryRequest.DeliveryDate);
                            transactionItemsDeliveredToBeUpdated.Add(transItemDelivered);
                        }
                    }
                    else
                    {
                        var deliveryRequestId = isSale
                            ? (saleTransDeliveryRequest != null ? saleTransDeliveryRequest.Id : saleTransDeliveryRequestToBeCreated?.Id)
                            : null;

                        var transactionItemDelivered = TransactionItemDelivered.Create(
                            Guid.NewGuid(),
                            transactionItem.Id,
                            deliveryRequestId,
                            batchId,
                            itemPosted.Quantity,
                            DateTime.SpecifyKind(createDto.Date, DateTimeKind.Utc),
                            isDelivered);

                        itemsDelivered.Add(transactionItemDelivered);
                    }

                    if (isDelivered)
                    {
                        var stockLevel = stockLevels.FirstOrDefault(x => x.ItemId == transactionItem.ItemId);

                        if (transRecord.TransactionType == TransactionType.PURC.ToString())
                        {
                            if (stockLevel == null)
                            {
                                var newStockLevel = StockLevel.Create(Guid.NewGuid(), itemPosted.Quantity, itemPosted.Quantity, transactionItem.ItemId, validLocation.Id);
                                stockLevelsToBeCreated.Add(newStockLevel);
                                stockLevels.Add(newStockLevel);
                            }
                            else
                            {
                                stockLevel.AddActual(itemPosted.Quantity);
                                stockLevel.AddAvailable(itemPosted.Quantity);
                                stockLevelsToBeUpdated.Add(stockLevel);
                            }
                        }
                        else if (isSale || transRecord.TransactionType == TransactionType.TRAN.ToString())
                        {
                            if (stockLevel == null || stockLevel.AvailableQuanity < itemPosted.Quantity)
                            {
                                throw new Exception($"Insufficient stock for item {transactionItem.Item.Name}. Available: {stockLevel?.AvailableQuanity ?? 0}, Requested: {itemPosted.Quantity}");
                            }

                            stockLevel.SubstractAvailable(itemPosted.Quantity);
                            stockLevelsToBeUpdated.Add(stockLevel);
                        }
                    }
                }

                if (saleTransDeliveryRequest != null)
                {
                    saleTransDeliveryRequest.Delivered(user.Id);
                    _context.SaleTransDeliveryRequests.Update(saleTransDeliveryRequest);
                }

                // 8. Apply persistence updates
                if (itemsDelivered.Any()) await _context.TransactionItemsDelivered.AddRangeAsync(itemsDelivered, cancellationToken);
                if (transactionItemsDeliveredToBeUpdated.Any()) _context.TransactionItemsDelivered.UpdateRange(transactionItemsDeliveredToBeUpdated);
                if (stockLevelsToBeUpdated.Any()) _context.StockLevels.UpdateRange(stockLevelsToBeUpdated.Distinct());
                if (stockLevelsToBeCreated.Any()) await _context.StockLevels.AddRangeAsync(stockLevelsToBeCreated, cancellationToken);

                // 9. Dispatch notification email if fully delivered
                if (saleTransDeliveryRequest != null &&
                    saleTransDeliveryRequest.IsDelivered &&
                    saleTransDeliveryRequest.TransactionItemDelivered != null && saleTransDeliveryRequest.TransactionItemDelivered.All(x => x.IsDelivered))
                {
                    var emailReceiver = await GetEmailRceiverForExistsingTransaction(transRecord);
                    await SaveTransactionEmailTemplate(transRecord, user, null,
                      saleTransDeliveryRequestToBeCreated != null ? saleTransDeliveryRequestToBeCreated.Id :  saleTransDeliveryRequest != null ? saleTransDeliveryRequest.Id : batchId, 
                      emailReceiver);
                }

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return qrCode;
            }
            catch (PostgresException ex) when (ex.SqlState == "55P03") // Lock timeout
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException("High delivery volume for these items. Please try your request again in a moment.", ex);
            }
            catch (PostgresException ex) when (ex.SqlState == "40P01") // Deadlock detected
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new InvalidOperationException("A concurrency conflict occurred. Please retry your request.", ex);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task <EmailReceiver> GetEmailRceiverForExistsingTransaction(Transaction transRecord)
        {
            var EmailReceiver = new EmailReceiver();
            if (transRecord.TransactionType == TransactionType.PURC.ToString())
            {
                var purchase = await _context.Purchases
                    .Include(x => x.Supplier)
                    .Where(x => x.TransactionId == transRecord.Id)
                    .Select(x => new { x.Supplier })
                    .FirstOrDefaultAsync();

                if (purchase == null || purchase.Supplier == null) { throw new Exception("Supplier not found"); }


                EmailReceiver.Email = purchase.Supplier?.Email ?? "";
                EmailReceiver.Name = $"{purchase.Supplier?.FirstName ?? ""} {purchase?.Supplier?.LastName ?? ""}";
                EmailReceiver.Code = purchase?.Supplier?.Code ?? "";
                EmailReceiver.Id = purchase?.Supplier?.Id ?? Guid.Empty;
            }
            else if (transRecord.TransactionType == TransactionType.SALE.ToString())
            {
                var sale = await _context.Sales
                    .Include(x => x.Customer)
                    .Where(x => x.TransactionId == transRecord.Id)
                    .Select(x => new { x.Customer })
                    .FirstOrDefaultAsync();

                if (sale == null || sale.Customer == null) { throw new Exception("Customer not found"); }


                var customer = sale.Customer;

                EmailReceiver.Email = customer?.Email ?? "";
                EmailReceiver.Name = $"{customer?.FirstName ?? ""} {customer?.LastName ?? ""}";
                EmailReceiver.Code = customer?.Code ?? "";
                EmailReceiver.Id = customer?.Id ?? Guid.Empty;

            }

            else if (transRecord.TransactionType == TransactionType.DEPO.ToString())
            {

                var financialDeposit = await _context.FinancialServiceDisbursement
                   .Include(x => x.ContactPerson)
                   .Where(x => x.TransactionId == transRecord.Id)
                   .Select(x => new { x.ContactPerson })
                   .FirstOrDefaultAsync();

                if (financialDeposit == null || financialDeposit.ContactPerson == null) { throw new Exception("Customer not found"); }


                var fscp = financialDeposit.ContactPerson;

                EmailReceiver.Email = fscp?.Email ?? "";
                EmailReceiver.Name = $"{fscp?.FirstName ?? ""} {fscp?.LastName ?? ""}";
                EmailReceiver.Code = fscp?.Code ?? "";
                EmailReceiver.Id = fscp?.Id ?? Guid.Empty;

            }
            else if (transRecord.TransactionType == TransactionType.TRAN.ToString())
            {
                //if (BusinessPartnerId.HasValue == false) throw new Exception("A destination or target shop is required for this transaction to compltete");
                //var stockTransferRequest = StockTransfer.Create(Guid.NewGuid(), transaction.Id, transaction.TransactionDate, StockTransferEnum.Pending, transaction.LocationId, (Guid)BusinessPartnerId, Guid.Parse(user.Id), DateTime.UtcNow);
                //await _context.StockTransfers.AddAsync(stockTransferRequest);
            }

            else
            {

                //var financialDeposit = await _context.FinancialServiceDisbursement
                // .Include(x => x.ContactPerson)
                // .Where(x => x.TransactionId == transRecord.Id)
                // .Select(x => new { x.ContactPerson })
                // .FirstOrDefaultAsync();

                //if (financialDeposit == null || financialDeposit.ContactPerson == null) { throw new Exception("Customer not found"); }


                //var fscp = financialDeposit.ContactPerson;

                //EmailReceiver.Email = fscp?.Email ?? "";
                //EmailReceiver.Name = $"{fscp?.FirstName ?? ""} {fscp?.LastName ?? ""}";
                //EmailReceiver.Code = fscp?.Code ?? "";
                //EmailReceiver.Id = fscp?.Id ?? Guid.Empty;

                //var employee = await _context.Fi
                //    .Include(x => x.EmployeeLocations)
                //    .Where(x => x.Id == BusinessPartnerId).FirstOrDefaultAsync();
                //if (employee == null) { throw new Exception("Employee not found"); }

                //var location = employee.EmployeeLocations.Where(x => x.GeneralStatus == GeneralStatus.Active).OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                //var employee_disbursement = EmployeeDisbursement.Create(Guid.NewGuid(), location.Id, employee.Id, transaction.Id);
                //await _context.EmployeeDisbursements.AddAsync(employee_disbursement);

                //EmailReceiver.Email = employee?.Email ?? "";
                //EmailReceiver.Name = $"{employee?.FirstName ?? ""} {employee?.LastName ?? ""}";
                //EmailReceiver.Code = employee?.Code ?? "";
                //EmailReceiver.Id = employee?.Id ?? Guid.Empty;
            }


            return EmailReceiver;
        }

        public async Task<TransactionCreatedReturnDataDto> CompleteTransationProcess(CreateTransactionDto createDto, TransactionResultsType? transResult)
        {


            var atomicTransaction = await _context.Database.BeginTransactionAsync();

            
            try
            {

                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null) throw new Exception("User not found");

                var location = await _locationRepository.GetByIdAsync(createDto.LocationId);
                if (location == null) throw new Exception("Please note that the Shop for this transaction was not found");

                //generate transaction numbers to avaiod premature  save changes in transaction

                var mainTransactionTransNumber = await _transactionCodeRepository.GenerateTransactionCodeAsync(createDto.TransactionResultsType.ToString(), location.Id);

                var paymentTransactionNumber = await _transactionCodeRepository.GenerateTransactionCodeAsync("PAY", location.Id);

                //create tranaction
                var transactionResults = await CreateGeneralTransactionAsync(createDto, location, user, mainTransactionTransNumber);

                var ReturnDto = await CreateSpecificTypeOfTransaction(transactionResults, user, createDto.BusinessPartnerId.HasValue ? createDto.BusinessPartnerId.Value : Guid.Empty);
                //process items
                await ProcessTransactionItemsAsync(transactionResults, createDto, Guid.Parse(user.Id));

                var transactionPayemntDto = new TransactionPaymentsDto
                {
                    Amount = createDto.AmountPaid,
                    CurrencyCode = createDto.CurrencyCode ?? "",
                    PaymentDate = (DateTime)createDto.Date,
                    PaymentMethod = createDto.PaymentMethod,
                    Remarks = createDto.Remarks,
                    TransationId = transactionResults.Id,
                    CouponCode = createDto.CouponCode ?? "",
                   
                };

                //add paymentUtcNow
                await AddPaymentInternalCallAsync(transactionResults, transactionPayemntDto, null, paymentTransactionNumber, ReturnDto.EmailReceiver);

                await _context.SaveChangesAsync();
                await atomicTransaction.CommitAsync();

                return new TransactionCreatedReturnDataDto { QrCode = ReturnDto.QrCode, TransactionNumber = transactionResults.TransactionNumber };
            }
            catch (Exception ex)
            {
                await atomicTransaction.RollbackAsync();
                throw;
            }

        }

        public async Task<GetTransactionDto> GetTransactionDetails(Guid transactionId)
        {
            try
            {
                var query = await _context.Transactions
                    .AsNoTracking() // Optional but good for read-only
                    .Include(c => c.Location)
                    .Include(p => p.TransactionPayments)
                        .ThenInclude(p => p.Coupon) // Make sure Coupon is loaded
                    .Include(t => t.TransactionItems)
                        .ThenInclude(ti => ti.Item) // Make sure Item is loaded
                    .Include(t => t.TransactionItems)
                        .ThenInclude(ti => ti.TransactionItemsDelivered)
                            .ThenInclude(d => d.TransactionItemReversals)
                    .Include(t => t.TransactionItems)
                        .ThenInclude(x => x.TransactionItemReceived)
                            .ThenInclude(d => d.TransactionItemReversals)
                    .FirstOrDefaultAsync(x => x.Id == transactionId);

                if (query == null)
                    return new GetTransactionDto();

                var returnData = new GetTransactionDto
                {
                    Id = query.Id,
                    TransactionCode = query.TransactionNumber,
                    LocationName = query.Location?.Name ?? "", // Simplified null check
                    CreatedAt = query.CreatedAt,
                    TransactionDate = query.TransactionDate,
                    TotalAmount = query.TotalAmount,
                    LocationId = query.LocationId,
                    Items = query.TransactionItems?.Select(z => new GetTransactionItemDto
                    {
                        Id = z.Id,
                        ItemId = z.ItemId,
                        Name = z.Item.Name,
                        Code = z.Item.Code, // Will this ever be null?
                        Quantity = z.Quantity, // Fixed spelling
                        UnitPrice = z.UnitPrice,

                        ItemsDelivered = z.TransactionItemsDelivered?.Select(d => new GetTransactionItemsDeliveredDto
                        {
                            DeliveryDate = d.DeliveryDate,
                            DeliveryId = d.Id,
                            Quantity = d.Quantity,
                            Status = d.SaleTransDeliveryRequestId.HasValue  && d.SaleTransDeliveryRequestId.Value != Guid.Empty ? d.IsDelivered ? "Delivered" : "Pending" :"N/A",
                            TransactionDeliveryRequestId = d.SaleTransDeliveryRequestId ?? Guid.Empty,
                            ItemReversals = d.TransactionItemReversals?.Select(r => new TransactionItemsReversalDto
                            {
                                Quantity = r.Quantity,
                                ReversalDate = r.ReversalDate,
                                ReversalId = r.Id
                            }).ToList() ?? new List<TransactionItemsReversalDto>() // Return empty list instead of null
                        }).OrderByDescending(x=> x.DeliveryDate).ToList() ?? new List<GetTransactionItemsDeliveredDto>(),


                        ItemsReceived = z.TransactionItemReceived?.Select(d => new GetTransactionItemsReceivedDto
                        {
                            DeliveryDate = d.DateReceived,
                            DeliveryId = d.Id,
                            Quantity = d.Quantity,
                            TransactionDeliveryRequestId = d.BatchId,
                            ItemReversals = d.TransactionItemReversals?.Select(r => new TransactionItemsReversalDto
                            {
                                Quantity = r.Quantity,
                                ReversalDate = r.ReversalDate,
                                ReversalId = r.Id,

                            }).ToList() ?? new List<TransactionItemsReversalDto>() // Return empty list instead of null
                        }).ToList() ?? new List<GetTransactionItemsReceivedDto>()
                    }).ToList() ?? new List<GetTransactionItemDto>(),

                    Payments = query.TransactionPayments?.Select(z => new GetTransactionPaymentsDto
                    {
                        PaymentDate = z.PaymentDate,
                        Amount = z.Amount,
                        PaymentMethodName = z.PaymentMethod.ToString(),
                        PaymentMethod = z.PaymentMethod,
                        TransactionId = z.TransactionId,
                        Coupon = z.Coupon != null ? new GetPaymentCouponDto
                        {
                            Amount = z.Coupon.Amount,
                            Code = z.Coupon.Code
                        } : null // Or new GetPaymentCouponDto() if you prefer
                    }).ToList() ?? new List<GetTransactionPaymentsDto>(),
                };


                return returnData;
            }
            catch (Exception ex)
            {
                // Log the error if you have logging
                // _logger.LogError(ex, "Error getting transaction {TransactionId}", transactionId);
                throw; // Or return new GetTransactionDto() if you want to swallow errors
            }
        }

        public async Task<TransactionCreatedReturnDataDto> DeliveryRequest(Guid TransactionId, DateTime DeliveryDate)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync();

            if (user == null) { throw new Exception("User not found"); }

            var sale = await _context.Sales
                .Include(x => x.Transaction)
                .Where(x => x.TransactionId == TransactionId)
                .FirstOrDefaultAsync();

            if (sale == null) throw new Exception("Transaction not found");

            //check if delivery request exists alrady
            var deliveryRequest = await _context.SaleTransDeliveryRequests.FirstOrDefaultAsync(x => x.SaleId == sale.TransactionId && x.IsDelivered == false || x.TransactionItemDelivered.All(x=> x.IsDelivered));

            if (deliveryRequest is not null) throw new Exception("There exists a delivery request that is pending completion");

            var newDeliveryRequest = SaleTransDeliveryRequest.Create(Guid.NewGuid(), sale.Id, false, DateTime.UtcNow, user.Id, DeliveryDate);

            //generate QR Code using the deliveryRequestId
            var qrCode = _qrCodeService.GenerateQrCodeBase64(newDeliveryRequest.Id.ToString(), 300, 300);
            await _context.SaleTransDeliveryRequests.AddRangeAsync(newDeliveryRequest);
            await _context.SaveChangesAsync();

            return new TransactionCreatedReturnDataDto { QrCode = qrCode, TransactionNumber = sale.Transaction.TransactionNumber };
        }


        public async Task CancelAsync(TransactionCancellationDto createDto)
        {
            var user = await _userRepository.GetUserByRefreshTokenAsync();
            if (user == null) throw new Exception("Unauthorized");

            var data = await _context.Transactions
                        .Include(t => t.TransactionItems)
                            .ThenInclude(x => x.TransactionItemsDelivered)
                        .FirstOrDefaultAsync(p => p.Id == createDto.Id);

            if (data == null)
                throw new Exception("Transaction not found or already cancelled");

            if (data.TransactionItems.Any(x => x.TransactionItemsDelivered.Any()))
            {
                throw new Exception("Transaction has Items delivered, hence cannot be cancelled");
            }

            if (data.GeneralStatus != GeneralStatus.Cancelled) throw new Exception("Transaction has already been cancelled");


            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {

                var now = DateTime.UtcNow;

                // Cancel delete the associated transaction
                 data.Cancel(now, Guid.Parse(user.Id));

                    // Soft delete all transaction items
                foreach (var item in data.TransactionItems)
                {
                    item.Cancel(Guid.Parse(user.Id), now);
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }


    }
}