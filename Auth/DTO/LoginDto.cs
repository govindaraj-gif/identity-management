using System.ComponentModel.DataAnnotations;

namespace Auth.DTO
{
    public record LoginDto
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
