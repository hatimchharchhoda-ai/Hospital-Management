using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class LoginDoctorDto
    {
        [Required]
        public string Identifier { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}