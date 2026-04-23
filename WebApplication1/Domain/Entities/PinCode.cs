using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Domain.Entities
{
    public class PinCode
    {
        public Guid Id { get; private set; }
        [Required]
        [StringLength(9)]
        public string Code { get; private set; }
        public DateTime? ExpiryDate { get; private set; }
        [Required]
        public bool IsUsed { get; private set; } = false;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

        public PinCode()
        {

        }

        private PinCode(Guid id, string code, DateTime expiryDate, DateTime createdAt)
        {
            Id = id;
            Code = code;
            ExpiryDate = expiryDate;
            CreatedAt = createdAt;
        }

        private PinCode(Guid id, string code, DateTime createdAt)
        {
            Id = id;
            Code = code;
            CreatedAt = createdAt;
        }

        public static PinCode CreateVisitCode(Guid id, string code, DateTime expiryDate, DateTime createdAt)
            => new PinCode(id, code, expiryDate, createdAt);

        public static PinCode CreateCardCode(Guid id, string code, DateTime createdAt)
            => new PinCode(id, code, createdAt);
    }
}
