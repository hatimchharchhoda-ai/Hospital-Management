using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class UpdatePatientDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string Mobile { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string Password { get; set; } = null!;
    }
}
