using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Domain.Entities;

namespace WebApplication1.Domain.Repositories
{
    public interface IDisbursementExternalResponseRepository
    {
        Task<TransDisbursementExternalResponse?> GetByIdAsync(Guid id);
        Task<TransDisbursementExternalResponse?> GetByDisbursementIdAsync(Guid disbursementId);
        IQueryable<TransDisbursementExternalResponse> GetAllAsync();
        IQueryable<TransDisbursementExternalResponse>GetByPersonCodeAsync(string personCode);
        IQueryable<TransDisbursementExternalResponse> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task AddAsync(TransDisbursementExternalResponse entity);
        Task UpdateAsync(TransDisbursementExternalResponse entity);
        Task DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
       
}