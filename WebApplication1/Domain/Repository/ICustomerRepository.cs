using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface ICustomerRepository
    {
        IQueryable<Customer> GetAllAsync(Guid locationId);
        Task<Customer> GetByIdAsync(Guid id);
        Task<Customer> CreateAsync(Customer createDto);
        Task<Customer> UpdateAsync(Customer updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<string> GenerateCodeAsync(Guid locationId);
    }
}
