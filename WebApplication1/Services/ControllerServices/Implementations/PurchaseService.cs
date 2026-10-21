// Services/Implementations/PurchaseService.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;
using WebApplication1.Domain.Repository;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;


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

        //public async Task<IEnumerable<GetPurchaseDto>> GetAllAsync(Guid locationId, GeneralStatus generalStatus, Guid? supplierId = null, Guid? salesPersonId = null)
        //{
        //    var query = from purchase in _context.Purchases
        //                .Include(p => p.Supplier)
        //                .Include(p => p.PurcasedBy)
        //                .Include(x => x.Transaction)
        //                        .ThenInclude(t => t.TransactionItems)
        //                            .ThenInclude(ti => ti.TransactionItemsDelivered)
        //                .Where(x => x.Transaction.LocationId == locationId
        //                         && x.Transaction.GeneralStatus == generalStatus
        //                        && (supplierId != null ? x.SupplierId == supplierId : true)
        //                       && (int)x.GeneralStatus == (int)generalStatus
        //                && (salesPersonId != null && salesPersonId != Guid.Empty ? x.PurcasedById == salesPersonId.ToString() : true)
        //                )
        //                .AsNoTracking()

        //                select new GetPurchaseDto
        //                {
        //                    Id = purchase.Id,
        //                    TransactionId = purchase.TransactionId,
        //                    TransactionCode = purchase.Transaction.TransactionNumber,
        //                    LocationName = purchase.Transaction.Location != null ? purchase.Transaction.Location.Name : "",
        //                    SupplierId = purchase.SupplierId,
        //                    SupplierName = purchase.Supplier != null
        //                        ? purchase.Supplier.FirstName + " " + purchase.Supplier.LastName
        //                        : "",
        //                    TransactionBy = purchase.PurcasedBy != null ? purchase.PurcasedBy.FullName : "",
        //                    CreatedAt = purchase.Transaction.CreatedAt,
        //                    TransactionDate = purchase.Transaction != null ? purchase.Transaction.TransactionDate : DateTime.MinValue,
        //                    TotalAmount = purchase.Transaction != null ? purchase.Transaction.TotalAmount : 0,
        //                    PaidAmount = purchase.Transaction.TransactionPayments
        //                        .Where(tp => tp.TransactionId == purchase.TransactionId)
        //                        .Sum(tp => tp.Amount)
        //                };

        //    return await query.ToListAsync();
        //}

// ─────────────────────────────────────────────────────────────────────────────
// GetPurchasesQuery (or inline in your service)
// Signature suggestion:
//   Task<PagedResult<GetPurchaseDto>> GetPurchasesAsync(
//       Guid locationId,
//       Guid? supplierId,
//       GeneralStatus generalStatus,
//       Guid? salesPersonId,
//       int page,
//       int pageSize,
//       CancellationToken ct = default)
// ─────────────────────────────────────────────────────────────────────────────

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
    var deliveredByItem =
        from d in _context.TransactionItemsDelivered.AsNoTracking()
        group d by d.TransactionItemId into g
        select new
        {
            TransactionItemId = g.Key,
            DeliveredQty      = g.Sum(x => (int?)x.Quantity) ?? 0,
        };

    // ── 3. Base query: filters only, no Include, no tracking ────────────────
    var baseQuery =
        from p in _context.Purchases.AsNoTracking()
        join t in _context.Transactions.AsNoTracking()
            on p.TransactionId equals t.Id
        where t.LocationId == locationId
           && (supplierId == null || p.SupplierId == supplierId)
           && (int)p.GeneralStatus == (int)generalStatus
           && (salesPersonId == null
               || salesPersonId == Guid.Empty
               || p.PurcasedById == salesPersonId.ToString())
        select new { Purchase = p, Transaction = t };

    // ── 4. Approved → only purchases with at least one partially delivered item
    if (generalStatus == GeneralStatus.Approved)
    {
        baseQuery = baseQuery.Where(x =>
    !_context.TransactionItems
        .Where(ti => ti.TransactionId == x.Transaction.Id)
        .All(ti =>  // ALL items must be fully delivered to exclude the purchase
            (deliveredByItem
                .Where(d => d.TransactionItemId == ti.Id)
                .Select(d => d.DeliveredQty)
                .FirstOrDefault()) >= ti.Quantity
        )
    || !_context.TransactionItems.Any(ti => ti.TransactionId == x.Transaction.Id)
);
    }

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
    join usr in _context.Users.AsNoTracking()
        on x.Purchase.PurcasedById equals usr.Id into usrJoin
    from usr in usrJoin.DefaultIfEmpty()
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
        TransactionBy   = usr != null ? (usr.FullName ?? string.Empty) : string.Empty,
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