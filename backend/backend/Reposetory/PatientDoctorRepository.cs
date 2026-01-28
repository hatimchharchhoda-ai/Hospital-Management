using backend.Data;
using backend.Interfaces;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace backend.Reposetory
{
    public class PatientDoctorRepository : IPatientDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientDoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Check if mapping already exists
        public async Task<PatientDoctor?> GetByPatientAndDoctorAsync(int patientId, int doctorId)
        {
            return await _context.PatientDoctors
                .FirstOrDefaultAsync(pd => pd.PatientId == patientId && pd.DoctorId == doctorId);
        }

        // Create new mapping
        public async Task<PatientDoctor> CreateAsync(PatientDoctor patientDoctor)
        {
            _context.PatientDoctors.Add(patientDoctor);
            await _context.SaveChangesAsync();
            return patientDoctor;
        }

        public async Task<List<Doctor>> GetDoctorsByPatientIdAsync(int patientId)
        {
            return await _context.PatientDoctors
                .Where(pd => pd.PatientId == patientId)
                .Select(pd => pd.Doctor)
                .ToListAsync();
        }

        public async Task<List<Patient>> GetPatientsByDoctorIdAsync(int doctorId)
        {
            return await _context.PatientDoctors
                .Where(pd => pd.DoctorId == doctorId)
                .Select(pd => pd.Patient)
                .ToListAsync();
        }
    }
}
