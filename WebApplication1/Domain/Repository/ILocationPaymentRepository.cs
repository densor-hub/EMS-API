// Repositories/ILocationPaymentRepository.cs
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Repositories
{
    public interface ILocationPaymentRepository
    {
        Task<EmployeeDisbursement> GetByIdAsync(Guid id);
        IQueryable<EmployeeDisbursement> GetAllAsync();
        Task AddAsync(EmployeeDisbursement locationPayment);
        Task Update(EmployeeDisbursement locationPayment);
        void Delete(EmployeeDisbursement locationPayment);
        Task<int> SaveChangesAsync();
    }
}