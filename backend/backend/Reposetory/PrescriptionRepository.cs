using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Reposetory
{
    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Prescription> AddAsync(Prescription prescription)
        {
            await _context.Prescriptions.AddAsync(prescription);
            return prescription;
        }
        public async Task<Prescription?> GetByIdAsync(int prescriptionId)
        {
            return await _context.Prescriptions
                .Include(p => p.Doctor)
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.PrescriptionId == prescriptionId);
        }

        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId, int doctorId)
        {
            return await _context.Prescriptions
                .Include(p => p.Doctor)
                .Where(p => p.PatientId == patientId && p.DoctorId == doctorId)
                .OrderByDescending(p => p.PrescriptionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetByDoctorIdAsync(int doctorId, int patientId)
        {
            return await _context.Prescriptions
                .Where(p => p.DoctorId == doctorId && p.PatientId == patientId)
                .Select(p => new Prescription
                {
                    PrescriptionId = p.PrescriptionId,
                    DoctorId = p.DoctorId,
                    PatientId = p.PatientId,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    ImagePublicId = p.ImagePublicId,
                    Patient = new Patient
                    {
                        PatientId = p.Patient.PatientId,
                        Name = p.Patient.Name,
                        Mobile = p.Patient.Mobile
                    }
                })
                .OrderByDescending(p => p.PrescriptionId)
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
