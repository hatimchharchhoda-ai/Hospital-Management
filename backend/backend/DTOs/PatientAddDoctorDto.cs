namespace backend.DTOs
{
    public class PatientAddDoctorDto
    {
        public int PatientId { get; set; }
        public string DoctorIdentifier { get; set; } = null!;
    }
}
