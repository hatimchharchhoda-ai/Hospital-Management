using backend.Interfaces;
using backend.Exceptions;

namespace backend.Services
{
    public interface IDoctorService
    {
        Task<bool> VerifyDoctorCanRegisterAsync(string email, string mobile);
        Task<int> GetDoctorIdAsync(string identifier);
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
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(mobile))
                throw new AppException("Email or mobile must be provided.");

            var exists = await _doctorRepository.ExistsByEmailOrMobileAsync(email, mobile);

            return !exists;
        }

        public async Task<int> GetDoctorIdAsync(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new AppException("Doctor identifier is required.");

            var doctor = await _doctorRepository.GetDoctorByEmailOrMobileAsync(identifier);

            if (doctor == null)
                throw new AppException("Doctor not found with the provided identifier.");

            return doctor.DoctorId;
        }
    }
}