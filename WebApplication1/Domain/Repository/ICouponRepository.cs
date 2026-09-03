using WebApplication1.Domain.Entities;
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface ICouponRepository
    {
        IQueryable<Coupon> GetAllAsync(Guid locationId);
        Task<Coupon?> GetByCodeAsync(string code, Guid locationId);
        Task<Coupon?> GetByIdAsync(Guid Id);
        Task<Coupon> CreateAsync(Coupon createDto);
        Task CreateRangeAsync(List<Coupon> createDto);
        Task<Coupon> UpdateAsync(Coupon updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<string> GenerateCodeAsync();
        Task <bool> ValidateCoupon(string CouponCode, CreateTransactionDto createDto);

        Task SaveChangesAsync();

    }
}
