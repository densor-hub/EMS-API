// Services/Implementations/CommentService.cs
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Entities;
using WebApplication1.Domain.Enums;
using WebApplication1.Domain.Repository;
using WebApplication1.Repositories;

namespace WebApplication1.Services.ControllerServices.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CommentService> _logger;

        public CommentService(
            ICommentRepository commentRepository,
            IUserRepository userRepository,
            ILogger<CommentService> logger)
        {
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<CommentResponseDto> CreateAsync(CreateCommentDto createDto)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    throw new Exception("User not found");

                var comment = TransactionComment.Create(
                    Guid.NewGuid(),
                    createDto.TransactionId,
                    createDto.TransactionType.ToString(),
                    createDto.Stage.ToString(),
                    createDto.Comment,
                    DateTime.UtcNow,
                    user.Id,
                    user.FullName
                );

                await _commentRepository.AddAsync(comment);
                await _commentRepository.SaveChangesAsync();

                return MapToResponseDto(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                throw;
            }
        }

        public async Task<CommentResponseDto> GetByIdAsync(Guid id)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(id);
                if (comment == null)
                    throw new Exception("Comment not found");

                return MapToResponseDto(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment with ID {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<CommentResponseDto>> GetAllAsync()
        {
            try
            {
                var comments = await _commentRepository.GetAllAsync();
                return comments.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all comments");
                throw;
            }
        }

        public async Task<IEnumerable<CommentResponseDto>> GetByTransactionIdAsync(Guid transactionId)
        {
            try
            {
                var comments = await _commentRepository.GetByTransactionIdAsync(transactionId);
                return comments.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments by transaction ID {TransactionId}", transactionId);
                throw;
            }
        }

        public async Task<IEnumerable<CommentResponseDto>> GetByTransactionTypeAsync(TransactionType transactionType)
        {
            try
            {
                var comments = await _commentRepository.GetByTransactionTypeAsync(transactionType);
                return comments.Select(MapToResponseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments by transaction type {TransactionType}", transactionType);
                throw;
            }
        }

        public async Task<CommentResponseDto> UpdateAsync(Guid id, UpdateCommentDto updateDto)
        {
            try
            {
                var user = await _userRepository.GetUserByRefreshTokenAsync();
                if (user == null)
                    throw new Exception("User not found");

                var comment = await _commentRepository.GetByIdAsync(id);
                if (comment == null)
                    throw new Exception("Comment not found");

                // Use reflection or add update method to entity
                var propertyInfo = comment.GetType().GetProperty("Comment");
                if (propertyInfo != null && !string.IsNullOrEmpty(updateDto.Comment))
                    propertyInfo.SetValue(comment, updateDto.Comment);

                if (updateDto.Stage.HasValue)
                {
                    propertyInfo = comment.GetType().GetProperty("Stage");
                    propertyInfo?.SetValue(comment, updateDto.Stage.Value);
                }

                await _commentRepository.Update(comment);
                await _commentRepository.SaveChangesAsync();

                return MapToResponseDto(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment with ID {Id}", id);
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var comment = await _commentRepository.GetByIdAsync(id);
                if (comment == null)
                    throw new Exception("Comment not found");

                _commentRepository.Delete(comment);
                await _commentRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID {Id}", id);
                throw;
            }
        }

        private CommentResponseDto MapToResponseDto(TransactionComment comment)
        {
            return new CommentResponseDto
            {
                Id = comment.Id,
                Stage = comment.Stage,
                TransactionType = comment.TransactionType,
                Comment = comment.Comment,
                CreatedAt = comment.CreatedAt,
                CreatedBy = comment.CreatedByName
            };
        }
    }
}