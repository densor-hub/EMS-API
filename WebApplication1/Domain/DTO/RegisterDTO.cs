using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Domain.DTO
{
    public class RegisterDTO
    {
        
        [Required]
        public  AdminDTOUser AdminInfo { get; set; }
        [Required]
        public CompanyResgisteringDTO Company { get; set; }
       
     }

    public class CompanyResgisteringDTO
    {
        [Required]
        public string Name { get; set; } 
        [Required]
        public string Email { get; set; }
        public string? Address { get; set; }
        [Required]
        public string Phone { get; set; }
        public string? Website { get; set; }
        public string? TIN { get; set; }
        public string? RegistrationNumber { get; set; }
    }

    public class AdminDTOUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
