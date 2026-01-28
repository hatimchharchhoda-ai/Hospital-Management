using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Doctors")]
    public class Doctor
    {
        [Key]
        public int DoctorId { get; set; }

        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(10)]
        public string Mobile { get; set; } = null!;

        [Required]
        [MaxLength(100)]
        public string Specialization { get; set; } = null!;

        // SECURITY: Never store plain password
        [Required]
        [MaxLength(500)]
        public string Password { get; set; } = null!;

    }
}
