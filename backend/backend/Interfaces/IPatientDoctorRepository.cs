using backend.Models;

namespace backend.Interfaces
{
    public interface IPatientDoctorRepository
    {
        Task<PatientDoctor?> GetByPatientAndDoctorAsync(int patientId, int doctorId);
        Task<PatientDoctor> CreateAsync(PatientDoctor patientDoctor);
        Task<List<Doctor>> GetDoctorsByPatientIdAsync(int patientId);
        Task<List<Patient>> GetPatientsByDoctorIdAsync(int doctorId);

    }
}
