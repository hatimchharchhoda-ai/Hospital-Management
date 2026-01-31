using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Prescriptions")]
    public class Prescription
    {
        [Key]
        public int PrescriptionId { get; set; }

        // 🔗 Foreign key to Doctor
        [Required]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public Doctor Doctor { get; set; } = null!;

        // 🔗 Foreign key to Patient
        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; } = null!;

        // 📝 Prescription details
        [Required(ErrorMessage = "Prescription description is required")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; } = string.Empty;

        // ☁️ Cloudinary Image URL
        [Required(ErrorMessage = "Prescription image is required")]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        // ☁️ Cloudinary Public ID (for delete/update)
        [Required]
        [StringLength(200)]
        public string ImagePublicId { get; set; } = string.Empty;
    }
}
