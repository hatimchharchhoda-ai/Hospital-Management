using backend.DTOs;
using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllUserAsync();
        Task<Patient?> GetByIdAsync(int id);
        Task<Patient?> UpdateAsync(int id, string name, string mobile, string password);
        Task<bool> MobileExistsAsync(string mobile);
        Task<bool> AddDoctorAsync(int patientId, string doctorIdentifier);
    }

    public class PatientService : IPatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IDoctorService _doctorService;
        private readonly IPatientDoctorService _patientDoctorService;

        public PatientService(IPatientRepository patientRepository, IDoctorService doctorService, IPatientDoctorService patientDoctorService)
        {
            _patientRepository = patientRepository;
            _doctorService = doctorService;
            _patientDoctorService = patientDoctorService;
        }

        public async Task<IEnumerable<Patient>> GetAllUserAsync()
        {
            return await _patientRepository.GetAllAsync();
        }

        public async Task<Patient?> GetByIdAsync(int id)
        {
            return await _patientRepository.GetByIdAsync(id);
        }

        public async Task<Patient?> UpdateAsync(
            int id, string name, string mobile, string password)
        {
            // 🔐 Hash password ONLY if it is provided
            string hashedPassword = password;

            if (!string.IsNullOrWhiteSpace(password))
            {
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            }

            // BUSINESS RULES CAN GO HERE LATER
            return await _patientRepository.UpdateAsync(
                id, name, mobile, hashedPassword);
        }

        public async Task<bool> MobileExistsAsync(string mobile)
        {
            return await _patientRepository.MobileExistsAsync(mobile);
        }

        public async Task<bool> AddDoctorAsync(int patientId, string doctorIdentifier)
        {
            if (patientId <= 0 || string.IsNullOrWhiteSpace(doctorIdentifier))
                return false;

            // 1️⃣ Get DoctorId from identifier
            var doctorId = await _doctorService.GetDoctorIdAsync(doctorIdentifier);
            if (doctorId == null)
                return false; // doctor not found

            // 2️⃣ Create mapping (PatientDoctor)
            var result = await _patientDoctorService.CreateMappingAsync(patientId, doctorId.Value);

            return result != null; // true if created, false if already exists
        }
    }
}
