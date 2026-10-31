using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;
using WebApplication1.DAL;
using WebApplication1.DAL.Repositories;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class StockAdjustmentService : IStockAdjustmentService
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly ITransactionService _transactionService;
        


        public StockAdjustmentService(
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
            var adjstments = await _context.StockAdjustments
                        .Include(p => p.Supplier)
                        //.Include(p => p.ApprovedBy)
                        .Include(x => x.Transaction)
                                .ThenInclude(x=> x.TransactionPayments)
                         .Include(x => x.Transaction)
                                .ThenInclude(t => t.TransactionItems)
                                    .ThenInclude(ti => ti.TransactionItemsDelivered)

                .FirstOrDefaultAsync(p => p.Id == id && p.Transaction.GeneralStatus == generalStatus);

            if (adjstments == null)
                return null;

            return new GetPurchaseDto
            {
                Id = adjstments.Id,  // Use the actual Purchase Id
                TransactionId = adjstments.TransactionId,  // Add separate field if needed
                TransactionCode = adjstments.Transaction.TransactionNumber,
                LocationName = adjstments.Transaction?.Location?.Name,
                SupplierId = adjstments.SupplierId,
                SupplierName = $"{adjstments.Supplier?.FirstName ?? ""} {adjstments.Supplier?.LastName ?? ""}".Trim(),
                // TransactionBy = purchase.Cr?.FullName,
                CreatedAt = adjstments.Transaction.CreatedAt,
                TransactionDate = adjstments.Transaction?.TransactionDate ?? DateTime.MinValue,
                TotalAmount = adjstments.Transaction?.TotalAmount ?? 0,
                PaidAmount = adjstments.Transaction?.TransactionPayments?.Sum(x => x.Amount) ?? 0,
                Balance = (adjstments.Transaction?.TotalAmount ?? 0) -
                                  (adjstments.Transaction?.TransactionPayments?.Sum(x => x.Amount) ?? 0)
            };
        }


        public async Task<PagedResult<GetPurchaseDto>> GetAllAsync(
            Guid locationId,
            GeneralStatus generalStatus,
            Guid? supplierId,
            Guid? salesPersonId,
            int? page =1,
            int? pageSize =20,
            CancellationToken ct = default)
            {

                // ── 2. Delivered qty per transaction item — Quantity is int ─────────────
                // var deliveredByItem =
                //     from d in _context.TransactionItemsDelivered.AsNoTracking()
                //     group d by d.TransactionItemId into g
                //     select new
                //     {
                //         TransactionItemId = g.Key,
                //         DeliveredQty      = g.Sum(x => (int?)x.Quantity) ?? 0,
                //     };

                // ── 3. Base query: filters only, no Include, no tracking ────────────────
                var baseQuery =
                    from p in _context.StockAdjustments.AsNoTracking()
                    join t in _context.Transactions.AsNoTracking()
                        on p.TransactionId equals t.Id
                    where t.LocationId == locationId
                    && (supplierId == null || p.SupplierId == supplierId)
                    && (int)t.GeneralStatus == (int)generalStatus
                    && (salesPersonId == null
                        || salesPersonId == Guid.Empty
                        || t.CreatedBy == salesPersonId
                        )
                    select new { Purchase = p, Transaction = t };

                // ── 4. Approved → only purchases with at least one partially delivered item
                // if (generalStatus == GeneralStatus.Approved)
                // {
                //     baseQuery = baseQuery.Where(x =>
                //         !_context.TransactionItems
                //             .Where(ti => ti.TransactionId == x.Transaction.Id)
                //             .All(ti =>  // ALL items must be fully delivered to exclude the purchase
                //                 (deliveredByItem
                //                     .Where(d => d.TransactionItemId == ti.Id)
                //                     .Select(d => d.DeliveredQty)
                //                     .FirstOrDefault()) >= ti.Quantity
                //             )
                //         || !_context.TransactionItems.Any(ti => ti.TransactionId == x.Transaction.Id)
                //     );
                // }

                // ── 5. Total count (before pagination) ──────────────────────────────────
                var total = await baseQuery.CountAsync(ct);

                if (total == 0)
                    return new PagedResult<GetPurchaseDto>(
                        Array.Empty<GetPurchaseDto>(), 0, page, pageSize);



                // 1. Project everything except Balance
                var items = await (
                    from x in baseQuery
                    join loc in _context.Locations.AsNoTracking()
                        on x.Transaction.LocationId equals loc.Id into locJoin
                    from loc in locJoin.DefaultIfEmpty()
                    join sup in _context.Suppliers.AsNoTracking()
                        on x.Purchase.SupplierId equals sup.Id into supJoin
                    from sup in supJoin.DefaultIfEmpty()
                    // join usr in _context.Users.AsNoTracking()
                    //     on x.Purchase.PurcasedById equals usr.Id into usrJoin
                    // from usr in usrJoin.DefaultIfEmpty()
                    orderby x.Transaction.CreatedAt descending
                    select new GetPurchaseDto
                    {
                        Id              = x.Purchase.Id,
                        TransactionId   = x.Purchase.TransactionId,
                        TransactionCode = x.Transaction.TransactionNumber ?? string.Empty,
                        LocationId      = x.Transaction.LocationId,
                        LocationName    = loc != null ? (loc.Name ?? string.Empty) : string.Empty,
                        SupplierId      = x.Purchase.SupplierId,
                        SupplierName    = sup != null
                                            ? ((sup.FirstName ?? "") + " " + (sup.LastName ?? "")).Trim()
                                            : string.Empty,
                        //TransactionBy   = usr != null ? (usr.FullName ?? string.Empty) : string.Empty,
                        CreatedAt       = x.Transaction.CreatedAt,
                        TransactionDate = x.Transaction.TransactionDate,
                        TotalAmount     = x.Transaction.TotalAmount,
                        // ✅ Reverted to correlated subquery — matches original working code
                        PaidAmount      = _context.TransactionPayments
                                            .Where(tp => tp.TransactionId == x.Transaction.Id)
                                            .Sum(tp => (decimal?)tp.Amount) ?? 0m,
                        Balance         = (x.Transaction.TotalAmount)
                                        - (_context.TransactionPayments
                                            .Where(tp => tp.TransactionId == x.Transaction.Id)
                                            .Sum(tp => (decimal?)tp.Amount) ?? 0m),
                    }
                )
                .Skip(((int)page - 1) * (int)pageSize)
                .Take((int)pageSize)
                .ToListAsync(ct);

                // 2. Compute Balance in memory — no EF, no NULL surprise
                foreach (var item in items)
                {
                    item.Balance = (item.TotalAmount ?? 0m) - item.PaidAmount;
                }

                return new PagedResult<GetPurchaseDto>(items, total, page, pageSize);
        }


        public async Task ManagerCheck(UpdateAdjustmentPurchaseDto updateDto)
        {
            // 1. Fix: missing closing paren + Enum.IsDefined(TYPE, value) is the correct overload
            var validStatus = Enum.IsDefined(typeof(GeneralStatus), updateDto.Status);
            if (!validStatus) throw new Exception("Invalid status submitted");

            var allowedStatus = new List<GeneralStatus> { GeneralStatus.Approved, GeneralStatus.Declined };

            if (allowedStatus.Contains(updateDto.Status) == false) throw new Exception("Invalid status submitted");

            // 2. Fix: include related entities if needed (Transaction for the comment)
            var adjustment = await _context.StockAdjustments
                .Include(x => x.Transaction)
                    .ThenInclude(x=> x.TransactionItems)
                        .ThenInclude(x=> x.Item)
                .Where(x => x.TransactionId == updateDto.TransactionId)
                .FirstOrDefaultAsync();

            if (adjustment == null) throw new Exception("Purchase not found");

            if (adjustment.Transaction.GeneralStatus != GeneralStatus.Active)
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
                    adjustment.TransactionId,
                    adjustment.Transaction.TransactionType.ToString(),
                    "1",
                    updateDto.Remarks,
                    DateTime.UtcNow,
                    user.Id,
                    user.FullName
                );
                _context.Comments.Add(comment);
            }

            // 4. Fix: user.Id is already a string GUID — no need to re-parse unless the property expects Guid
            adjustment.Transaction.UpdateGeneralStatus(updateDto.Status, Guid.Parse(user.Id), DateTime.UtcNow);

            if (updateDto.Status == GeneralStatus.Approved)
            {
                adjustment.Approve(user.Id, DateTime.UtcNow);


               var items = adjustment.Transaction.TransactionItems.Select(x =>
                 new ConfirmTransactionDeliveryDTOItems
                    {
                        Code = x.Item.Code,
                        Name = x.Item.Name,
                        ItemId = x.ItemId,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice
                    }).ToList();

                // var items = new List<ConfirmTransactionDeliveryDTOItems> ();
                // foreach(var transItem in adjustment.Transaction.TransactionItems)
                // {
                //     var newDeliveryItem =  new ConfirmTransactionDeliveryDTOItems
                //     {
                //         Code = transItem.Item.Code,
                //         Name = transItem.Item.Name,
                //         ItemId = transItem.ItemId,
                //         Quantity = transItem.Quantity,
                //         UnitPrice = transItem.UnitPrice
                //     };
                // }
                var confirmationDto = new ConfirmTransactionDeliveryDTO
                {
                  Date = DateTime.UtcNow,
                  Items =  items,
                  LocationId = adjustment.Transaction.LocationId,
                  TransactionId = adjustment.TransactionId,
                  Transportation = updateDto.Transportaion
                };
                //stock adjustment
               await _transactionService.DeliverItems(confirmationDto, user, null);
            }

            if (updateDto.Status == GeneralStatus.Declined)
            {
                adjustment.Decline(user.Id, DateTime.UtcNow);
            }
            _context.StockAdjustments.Update(adjustment);

            await _context.SaveChangesAsync();
        }
    }
}