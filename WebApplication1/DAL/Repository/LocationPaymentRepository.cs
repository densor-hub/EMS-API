// Repositories/LocationPaymentRepository.cs
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Repositories
{
    public class LocationPaymentRepository : ILocationPaymentRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<EmployeeDisbursement> _dbSet;

        public LocationPaymentRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<EmployeeDisbursement>();
        }

        public async Task<EmployeeDisbursement> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(x => x.Location)
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<EmployeeDisbursement> GetAllAsync()
        {
            return  _dbSet
                .Include(x => x.Location)
                .Include(x => x.Employee)
                .OrderByDescending(x => x.Id);
        }

        public async Task AddAsync(EmployeeDisbursement locationPayment)
        {
            await _dbSet.AddAsync(locationPayment);
        }

        public async Task Update(EmployeeDisbursement locationPayment)
        {
            _dbSet.Update(locationPayment);
            await Task.CompletedTask;
        }

        public void Delete(EmployeeDisbursement locationPayment)
        {
            _dbSet.Remove(locationPayment);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}