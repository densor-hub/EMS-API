using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.DTOs;

namespace WebApplication1.Services.ControllerServices
{
    public interface ISaleService
    {
        Task<Sale> GetByIdAsync(Guid id);
        Task<IEnumerable<GetSalesTrans>> GetAllAsync(Guid locationId, GeneralStatus generalStatus, string type, Guid? customerId = null, Guid? salesPersonId = null);
        Task<decimal> GetTotalSalesAmountAsync(DateTime startDate, DateTime endDate);
        Task<GetSalesReceiptDto> GenerateReceipt(Guid? saleTransDeliveryRequestId, string transNumber);
    }
}
