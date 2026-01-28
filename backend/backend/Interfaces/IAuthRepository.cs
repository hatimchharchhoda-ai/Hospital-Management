using backend.Models;

namespace backend.Interfaces
{
    public interface IAuthRepository
    {
        // ---------------- PATIENT ----------------
        Task<Patient> AddPatientAsync(Patient patient);
        Task<Patient?> GetPatientByMobileAsync(string mobile);

        // ---------------- DOCTOR (FUTURE) ----------------
         Task<Doctor> RegisterDoctorAsync(Doctor doctor);
        Task<Doctor?> GetByEmailOrMobileAsync(string identifier);
    }
}
