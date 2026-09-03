// Services/ICommentService.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Services
{
    public interface ICommentService
    {
        Task<CommentResponseDto> CreateAsync(CreateCommentDto createDto);
        Task<CommentResponseDto> GetByIdAsync(Guid id);
        Task<IEnumerable<CommentResponseDto>> GetAllAsync();
        Task<IEnumerable<CommentResponseDto>> GetByTransactionIdAsync(Guid transactionId);
        Task<IEnumerable<CommentResponseDto>> GetByTransactionTypeAsync(TransactionType transactionType);
        Task<CommentResponseDto> UpdateAsync(Guid id, UpdateCommentDto updateDto);
        Task DeleteAsync(Guid id);
    }
}