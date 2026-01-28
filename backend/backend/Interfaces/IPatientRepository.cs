using backend.Models;

namespace backend.Interfaces
{
    public interface IPatientRepository
    {
        Task<IEnumerable<Patient>> GetAllAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> UpdateAsync(int id, string name, string mobile, string password);
        Task<bool> MobileExistsAsync(string mobile);
    }
}
    