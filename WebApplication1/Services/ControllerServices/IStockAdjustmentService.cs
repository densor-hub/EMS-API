using WebApplication1.Domain.DTO;

using WebApplication1.Controllers;
using WebApplication1.DTOs;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Services.ControllerServices
{
    public interface IStockAdjustmentService
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
         Task ManagerCheck(UpdateAdjustmentPurchaseDto updateDto);
    }
}
