using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

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

        // 🗓 Date and time
        [Required]
        public DateOnly AppointmentDate { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        // Status
        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;
    }

    public enum AppointmentStatus
    {
        Booked = 1,
        Cancelled = 2,
        Completed = 3
    }
}
