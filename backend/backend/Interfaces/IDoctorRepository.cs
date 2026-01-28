using backend.Models;

namespace backend.Interfaces
{
    public interface IDoctorRepository
    {
        Task<bool> ExistsByEmailOrMobileAsync(string email, string mobile);
        Task<Doctor?> GetDoctorByEmailOrMobileAsync(string identifier);
    }
}
