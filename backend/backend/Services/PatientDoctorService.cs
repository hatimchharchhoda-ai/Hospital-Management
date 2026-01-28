using backend.Interfaces;
using backend.Models;
using backend.Exceptions;

namespace backend.Services
{
    public interface IPatientDoctorService
    {
        Task<PatientDoctor> CreateMappingAsync(int patientId, int doctorId);
        Task<List<Doctor>> GetDoctorsByPatientIdAsync(int patientId);
        Task<List<Patient>> GetPatientsByDoctorIdAsync(int doctorId);
    }

    public class PatientDoctorService : IPatientDoctorService
    {
        private readonly IPatientDoctorRepository _repository;

        public PatientDoctorService(IPatientDoctorRepository repository)
        {
            _repository = repository;
        }

        public async Task<PatientDoctor> CreateMappingAsync(int patientId, int doctorId)
        {
            // 1️⃣ Validate input
            if (patientId <= 0 || doctorId <= 0)
                throw new AppException("PatientId and DoctorId must be valid.");

            // 2️⃣ Check for duplicate
            var existing = await _repository.GetByPatientAndDoctorAsync(patientId, doctorId);
            if (existing != null)
                throw new AppException("This patient is already linked with the doctor.");

            // 3️⃣ Create new mapping
            var newMapping = new PatientDoctor
            {
                PatientId = patientId,
                DoctorId = doctorId
            };

            return await _repository.CreateAsync(newMapping);
        }

        public Task<List<Doctor>> GetDoctorsByPatientIdAsync(int patientId)
        {
            return _repository.GetDoctorsByPatientIdAsync(patientId);
        }

        public Task<List<Patient>> GetPatientsByDoctorIdAsync(int doctorId)
        {
            return _repository.GetPatientsByDoctorIdAsync(doctorId);
        }
    }
}