using backend.Interfaces;

namespace backend.Services
{
    public interface IDoctorService
    {
        Task<bool> VerifyDoctorCanRegisterAsync(string email, string mobile);
        Task<int?> GetDoctorIdAsync(string identifier);
    }

    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        public async Task<bool> VerifyDoctorCanRegisterAsync(string email, string mobile)
        {
            var exists = await _doctorRepository
                .ExistsByEmailOrMobileAsync(email, mobile);

            return !exists;
        }

        public async Task<int?> GetDoctorIdAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            var doctor = await _doctorRepository.GetDoctorByEmailOrMobileAsync(identifier);

            return doctor?.DoctorId;
        }
    }
}
