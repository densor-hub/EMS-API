using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repository
{
    public interface IFinancialDepositRepository
    {
        Task<IEnumerable<FinancialServiceDisbursement>> GetDepositsAsync(Guid locationId, Guid? bankId);
        Task<FinancialServiceDisbursement> AddAsync(FinancialServiceDisbursement deposit);
    }
}
