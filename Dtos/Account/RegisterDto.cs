using System.ComponentModel.DataAnnotations;

namespace coreC_.Dtos.Account
{
    public class RegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]

        // không cần validate ở đây vì đã cấu hình trong program.cs
        public string Password { get; set; }
    }
}
