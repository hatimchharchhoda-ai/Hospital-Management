using backend.Models;

namespace backend.DTOs
{
    public class AppointmentListDto
    {
        public int AppointmentId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public AppointmentStatus Status { get; set; }

        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = null!;

        public int PatientId { get; set; }
        public string PatientName { get; set; } = null!;
    }
}
