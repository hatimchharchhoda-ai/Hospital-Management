using backend.DTOs;
using backend.Interfaces;
using backend.Models;
using backend.Exceptions;

namespace backend.Services
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllUserAsync();
        Task<Patient> GetByIdAsync(int id);
        Task<Patient> UpdateAsync(int id, string name, string mobile, string password);
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

        public async Task<Patient> GetByIdAsync(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
                throw new AppException($"Patient with ID {id} not found.");

            return patient;
        }

        public async Task<Patient> UpdateAsync(int id, string name, string mobile, string password)
        {
            // 🔐 Hash password ONLY if provided
            string hashedPassword = password;
            if (!string.IsNullOrWhiteSpace(password))
            {
                hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            }

            // Update patient
            var updatedPatient = await _patientRepository.UpdateAsync(id, name, mobile, hashedPassword);

            if (updatedPatient == null)
                throw new AppException($"Patient with ID {id} not found or could not be updated.");

            return updatedPatient;
        }

        public async Task<bool> MobileExistsAsync(string mobile)
        {
            return await _patientRepository.MobileExistsAsync(mobile);
        }

        public async Task<bool> AddDoctorAsync(int patientId, string doctorIdentifier)
        {
            if (patientId <= 0)
                throw new AppException("Invalid patient ID.");

            if (string.IsNullOrWhiteSpace(doctorIdentifier))
                throw new AppException("Doctor identifier is required.");

            // 1️⃣ Get DoctorId from identifier
            var doctorId = await _doctorService.GetDoctorIdAsync(doctorIdentifier);
            if (doctorId <= 0)
                throw new AppException("Doctor not found with the provided identifier.");

            // 2️⃣ Create mapping (PatientDoctor)
            var result = await _patientDoctorService.CreateMappingAsync(patientId, doctorId);

            return result != null; // true if created
        }
    }
}