// Services/IPurchaseService.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;

namespace WebApplication1.Services.ControllerServices
{
    public interface IPurchaseService
    {
        Task<GetPurchaseDto> GetByIdAsync(Guid id, GeneralStatus generalStatus);
       Task<PagedResult<GetPurchaseDto>> GetAllAsync(
            Guid locationId,
            GeneralStatus generalStatus,
            Guid? supplierId,
            Guid? salesPersonId,
            int? page =1,
            int? pageSize =20,
            CancellationToken ct = default
        );
         Task ManagerCheck(UpdatePurchaseDto updateDto);
       // Task<bool> DeleteAsync(Guid id, Guid userId, string reason);
        //Task<PurchaseCancellationDto> CancelAsync(CreatePurchaseCancellationDto createDto, Guid userId);
    }
}