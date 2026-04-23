using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface ISaleRepository
    {
        Task<Sale> GetByIdAsync(Guid id);
        Task<IEnumerable<PurchaseDto>> GetAllAsync(Guid locationId, GeneralStatus generalStatus, Guid? customerId = null, Guid? salesPersonId = null);
        Task<string> CreateAsync(CreateTransactionDto createDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId, string reason, Sale? preLoadedSale = null);
        Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate);
        Task<SaleCancellationDto> CancellationAsync(CreateSaleCancellationDto createDto, Guid userId);
    }
}
