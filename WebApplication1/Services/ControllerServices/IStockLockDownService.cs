using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Services
{
    public interface IStockLockDownService
    {

        Task<StockLockDownRequest> CreateStockLockDownWithItemsAsync(CreateStockLockDownRequestDto createDto);
        Task<IEnumerable<GetAllStockLockDownDto>> GetStockLockDownRequestsByLocationAsync(GetStockLockDownQuery query);
        Task<GetStockLockDownByIdDto> GetStockLockDownRequestsByIdAsync(Guid id);
        Task<StockTakeItemSubmission> SubmitStockTakeItem(CreateStockSubmissionDto createDto);
        Task<IEnumerable<GetStockLockDownSubmittedItemDto>> GetSubmittedStockLockedItems(GetStockLockDownQuery query);
        Task<StockTakeItemSubmission> UpdateStockTakeItem(UpdateStockTakeSubmissionDto createDto);
        Task<IEnumerable<StockTakeItemSubmission>> GetSubmissionsByStockLockDownItemAsync(Guid stockLockDownItemId);
        IQueryable<StockLockDownItem> GetItemsWithPendingSubmissionsAsync(List<Guid> itemIds);
        Task<IEnumerable<CommentResponseDto>> GetComments(Guid StockLockDownRequestId);



    }
}