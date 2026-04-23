// Services/Interfaces/IPurchaseService.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;

namespace WebApplication1.Domain.Repository
{
    public interface IPurchaseRepository
    {
        Task<PurchaseDto> GetByIdAsync(Guid id, GeneralStatus generalStatus);
        Task<IEnumerable<PurchaseDto>> GetAllAsync(Guid locationId, GeneralStatus GeneralStatus, Guid? SuplierId= null, Guid? salesPersonId = null);
        Task<Guid> CreateAsync(CreateTransactionDto createDto, Guid userId);
        
        Task<PurchaseDto> UpdateAsync(Guid id, UpdatePurchaseDto updateDto, Guid userId);
        Task<bool> DeleteAsync(Guid id, Guid userId, string reason);
        Task<PurchaseCancellationDto> CancelAsync(CreatePurchaseCancellationDto createDto, Guid userId);
    }
}