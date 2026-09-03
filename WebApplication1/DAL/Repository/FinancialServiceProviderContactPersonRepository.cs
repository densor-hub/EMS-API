using Microsoft.EntityFrameworkCore;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;

namespace WebApplication1.DAL.Repository
{
    public class FinancialServiceProviderContactPersonRepository : IFinancialServiceProviderContactPersonRepository
    {
        private readonly AppDbContext _appDbContext;
        public FinancialServiceProviderContactPersonRepository(AppDbContext appDbContext)
        {

            _appDbContext = appDbContext;

        }
        public async Task<FinancialServiceProviderContactPerson> AddAsync(FinancialServiceProviderContactPerson BankContactPerson)
        {
            await _appDbContext.FinancialServiceProviderContactPersons.AddAsync(BankContactPerson);

            return BankContactPerson;
        }

        public async Task<IEnumerable<FinancialServiceProviderContactPerson>> AddRangeAsync(List<FinancialServiceProviderContactPerson> contactPersons)
        {
            await _appDbContext.FinancialServiceProviderContactPersons.AddRangeAsync(contactPersons);

            return contactPersons;
        }

        public async Task DeleteAsync(Guid contactPersonId)
        {
            var toBeDeleted = _appDbContext.FinancialServiceProviderContactPersons.Where(x => x.Id == contactPersonId).FirstOrDefault();
            if (toBeDeleted != null) { throw new Exception(" Person not found"); }

         
             _appDbContext.Remove(toBeDeleted);

            await Task.CompletedTask;
        }

      
        public Task<FinancialServiceProviderContactPerson?> GetByIdAsync(Guid id)
        {
            return _appDbContext.FinancialServiceProviderContactPersons.Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<FinancialServiceProviderContactPerson?> GetByCodeAsync(string Code)
        {
            return await _appDbContext.FinancialServiceProviderContactPersons.Where(x => x.Code.ToUpper().Trim() == Code.ToUpper().Trim()).FirstOrDefaultAsync();
        }
        public async Task<IQueryable<FinancialServiceProviderContactPerson>> GetByAllByBankIdAsync(Guid bankId, string? filter, GeneralStatus? status = null)
        {
            var data =  _appDbContext.FinancialServiceProviderContactPersons.Where(x => x.Id == bankId).AsNoTracking();

            if (!string.IsNullOrEmpty(filter))
            {
                data = data.Where(x=> x.FullName.ToLower().Trim().Contains(filter.ToLower().Trim()));
            }

            if (status != null)
            {
                data = data.Where(x => x.GeneralStatus == status);
            }

            return data;
        }

        public async Task<IEnumerable<FinancialServiceProviderContactPerson>> UpdateRageAsync(List<FinancialServiceProviderContactPerson> contactPersons)
        {
             _appDbContext.FinancialServiceProviderContactPersons.UpdateRange(contactPersons);

            await Task.CompletedTask;
            return contactPersons;
        }


       

    }
}
