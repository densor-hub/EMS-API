// Services/IStockTransferService.cs
using WebApplication1.Controllers;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.QueryFilters;
using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public interface IStockTransferService
    {
        Task CreateStockTransferAsync(CreateTransactionDto dto);
       Task ApproveStockTransferAsync(StockTransferApproveDto dto);
        Task ReceiveStockTransferAsync(StockTransferReceiveDto dto);
        
        Task<StockTransfer> GetStockTransferAsync(Guid id);
        Task<IEnumerable<StockTransfersDTO>> GetStockTransferRecordsAsync(BrowseStockTransfersFilters filters);
    }
}