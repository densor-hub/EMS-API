using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Domain.DTO
{
    public class LoginRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
