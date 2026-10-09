using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class FinancialDepositRepository : IFinancialDepositRepository
    {

        private readonly AppDbContext _appDbContext;
        public FinancialDepositRepository(AppDbContext appDbContext)
        {

            _appDbContext = appDbContext;

        }
        public async Task<FinancialServiceDisbursement> AddAsync(FinancialServiceDisbursement deposit)
        {
            await _appDbContext.FinancialServiceDisbursement.AddAsync(deposit);
            await _appDbContext.SaveChangesAsync();

            return deposit;
        }

        public async Task<IEnumerable<FinancialServiceDisbursement>> GetDepositsAsync(Guid locationId, Guid? financialServiceProviderId)
        {
            var data = _appDbContext.FinancialServiceDisbursement
                    .Include(x=> x.Transaction)
                        .ThenInclude(x=>x.Location)
                    .Include(x=> x.FinancialServiceDisbursementContactPersons)
                        .ThenInclude(x=> x.ContactPerson)
                            .ThenInclude(x=> x.FinancialServiceProvider)
                   
                    .Where(x => x.Transaction.LocationId  == locationId).AsNoTracking();

            if (financialServiceProviderId.HasValue && financialServiceProviderId != Guid.Empty)
            {
                data = data.Where(x => x.FinancialServiceProviderId == financialServiceProviderId);
            }

            return data;
        }

       
    }
}
