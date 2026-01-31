using backend.Models;

namespace backend.Interfaces
{
    public interface IPrescriptionRepository
    {
        Task<Prescription> AddAsync(Prescription prescription);
        Task<bool> SaveChangesAsync();
        Task<Prescription?> GetByIdAsync(int prescriptionId);
        Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId, int doctorId);
        Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId, int patientId);
    }
}
