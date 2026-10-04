using System.ComponentModel.DataAnnotations;

namespace CarAutomotive.Core.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
