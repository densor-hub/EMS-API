using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Domain.DTO
{
    public class RefreshTokenRequest
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}