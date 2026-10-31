using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication1.Domain.Enums;

namespace WebApplication1.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        // Optional: which device/browser requested it — helps revocation-per-device
        [MaxLength(256)]
        public string? DeviceInfo { get; set; }

        [MaxLength(64)]
        public string? CreatedByIp { get; set; }

        [NotMapped]
        public bool IsActive => RevokedAt == null && DateTime.UtcNow < ExpiresAt;

        public static RefreshToken Create(string token, string userId, DateTime expiresAt, string? deviceInfo = null, string? ip = null)
            => new()
            {
                Id         = Guid.NewGuid(),
                Token      = token,
                UserId     = userId,
                ExpiresAt  = expiresAt,
                CreatedAt  = DateTime.UtcNow,
                DeviceInfo = deviceInfo,
                CreatedByIp = ip
            };
    }
}