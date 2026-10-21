using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Repository;

namespace WebApplication1.Services.ControllerServices
{
    public interface IItemSplitService
    {

        Task<Guid> CreateAsync(
            CreateItemSplitSetupRequest request,
            CancellationToken ct = default);

        Task<IReadOnlyList<ItemSplitSetupDto>> GetAllAsync(
            Guid locationId,
            CancellationToken ct = default);

        Task<ItemSplitSetupDto?> GetByIdAsync(
            Guid id,
            Guid locationId, 
            CancellationToken ct = default);

        Task<bool> UpdateAsync(
            Guid id,
            UpdateItemSplitSetupRequest request,
            CancellationToken ct = default);

        Task<bool> DeleteAsync(
            Guid id,
            CancellationToken ct = default);

        Task<Guid> CreateTransactionAsync(
            CreateItemSplitTransactionRequest request,
            CancellationToken ct = default);

        Task<IReadOnlyList<ItemSplitTransactionDto>> GetAllTransactionsAsync(
            CancellationToken ct = default);

        Task<ItemSplitTransactionDto?> GetTransactionByIdAsync(
           Guid id,
           CancellationToken ct = default);

    }
}