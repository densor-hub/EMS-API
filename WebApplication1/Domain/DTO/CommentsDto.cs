// DTOs/CommentDtos.cs
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.DTO
{
    public class CreateCommentDto
    {
        public Guid TransactionId { get; set; }
        public int Stage { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Comment { get; set; }
    }

    public class UpdateCommentDto
    {
        public string Comment { get; set; }
        public int? Stage { get; set; }
    }

    public class CommentResponseDto
    {
        public Guid Id { get; set; }
        public string Stage { get; set; }
        public string TransactionType { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
    }
}