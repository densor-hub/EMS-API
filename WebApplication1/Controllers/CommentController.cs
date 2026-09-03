// Controllers/CommentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Domain.DTO;
using WebApplication1.Domain.Enums;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentController> _logger;

        public CommentController(ICommentService commentService, ILogger<CommentController> logger)
        {
            _commentService = commentService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetAllComments()
        {
            try
            {
                var result = await _commentService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all comments");
                return StatusCode(500, new { message = "An error occurred while retrieving comments" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CommentResponseDto>> GetCommentById(Guid id)
        {
            try
            {
                var result = await _commentService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comment with ID {Id}", id);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("transaction/{transactionId}")]
        public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetCommentsByTransactionId(Guid transactionId)
        {
            try
            {
                var result = await _commentService.GetByTransactionIdAsync(transactionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments by transaction ID {TransactionId}", transactionId);
                return StatusCode(500, new { message = "An error occurred while retrieving comments" });
            }
        }

        [HttpGet("type/{transactionType}")]
        public async Task<ActionResult<IEnumerable<CommentResponseDto>>> GetCommentsByTransactionType(TransactionType transactionType)
        {
            try
            {
                var result = await _commentService.GetByTransactionTypeAsync(transactionType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments by transaction type {TransactionType}", transactionType);
                return StatusCode(500, new { message = "An error occurred while retrieving comments" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CommentResponseDto>> CreateComment([FromBody] CreateCommentDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _commentService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetCommentById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating comment");
                return StatusCode(500, new { message = "An error occurred while creating the comment" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CommentResponseDto>> UpdateComment(Guid id, [FromBody] UpdateCommentDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _commentService.UpdateAsync(id, updateDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment with ID {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComment(Guid id)
        {
            try
            {
                await _commentService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment with ID {Id}", id);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}