using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;
using WebApplication1.DTOs;

namespace WebApplication1.DAL.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly AppDbContext _context;
        public CouponRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Coupon> CreateAsync(Coupon createDto)
        {
            await _context.Coupons.AddAsync(createDto);
            //await _context.SaveChangesAsync();

            return createDto;
        }

        public async Task CreateRangeAsync(List<Coupon> createDto)
        {
            await _context.Coupons.AddRangeAsync(createDto);
           // await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x=> x.Id == id);
            if (coupon == null) throw new InvalidOperationException("Coupon not found");
            _context.Remove(coupon);
            return _context.ChangeTracker.HasChanges();
           
        }

       
        public async Task<string> GenerateCodeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            const int maxAttempts = 100;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var firstPart = new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var secondPart = new string(Enumerable.Repeat(chars, 5)
                    .Select(s => s[random.Next(s.Length)]).ToArray());

                var generatedCode = $"{firstPart}-{secondPart}";

                var exists = await _context.Coupons
                    .AnyAsync(l => l.Code.ToLower().Trim() == generatedCode.ToLower().Trim());

                if (!exists)
                {
                    return generatedCode;
                }
            }

            throw new InvalidOperationException("Unable to generate unique transaction number after 100 attempts");
        }

        public IQueryable<Coupon> GetAllAsync(Guid locationId)
        {
            return _context.Coupons.Where(x=> x.LocationId == locationId);
        }

        public async Task<Coupon?> GetByCodeAsync(string code, Guid locationId)
        {
            return await _context.Coupons.Where(x => x.LocationId == locationId
                    && x.Code.ToLower().Trim() == code.ToLower().Trim()).FirstOrDefaultAsync();
        }

        public Task<Coupon?> GetByIdAsync(Guid Id)
        {
            return _context.Coupons.Where(x => x.Id == Id).FirstOrDefaultAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Coupon> UpdateAsync(Coupon updateDto)
        {
            _context.Coupons.Update(updateDto);
            return updateDto;
        }

        public async Task<bool> ValidateCoupon(string CouponCode, CreateTransactionDto createDto)
        {

            if (!string.IsNullOrEmpty(createDto.CouponCode))
            {
                // 1. Get location with company in a single query
                var location = await _context.Locations
                    .Where(x => x.Id == createDto.LocationId)
                    .Select(x => new { x.Id, x.CompanyId })
                    .FirstOrDefaultAsync();

                if (location == null)
                    return false;

                // 2. Validate coupon in a single optimized query
                var couponAmount = await _context.Coupons
                    .Where(x => x.Code.ToUpper().Trim() == createDto.CouponCode.ToUpper()
                        && x.Location.CompanyId == location.CompanyId)
                    .Select(x => x.Amount)
                    .FirstOrDefaultAsync();

                //if (couponAmount == 0)
                //    return false;

                // 3. Calculate total in one efficient query using a join
                var soldItemIds = createDto.Items.Select(x => x.ItemId).ToList();

                var totalSumSold = await _context.Items
                    .Where(x => soldItemIds.Contains(x.Id))
                    .Select(x => new { x.Id, x.SellingPrice })
                    .ToListAsync()
                    .ContinueWith(task =>
                    {
                        var itemPrices = task.Result.ToDictionary(x => x.Id, x => x.SellingPrice);
                        return createDto.Items.Sum(item => itemPrices.GetValueOrDefault(item.ItemId, 0) * item.Quantity);
                    });

                // 4. Validate payment
                if (couponAmount + createDto.AmountPaid < totalSumSold) return false;
                
                
                return true;
            } else
            {
                return false;
            }

        }
    }
}
