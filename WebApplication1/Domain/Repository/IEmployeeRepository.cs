using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface IEmployeeRepository
    {
        IQueryable<Employee> GetAll();
        IQueryable<Employee> GetAllByCompanyId(Guid companyId);
        IQueryable<Employee> GetAllByLocationId(Guid locationId);
        Task<Employee> GetByIdAsync(Guid id);
        Task<Employee?> GetByIdAndLocationAsync(Guid id, Guid locationId);
        Task<Employee> CreateAsync(Employee createDto);
        Task<Employee> UpdateAsync(Employee updateDto);
        Task<bool> DeleteAsync(Guid id);
        Task<string> GenerateCodeAsync(Guid locationId);
    }
}
