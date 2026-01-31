using backend.DTOs;
using backend.Exceptions;
using backend.Interfaces;
using backend.Models;

namespace backend.Services
{
    public interface IPrescriptionService
    {
        Task<Prescription> CreateAsync(CreatePrescriptionDto dto);
        Task<Prescription> GetByIdAsync(int prescriptionId);
        Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId, int doctorId);
        Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId, int patientId);
    }
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IPrescriptionRepository _prescriptionRepository;
        private readonly IPhotoService _photoService;

        public PrescriptionService(
            IPrescriptionRepository prescriptionRepository,
            IPhotoService photoService)
        {
            _prescriptionRepository = prescriptionRepository;
            _photoService = photoService;
        }

        public async Task<Prescription> CreateAsync(CreatePrescriptionDto dto)
        {
            // 1️⃣ Upload image to Cloudinary
            var uploadResult = await _photoService.UploadPrescriptionImageAsync(dto.Image);

            if (uploadResult.Url == null)
                throw new AppException("Image upload failed");

            // 2️⃣ Map DTO → Entity
            var prescription = new Prescription
            {
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                Description = dto.Description,
                ImageUrl = uploadResult.Url,
                ImagePublicId = uploadResult.PublicId
            };

            // 3️⃣ Save to DB
            await _prescriptionRepository.AddAsync(prescription);
            await _prescriptionRepository.SaveChangesAsync();

            return prescription;
        }

        public async Task<Prescription> GetByIdAsync(int prescriptionId)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);

            if (prescription == null)
                throw new AppException("Prescription not found");

            return prescription;
        }

        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId, int doctorId)
        {
            return await _prescriptionRepository.GetByPatientIdAsync(patientId, doctorId);
        }

        public async Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId, int patientId)
        {
            return await _prescriptionRepository.GetByDoctorIdAsync(doctorId, patientId);
        }
    }
}
