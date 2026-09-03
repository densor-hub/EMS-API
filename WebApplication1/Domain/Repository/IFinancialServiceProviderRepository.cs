// Repositories/IBankRepository.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Repositories
{
    public interface IFinancialServiceProviderRepository 
    {
        IQueryable<FinancialServiceProvider> GetFinancialServiceProvidersByLocationAsync(Guid locationId, string? filter, GeneralStatus? status = null);
        Task<FinancialServiceProvider> AddAsync (FinancialServiceProvider bank);
        Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null);
        Task <FinancialServiceProvider?> GetByIdAsync(Guid id);
        Task <FinancialServiceProvider> Update(FinancialServiceProvider bank);
    }

   
}