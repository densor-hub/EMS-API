// Repositories/ICommentRepository.cs
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Repositories
{
    public interface ICurrencyRepository
    {
        Task<Currency?> GetByIdAsync(Guid id);
        Task<Currency?> GetByCurrencyCodeAsync(string id);
        IQueryable<Currency>  GetAllAsync();
        Task AddAsync(Currency comment);
        Task Update(Currency comment);
        void Delete(Currency comment);
        Task<int> SaveChangesAsync();
    }
}