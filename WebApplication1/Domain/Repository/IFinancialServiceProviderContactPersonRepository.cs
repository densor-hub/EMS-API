using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Repository
{
    public interface IFinancialServiceProviderContactPersonRepository
    {
        Task<FinancialServiceProviderContactPerson> AddAsync(FinancialServiceProviderContactPerson BankContactPerson);
        Task<FinancialServiceProviderContactPerson?> GetByIdAsync(Guid id);
        Task<IEnumerable<FinancialServiceProviderContactPerson>> AddRangeAsync(List<FinancialServiceProviderContactPerson> contactPersons);
        Task<IEnumerable<FinancialServiceProviderContactPerson>> UpdateRageAsync(List<FinancialServiceProviderContactPerson> contactPersons);
        Task<IQueryable<FinancialServiceProviderContactPerson>> GetByAllByBankIdAsync(Guid bankId, string? filter, GeneralStatus? status =null);
        Task DeleteAsync (Guid contactPersonId);
        Task<FinancialServiceProviderContactPerson?> GetByCodeAsync(string Code);
    }
}
