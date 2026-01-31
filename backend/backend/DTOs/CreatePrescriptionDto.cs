using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class CreatePrescriptionDto
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
