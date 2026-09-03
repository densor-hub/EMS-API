using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAL;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repositories
{
    public class DisbursementExternalResponseRepository : IDisbursementExternalResponseRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<TransDisbursementExternalResponse> _dbSet;

        public DisbursementExternalResponseRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TransDisbursementExternalResponse>();
        }

        public async Task<TransDisbursementExternalResponse?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(r => r.Transaction)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<TransDisbursementExternalResponse?> GetByDisbursementIdAsync(Guid transactionId)
        {
            return await _dbSet
                .Include(r => r.Transaction)
                .FirstOrDefaultAsync(r => r.TransactionId == transactionId);
        }

        public  IQueryable<TransDisbursementExternalResponse> GetAllAsync() { 
            return  _dbSet
                .Include(r => r.Transaction)
                .OrderByDescending(r => r.RespondedAt).AsNoTracking()
                ;
        }

        public  IQueryable<TransDisbursementExternalResponse> GetByPersonCodeAsync(string personCode)
        {
            return  _dbSet
                .Include(r => r.Transaction)
                .Where(r => r.PersonCode == personCode)
                .OrderByDescending(r => r.RespondedAt);
        }

     

        public   IQueryable<TransDisbursementExternalResponse> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var data = _dbSet
                .Include(r => r.Transaction)
                .Where(r => r.RespondedAt >= startDate && r.RespondedAt <= endDate)
                .OrderByDescending(r => r.RespondedAt);

            return data;
        }

        public async Task AddAsync(TransDisbursementExternalResponse entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task UpdateAsync(TransDisbursementExternalResponse entity)
        {
            _dbSet.Update(entity);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbSet.AnyAsync(r => r.Id == id);
        }

     
    }
}