// Services/IPurchaseService.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;

namespace WebApplication1.Services.ControllerServices
{
    public interface IPurchaseService
    {
        Task<GetPurchaseDto> GetByIdAsync(Guid id, GeneralStatus generalStatus);
        Task<IEnumerable<GetPurchaseDto>> GetAllAsync(Guid locationId, GeneralStatus generalStatus, Guid? supplierId = null, Guid? salesPersonId = null);
       // Task<GetPurchaseDto> UpdateAsync(Guid id, UpdatePurchaseDto updateDto, Guid userId);
       // Task<bool> DeleteAsync(Guid id, Guid userId, string reason);
        //Task<PurchaseCancellationDto> CancelAsync(CreatePurchaseCancellationDto createDto, Guid userId);
    }
}