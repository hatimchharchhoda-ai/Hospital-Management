namespace backend.DTOs
{
    public class UpdateAppointmentRequest
    {
        public int AppointmentId { get; set; }

        public DateOnly AppointmentDate { get; set; }

        public TimeOnly StartTime { get; set; }

        public int PatientId { get; set; }

        public int  DoctorId { get; set; }

    }
}
