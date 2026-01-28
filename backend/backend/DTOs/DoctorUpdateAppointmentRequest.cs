using backend.Models;

namespace backend.DTOs
{
    public class DoctorUpdateAppointmentRequest
    {
        public int AppointmentId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public AppointmentStatus Status { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }
    }
}
