// Repositories/BankRepository.cs
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Repositories
{
    public class FinancialServiceProviderRepository : IFinancialServiceProviderRepository
    {
        private readonly AppDbContext _context;
        public FinancialServiceProviderRepository(AppDbContext context)
        {
            _context = context;
        }

        public  async Task<FinancialServiceProvider> AddAsync(FinancialServiceProvider dto)
        {
           await  _context.FinancialServiceProviders.AddAsync(dto);
            return dto;
        }

        public  IQueryable <FinancialServiceProvider> GetFinancialServiceProvidersByLocationAsync(Guid locationId, string? filter, GeneralStatus? status = null)
        {
            var FinancialServiceProviders =  _context.FinancialServiceProviders
                .Include(b => b.Location)
                .Where(b => b.LocationId == locationId &&
                (!string.IsNullOrEmpty(filter) ? b.Name.ToLower().Trim().Contains(filter.ToLower().Trim()) || b.Code.ToLower().Trim().Contains(filter.ToLower().Trim()) : true))
            .AsNoTracking();

            if (status != null)
            {
                FinancialServiceProviders = FinancialServiceProviders.Where(x=> x.GeneralStatus == status);
            }

            return FinancialServiceProviders;
        }

        public async Task<FinancialServiceProvider?> GetByIdAsync(Guid id)
        {
            return await _context.FinancialServiceProviders
                .Include(x=> x.Location)
                .Include(x=> x.ContactPersons)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }


        public async Task<bool> IsCodeUniqueAsync(string code, Guid? excludeId = null)
        {
            var query = _context.FinancialServiceProviders.Where(b => b.Code == code);

            if (excludeId.HasValue)
            {
                query = query.Where(b => b.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<FinancialServiceProvider> Update(FinancialServiceProvider bank)
        {
            _context.FinancialServiceProviders.Update(bank);

            return bank;
        }
    }
}